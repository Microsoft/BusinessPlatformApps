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

using Newtonsoft.Json;

// Device data processing
public static async Task Run(CloudBlockBlob myBlob, TraceWriter log)
{
    log.Info($"{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")} - Processing blob {myBlob.StorageUri}");

    await myBlob.FetchAttributesAsync();
    var timestamp = myBlob.Properties.LastModified.Value;

    var devices = new List<Device>();
    int parseFailCount = 0;

    using (var blobStream = new MemoryStream())
    {
        // Download blob content and save them to memory stream
        await myBlob.DownloadToStreamAsync(blobStream);
        blobStream.Position = 0;

        // Create generic Avro reader from memory stream
        using (var reader = AvroContainer.CreateGenericReader(blobStream))
        {
            // For one Avro Container, it may contains multiple blocks
            // Loop through each block within the container
            while (reader.MoveNext())
            {
                // Loop through Avro record inside the block and extract the fields
                foreach (AvroRecord record in reader.Current.Objects)
                {
                    try
                    {
                        var fields = record.Schema.Fields;
                        var deviceId = record.GetField<string>("id");

                        var connectionDeviceId = deviceId;
                        if (fields.Any(field => field.Name == "deviceId"))
                        {
                            connectionDeviceId = record.GetField<string>("deviceId");
                        }

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
                            ConnectionDeviceId = connectionDeviceId,
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
                        log.Error($"{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")} - Failed to process Avro record");
                        log.Error($"{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")} - {e.ToString()}");
                        parseFailCount++;
                    }
                }
            }
        }
    }

    log.Info($"{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")} - Parsed {devices.Count} devices with {parseFailCount} failures");

    var devicesTable = CreateDevicesTable();
    var propertiesTable = CreatePropertiesTable();

    foreach (var device in devices)
    {
        var deviceRow = devicesTable.NewRow();
        deviceRow["deviceId"] = device.DeviceId;
        deviceRow["connectionDeviceId"] = device.ConnectionDeviceId;
        deviceRow["deviceTemplate"] = $"{device.DeviceTemplateId}/{device.DeviceTemplateVersion}";
        deviceRow["name"] = device.DeviceName;
        deviceRow["simulated"] = device.Simulated;
        deviceRow["timestamp"] = timestamp.UtcDateTime;

        devicesTable.Rows.Add(deviceRow);

        device.CloudProperties.ToList().ForEach(entry => ProcessingProperty(device, entry, PropertyKind.CloudProperty, propertiesTable, timestamp));
        device.DeviceProperties.ToList().ForEach(entry => ProcessingProperty(device, entry, PropertyKind.DeviceProperty, propertiesTable, timestamp));
        device.DeviceSettings.ToList().ForEach(entry => ProcessingProperty(device, entry, PropertyKind.DeviceSetting, propertiesTable, timestamp));
    }

    var cs = ConfigurationManager.AppSettings["SQL_CONNECTIONSTRING"];
    using (SqlConnection conn = new SqlConnection(cs))
    {
        conn.Open();

        log.Info($"{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")} - Inserting into table: {devicesTable.TableName}");
        using (SqlCommand cmd = new SqlCommand("dbo.[InsertDevices]", conn) { CommandType = CommandType.StoredProcedure })
        {
            cmd.Parameters.Add(new SqlParameter("@tableType", devicesTable));
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                var rows = await cmd.ExecuteNonQueryAsync();
                stopWatch.Stop();
                log.Info($"{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")} - Added/Updated {rows} rows to the database. Elapsed: {stopWatch.Elapsed}");
            }
            catch (Exception exception)
            {
                stopWatch.Stop();
                log.Error($"{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")} - Elapsed: {stopWatch.Elapsed}", exception);
                throw;
            }
        }

        log.Info($"{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")} - Inserting into table: {propertiesTable.TableName}");
        using (SqlCommand cmd = new SqlCommand("dbo.[InsertProperties]", conn) { CommandType = CommandType.StoredProcedure })
        {
            cmd.Parameters.Add(new SqlParameter("@tableType", propertiesTable));
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                var rows = await cmd.ExecuteNonQueryAsync();
                stopWatch.Stop();
                log.Info($"{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")} - Added/Updated {rows} rows to the database. Elapsed: {stopWatch.Elapsed}");
            }
            catch (Exception exception)
            {
                stopWatch.Stop();
                log.Error($"{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")} - Elapsed: {stopWatch.Elapsed}", exception);
                throw;
            }
        }
    }
}

private static void ProcessingProperty(Device device, KeyValuePair<string, dynamic> entry, PropertyKind propertyKind, DataTable propertiesTable, DateTimeOffset timestamp)
{
    var propertyRow = propertiesTable.NewRow();

    propertyRow["id"] = $"{device.DeviceId}/{propertyKind.ToString()}/{entry.Key}";
    propertyRow["deviceId"] = device.DeviceId;
    propertyRow["deviceTemplate"] = $"{device.DeviceTemplateId}/{device.DeviceTemplateVersion}";
    propertyRow["propertyDefinition"] = $"{device.DeviceTemplateId}/{device.DeviceTemplateVersion}/{propertyKind.ToString()}/{entry.Key}";
    propertyRow["timestamp"] = timestamp.UtcDateTime;

    switch (entry.Value)
    {
        case bool _:
            propertyRow["booleanValue"] = bool.Parse(entry.Value.ToString());
            break;
        case int _:
        case Int64 _:
        case double _:
        case float _:
            propertyRow["numericValue"] = float.Parse(entry.Value.ToString());
            break;
        case string _:
            propertyRow["stringValue"] = entry.Value;
            break;
        case IDictionary<string, object> _:
            propertyRow["stringValue"] = JsonConvert.SerializeObject(entry.Value);
            break;
        case null:
            break;
        default:
            propertyRow["stringValue"] = JsonConvert.SerializeObject(entry.Value);
            break;
    }

    propertiesTable.Rows.Add(propertyRow);
}

// The length for the columns matches the length inside database
private static DataTable CreateDevicesTable()
{
    var table = new DataTable("Devices");
    table.Columns.Add(new DataColumn("deviceId", typeof(string)) { MaxLength = 200 });
    table.Columns.Add(new DataColumn("connectionDeviceId", typeof(string)) { MaxLength = 200 });
    table.Columns.Add(new DataColumn("deviceTemplate", typeof(string)) { MaxLength = 101 });
    table.Columns.Add(new DataColumn("name", typeof(string)) { MaxLength = 200 });
    table.Columns.Add(new DataColumn("simulated", typeof(bool)));
    table.Columns.Add(new DataColumn("timestamp", typeof(DateTime)));

    return table;
}

private static DataTable CreatePropertiesTable()
{
    var table = new DataTable("Properties");
    table.Columns.Add(new DataColumn("id", typeof(string)) { MaxLength = 507 });
    table.Columns.Add(new DataColumn("deviceId", typeof(string)) { MaxLength = 200 });
    table.Columns.Add(new DataColumn("deviceTemplate", typeof(string)) { MaxLength = 101 });
    table.Columns.Add(new DataColumn("propertyDefinition", typeof(string)) { MaxLength = 408 });
    table.Columns.Add(new DataColumn("timestamp", typeof(DateTime)));
    table.Columns.Add(new DataColumn("numericValue", typeof(decimal)));
    table.Columns.Add(new DataColumn("stringValue", typeof(string)));
    table.Columns.Add(new DataColumn("booleanValue", typeof(bool)));

    return table;
}

private struct Device
{
    public string DeviceId { get; set; }

    public string ConnectionDeviceId { get; set; }

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