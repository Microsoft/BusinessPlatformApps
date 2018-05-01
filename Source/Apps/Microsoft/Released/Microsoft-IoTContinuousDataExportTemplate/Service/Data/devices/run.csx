#r "System.Configuration"
#r "System.Data"
#r "Newtonsoft.Json"
#r "Microsoft.WindowsAzure.Storage"

using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Hadoop.Avro;
using Microsoft.Hadoop.Avro.Container;

using Microsoft.WindowsAzure.Storage.Blob;

// Device data processing
public static async Task Run(CloudBlockBlob myBlob, TraceWriter log)
{
    log.Info($"Processing blob {myBlob.StorageUri}");

    var devices = new List<Device>();
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
                    log.Info(record.ToString());
                    try
                    {
                        var deviceId = record.GetField<string>("id");
                        var deviceName = record.GetField<string>("name");
                        var simulated = record.GetField<bool>("simulated");

                        var deviceTemplateRecord = record.GetField<AvroRecord>("deviceTemplate");
                        var templateId = deviceTemplateRecord.GetField<string>("id");
                        var templateVersion = deviceTemplateRecord.GetField<string>("version");

                        var propertiesRecord = record.GetField<AvroRecord>("properties");
                        var cloudProperties = propertiesRecord.GetField<IDictionary<string, dynamic>>("cloud");
                        var deviceProperties = propertiesRecord.GetField<IDictionary<string, dynamic>>("device");

                        var settingsRecord = record.GetField<AvroRecord>("settings");
                        var deviceSettings = settingsRecord.GetField<IDictionary<string, dynamic>>("device");

                        devices.Add(new Device()
                        {
                            DeviceId = deviceId,
                            DeviceName = deviceName,
                            Simulated = simulated,
                            DeviceTemplateId = templateId,
                            DeviceTemplateVersion = templateVersion,
                            CloudProperties = cloudProperties,
                            DeviceProperties = deviceProperties,
                            DeviceSettings = deviceSettings
                        });
                    }
                    catch (Exception e)
                    {
                        log.Error("$Failed to process Avro record");
                        log.Error(e.ToString());
                        parseFailCount++;
                    }
                }
            }
        }
    }

    log.Info($"Parsed {devices.Count} devices with {parseFailCount} failures");

    var devicesTable = CreateDevicesTable();
    var propertiesTable = CreatePropertiesTable();

    foreach (var device in devices)
    {
        var deviceRow = devicesTable.NewRow();
        deviceRow["deviceId"] = device.DeviceId;
        deviceRow["model"] = $"{device.DeviceTemplateId}/{device.DeviceTemplateVersion}";
        deviceRow["name"] = device.DeviceName;
        deviceRow["simulated"] = device.Simulated;

        devicesTable.Rows.Add(deviceRow);

        device.CloudProperties.ToList().ForEach(entry => ProcessingProperty(device, entry, PropertyKind.CloudProperty, propertiesTable));
        device.DeviceProperties.ToList().ForEach(entry => ProcessingProperty(device, entry, PropertyKind.DeviceProperty, propertiesTable));
        device.DeviceSettings.ToList().ForEach(entry => ProcessingProperty(device, entry, PropertyKind.DeviceSetting, propertiesTable));
    }

    var cs = ConfigurationManager.AppSettings["SQL_CONNECTIONSTRING"];
    using (SqlConnection conn = new SqlConnection(cs))
    {
        conn.Open();

        log.Info($"Inserting into table: {devicesTable.TableName}");
        using (SqlCommand cmd = new SqlCommand("dbo.[InsertDevices]", conn) { CommandType = CommandType.StoredProcedure })
        {
            cmd.Parameters.Add(new SqlParameter("@tableType", devicesTable));
            var rows = await cmd.ExecuteNonQueryAsync();
            log.Info($"Added {rows} rows to the database");
        }

        log.Info($"Inserting into table: {propertiesTable.TableName}");
        using (SqlCommand cmd = new SqlCommand("dbo.[InsertProperties]", conn) { CommandType = CommandType.StoredProcedure })
        {
            cmd.Parameters.Add(new SqlParameter("@tableType", propertiesTable));
            var rows = await cmd.ExecuteNonQueryAsync();
            log.Info($"Added {rows} rows to the database");
        }
    }
}

private static void ProcessingProperty(Device device, KeyValuePair<string, dynamic> entry, PropertyKind propertyKind, DataTable propertiesTable)
{
    var propertyRow = propertiesTable.NewRow();

    propertyRow["id"] = $"{device.DeviceId}/{propertyKind.ToString()}/{entry.Key}";
    propertyRow["deviceId"] = device.DeviceId;
    propertyRow["model"] = $"{device.DeviceTemplateId}/{device.DeviceTemplateVersion}";
    propertyRow["definition"] = $"{device.DeviceTemplateId}/{device.DeviceTemplateVersion}/{propertyKind.ToString()}/{entry.Key}";
    // TODO: decide what to do with this
    propertyRow["lastUpdated"] = DateTime.UtcNow;
    // propertyRow["field"] = entry.Key;
    // propertyRow["kind"] = propertyKind.ToString();

    switch (entry.Value)
    {
        case bool _:
            propertyRow["booleanValue"] = bool.Parse(entry.Value.ToString());
            break;
        case int _:
        case Int64 _:
        case double _:
        case float _:
            propertyRow["numericValue"] = decimal.Parse(entry.Value.ToString());
            break;
        case null:
            break;
        default:
            propertyRow["stringValue"] = entry.Value.ToString();
            break;
    }

    propertiesTable.Rows.Add(propertyRow);
}

private static DataTable CreateDevicesTable()
{
    var table = new DataTable("Devices");
    table.Columns.Add(new DataColumn("deviceId", typeof(string)) { MaxLength = 200 });
    table.Columns.Add(new DataColumn("model", typeof(string)) { MaxLength = 101 });
    table.Columns.Add(new DataColumn("name", typeof(string)) { MaxLength = 200 });
    table.Columns.Add(new DataColumn("simulated", typeof(bool)));

    return table;
}

private static DataTable CreatePropertiesTable()
{
    var table = new DataTable("Properties");
    table.Columns.Add(new DataColumn("id", typeof(string)) { MaxLength = 507 });
    table.Columns.Add(new DataColumn("deviceId", typeof(string)) { MaxLength = 200 });
    table.Columns.Add(new DataColumn("model", typeof(string)) { MaxLength = 101 });
    table.Columns.Add(new DataColumn("definition", typeof(string)) { MaxLength = 408 });
    table.Columns.Add(new DataColumn("lastUpdated", typeof(DateTime)));
    // table.Columns.Add(new DataColumn("field", typeof(string)) { MaxLength = 255 });
    // table.Columns.Add(new DataColumn("kind", typeof(string)) { MaxLength = 50 });
    table.Columns.Add(new DataColumn("numericValue", typeof(decimal)));
    table.Columns.Add(new DataColumn("stringValue", typeof(string)));
    table.Columns.Add(new DataColumn("booleanValue", typeof(bool)));

    return table;
}

private struct Device
{
    public string DeviceId { get; set; }

    public string DeviceName { get; set; }

    public bool Simulated { get; set; }

    public string DeviceTemplateId { get; set; }

    public string DeviceTemplateVersion { get; set; }

    public IDictionary<string, dynamic> CloudProperties { get; set; }

    public IDictionary<string, dynamic> DeviceProperties { get; set; }

    public IDictionary<string, dynamic> DeviceSettings { get; set; }
}

private enum PropertyKind
{
    CloudProperty,
    DeviceProperty,
    DeviceSetting
}