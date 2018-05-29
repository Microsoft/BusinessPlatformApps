#r "System.Configuration"
#r "System.Data"
#r "Newtonsoft.Json"
#r "Microsoft.WindowsAzure.Storage"

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Hadoop.Avro;
using Microsoft.Hadoop.Avro.Container;

using Microsoft.WindowsAzure.Storage.Blob;

// Device Template data processing
public static async Task Run(CloudBlockBlob myBlob, TraceWriter log)
{
    log.Info($"{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")} - Processing blob {myBlob.StorageUri}");

    await myBlob.FetchAttributesAsync();
    var timestamp = myBlob.Properties.LastModified.Value;

    var templates = new List<DeviceTemplate>();
    int parseFailCount = 0;

    using (var blobStream = new MemoryStream())
    {
        await myBlob.DownloadToStreamAsync(blobStream);
        blobStream.Position = 0;

        using (var reader = AvroContainer.CreateGenericReader(blobStream))
        {
            while (reader.MoveNext())
            {
                foreach (AvroRecord record in reader.Current.Objects)
                {
                    try
                    {
                        var deviceTemplateId = record.GetField<string>("id");
                        var deviceTemplateName = record.GetField<string>("name");
                        var deviceTemplateVersion = record.GetField<string>("version");

                        var measurementsRecord = record.GetField<AvroRecord>("measurements");

                        var telemetryMap = measurementsRecord.GetField<IDictionary<string, dynamic>>("telemetry");
                        var telemetry = telemetryMap.ToDictionary(e => e.Key, e => ProcessingMeasurement(e.Value as AvroRecord));

                        var statesMap = measurementsRecord.GetField<IDictionary<string, dynamic>>("states");
                        var states = statesMap.ToDictionary(e => e.Key, e => ProcessingMeasurement(e.Value as AvroRecord));

                        var eventsMap = measurementsRecord.GetField<IDictionary<string, dynamic>>("events");
                        var events = eventsMap.ToDictionary(e => e.Key, e => ProcessingMeasurement(e.Value as AvroRecord));

                        var propertiesRecord = record.GetField<AvroRecord>("properties");

                        var cloudPropertiesMap = propertiesRecord.GetField<IDictionary<string, dynamic>>("cloud");
                        var cloudProperties = cloudPropertiesMap.ToDictionary(e => e.Key, e => ProcessingProperty(e.Value as AvroRecord));

                        var devicePropertiesMap = propertiesRecord.GetField<IDictionary<string, dynamic>>("device");
                        var deviceProperties = devicePropertiesMap.ToDictionary(e => e.Key, e => ProcessingProperty(e.Value as AvroRecord));

                        var settingsRecord = record.GetField<AvroRecord>("settings");
                        var deviceSettingsMap = settingsRecord.GetField<IDictionary<string, dynamic>>("device");
                        var deviceSettings = deviceSettingsMap.ToDictionary(e => e.Key, e => ProcessingProperty(e.Value as AvroRecord));

                        templates.Add(new DeviceTemplate()
                        {
                            TemplateId = deviceTemplateId,
                            TemplateName = deviceTemplateName,
                            TemplateVersion = deviceTemplateVersion,
                            Telemetry = telemetry,
                            States = states,
                            Events = events,
                            CloudProperties = cloudProperties,
                            DeviceProperties = deviceProperties,
                            DeviceSettings = deviceSettings
                        });
                    }
                    catch (Exception e)
                    {
                        log.Error($"{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")} - Failed to process Avro record");
                        log.Error($"{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")} - {e.ToString()}");
                        parseFailCount++;
                    }
                }
            }
        }
    }

    log.Info($"{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")} - Parsed {templates.Count} templates with {parseFailCount} failures");

    var templatesTable = CreateDeviceTemplatesTable();
    var measurementDefinitionsTable = CreateMeaurementDefinitionsTable();
    var propertyDefinitionsTable = CreatePropertyDefinitionsTable();

    foreach (var template in templates)
    {
        var templateRow = templatesTable.NewRow();
        templateRow["id"] = $"{template.TemplateId}/{template.TemplateVersion}";
        templateRow["deviceTemplateId"] = template.TemplateId;
        templateRow["deviceTemplateVersion"] = template.TemplateVersion;
        templateRow["name"] = template.TemplateName;
        templateRow["timestamp"] = timestamp.UtcDateTime;

        templatesTable.Rows.Add(templateRow);

        template.Telemetry.ToList().ForEach(entry => InsertMeasurementIntoTable(template, entry.Key, entry.Value, MeasurementKind.Telemetry, measurementDefinitionsTable, timestamp));
        template.States.ToList().ForEach(entry => InsertMeasurementIntoTable(template, entry.Key, entry.Value, MeasurementKind.State, measurementDefinitionsTable, timestamp));
        template.Events.ToList().ForEach(entry => InsertMeasurementIntoTable(template, entry.Key, entry.Value, MeasurementKind.Event, measurementDefinitionsTable, timestamp));

        template.CloudProperties.ToList().ForEach(entry => InsertPropertyIntoTable(template, entry.Key, entry.Value, PropertyKind.CloudProperty, propertyDefinitionsTable, timestamp));
        template.DeviceProperties.ToList().ForEach(entry => InsertPropertyIntoTable(template, entry.Key, entry.Value, PropertyKind.DeviceProperty, propertyDefinitionsTable, timestamp));
        template.DeviceSettings.ToList().ForEach(entry => InsertPropertyIntoTable(template, entry.Key, entry.Value, PropertyKind.DeviceSetting, propertyDefinitionsTable, timestamp));
    }

    var cs = ConfigurationManager.AppSettings["SQL_CONNECTIONSTRING"];
    using (SqlConnection conn = new SqlConnection(cs))
    {
        conn.Open();

        log.Info($"{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")} - Inserting into table: {templatesTable.TableName}");
        using (SqlCommand cmd = new SqlCommand("dbo.[InsertDeviceTemplates]", conn) { CommandType = CommandType.StoredProcedure })
        {
            cmd.Parameters.Add(new SqlParameter("@tableType", templatesTable));
            var rows = await cmd.ExecuteNonQueryAsync();
            log.Info($"{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")} - Added/Updated {rows} rows to the database table {templatesTable.TableName}");
        }

        log.Info($"{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")} - Inserting into table: {measurementDefinitionsTable.TableName}");
        using (SqlCommand cmd = new SqlCommand("dbo.[InsertMeasurementDefinitions]", conn) { CommandType = CommandType.StoredProcedure })
        {
            cmd.Parameters.Add(new SqlParameter("@tableType", measurementDefinitionsTable));
            var rows = await cmd.ExecuteNonQueryAsync();
            log.Info($"{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")} - Added/Updated {rows} rows to the database table {measurementDefinitionsTable.TableName}");
        }

        log.Info($"{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")} - Inserting into table: {propertyDefinitionsTable.TableName}");
        using (SqlCommand cmd = new SqlCommand("dbo.[InsertPropertyDefinitions]", conn) { CommandType = CommandType.StoredProcedure })
        {
            cmd.Parameters.Add(new SqlParameter("@tableType", propertyDefinitionsTable));
            var rows = await cmd.ExecuteNonQueryAsync();
            log.Info($"{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")} - Added/Updated {rows} rows to the database table {propertyDefinitionsTable.TableName}");
        }
    }
}

public static bool FieldExists(dynamic dynObj, string fieldName)
{
    var recordFields = dynObj.Schema.Fields as IList<Microsoft.Hadoop.Avro.Schema.RecordField>;
    return recordFields.Any(f => f.Name.Equals(fieldName));
}

private static void InsertMeasurementIntoTable(DeviceTemplate template, string field, Measurement measurement, MeasurementKind kind, DataTable table, DateTimeOffset timestamp)
{
    var row = table.NewRow();
    row["id"] = $"{template.TemplateId}/{template.TemplateVersion}/{field}";
    row["deviceTemplate"] = $"{template.TemplateId}/{template.TemplateVersion}";
    row["field"] = field;
    row["kind"] = kind.ToString();
    row["dataType"] = measurement.DataType;
    row["name"] = measurement.Name;
    row["category"] = measurement.Category;
    row["timestamp"] = timestamp.UtcDateTime;

    table.Rows.Add(row);
}

private static void InsertPropertyIntoTable(DeviceTemplate template, string field, Property property, PropertyKind kind, DataTable table, DateTimeOffset timestamp)
{
    var row = table.NewRow();
    row["id"] = $"{template.TemplateId}/{template.TemplateVersion}/{kind.ToString()}/{field}";
    row["deviceTemplate"] = $"{template.TemplateId}/{template.TemplateVersion}";
    row["field"] = field;
    row["kind"] = kind.ToString();
    row["dataType"] = property.DataType;
    row["name"] = property.Name;
    row["timestamp"] = timestamp.UtcDateTime;

    table.Rows.Add(row);
}

private static Measurement ProcessingMeasurement(AvroRecord record)
{
    var name = record.GetField<string>("name");

    var dataType = default(string);
    if (FieldExists(record, "dataType"))
    {
        dataType = record.GetField<string>("dataType");
    }

    var category = default(string);
    if (FieldExists(record, "category"))
    {
        category = record.GetField<string>("category");
    }

    return new Measurement() { Name = name, DataType = dataType, Category = category };
}

private static Property ProcessingProperty(AvroRecord record)
{
    var name = record.GetField<string>("name");
    var dataType = record.GetField<string>("dataType");

    return new Property() { Name = name, DataType = dataType };
}

// The length for the columns matches the length inside database
private static DataTable CreateDeviceTemplatesTable()
{
    var table = new DataTable("DeviceTemplates");

    table.Columns.Add(new DataColumn("id", typeof(string)) { MaxLength = 101 });
    table.Columns.Add(new DataColumn("deviceTemplateId", typeof(string)) { MaxLength = 50 });
    table.Columns.Add(new DataColumn("deviceTemplateVersion", typeof(string)) { MaxLength = 50 });
    table.Columns.Add(new DataColumn("name", typeof(string)) { MaxLength = 1000 });
    table.Columns.Add(new DataColumn("timestamp", typeof(DateTime)));

    return table;
}

private static DataTable CreateMeaurementDefinitionsTable()
{
    var table = new DataTable("MeasurementDefinitions");

    table.Columns.Add(new DataColumn("id", typeof(string)) { MaxLength = 357 });
    table.Columns.Add(new DataColumn("deviceTemplate", typeof(string)) { MaxLength = 101 });
    table.Columns.Add(new DataColumn("field", typeof(string)) { MaxLength = 255 });
    table.Columns.Add(new DataColumn("kind", typeof(string)) { MaxLength = 50 });
    table.Columns.Add(new DataColumn("dataType", typeof(string)) { MaxLength = 100 });
    table.Columns.Add(new DataColumn("name", typeof(string)) { MaxLength = 200 });
    table.Columns.Add(new DataColumn("category", typeof(string)) { MaxLength = 100 });
    table.Columns.Add(new DataColumn("timestamp", typeof(DateTime)));

    return table;
}

private static DataTable CreatePropertyDefinitionsTable()
{
    var table = new DataTable("PropertyDefinitions");

    table.Columns.Add(new DataColumn("id", typeof(string)) { MaxLength = 408 });
    table.Columns.Add(new DataColumn("deviceTemplate", typeof(string)) { MaxLength = 101 });
    table.Columns.Add(new DataColumn("field", typeof(string)) { MaxLength = 255 });
    table.Columns.Add(new DataColumn("kind", typeof(string)) { MaxLength = 50 });
    table.Columns.Add(new DataColumn("dataType", typeof(string)) { MaxLength = 100 });
    table.Columns.Add(new DataColumn("name", typeof(string)) { MaxLength = 200 });
    table.Columns.Add(new DataColumn("timestamp", typeof(DateTime)));

    return table;
}

private struct DeviceTemplate
{
    public string TemplateId { get; set; }
    public string TemplateName { get; set; }
    public string TemplateVersion { get; set; }
    public IDictionary<string, Measurement> Telemetry { get; set; }
    public IDictionary<string, Measurement> States { get; set; }
    public IDictionary<string, Measurement> Events { get; set; }
    public IDictionary<string, Property> CloudProperties { get; set; }
    public IDictionary<string, Property> DeviceProperties { get; set; }
    public IDictionary<string, Property> DeviceSettings { get; set; }
}

private struct Measurement
{
    public string Name { get; set; }
    public string DataType { get; set; }
    public string Category { get; set; }
}

private struct Property
{
    public string Name { get; set; }
    public string DataType { get; set; }
}

private enum PropertyKind
{
    CloudProperty,
    DeviceProperty,
    DeviceSetting
}

private enum MeasurementKind
{
    Telemetry,
    State,
    Event
}