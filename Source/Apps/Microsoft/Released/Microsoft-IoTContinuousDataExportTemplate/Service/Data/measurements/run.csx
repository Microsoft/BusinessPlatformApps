#r "System.Configuration"
#r "System.Data"
#r "Newtonsoft.Json"
#r "Microsoft.WindowsAzure.Storage"

using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using System.Text;

using Microsoft.Azure.WebJobs;

using Microsoft.Hadoop.Avro;
using Microsoft.Hadoop.Avro.Container;
using Microsoft.WindowsAzure.Storage.Blob;

using Newtonsoft.Json;

private static int counter = 0;

// Process measurements data
public static async Task Run(CloudBlockBlob myBlob, TraceWriter log, ExecutionContext context)
{
    log.Info($"{GetLogPrefix(context)} - Processing blob {myBlob.StorageUri}");

    await myBlob.FetchAttributesAsync();
    var timestamp = myBlob.Properties.LastModified.Value;
    int.TryParse(ConfigurationManager.AppSettings["SQL_CONNECTIONSTRING"], out int historyDataHours);
    if (historyDataHours > 0 && DateTime.UtcNow.Subtract(timestamp.UtcDateTime) > TimeSpan.FromHours(historyDataHours))
    {
        log.Info($"{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")} - Blob timestamp: {timestamp} older than {historyDataHours} hour, ignored");
        return;
    }

    var currentCount = System.Threading.Interlocked.Increment(ref counter);
    log.Info($"{GetLogPrefix(context)} - Concurrent job count: {currentCount}");

    IList<Message> messages = new List<Message>();
    int parseFailCount = 0;

    using (var blobStream = new MemoryStream())
    {
        var stopWatch = System.Diagnostics.Stopwatch.StartNew();
        // Download blob content and save them to memory stream
        await myBlob.DownloadToStreamAsync(blobStream);
        stopWatch.Stop();
        blobStream.Position = 0;
        log.Info($"{GetLogPrefix(context)} - Downloaded blob content. Length: {blobStream.Length}. Elapsed: {stopWatch.Elapsed}");

        // Create Avro reader from memory stream
        using (var reader = AvroContainer.CreateGenericReader(blobStream))
        {
            // Loop through blocks within the container
            while (reader.MoveNext())
            {
                // Loop through Avro record inside the block and get all the fields
                foreach (AvroRecord record in reader.Current.Objects)
                {
                    try
                    {
                        var messageId = Guid.NewGuid();
                        var systemProperties = record.GetField<IDictionary<string, object>>("SystemProperties");
                        var deviceId = systemProperties["connectionDeviceId"] as string;
                        var enqueueTime = DateTime.Parse(record.GetField<string>("EnqueuedTimeUtc"));

                        using (var stream = new MemoryStream(record.GetField<byte[]>("Body")))
                        {
                            using (var streamReader = new StreamReader(stream, Encoding.UTF8))
                            {
                                try
                                {
                                    var body = JsonSerializer.Create().Deserialize(streamReader, typeof(IDictionary<string, dynamic>)) as IDictionary<string, dynamic>;
                                    messages.Add(new Message
                                    {
                                        messageId = messageId,
                                        timestamp = enqueueTime,
                                        deviceId = deviceId,
                                        values = body,
                                        messageSize = (int)stream.Length
                                    });
                                }
                                catch (Exception e)
                                {
                                    log.Error($"{GetLogPrefix(context)} - Failed to process the body for device {deviceId}");
                                    log.Error($"{GetLogPrefix(context)} - {e.ToString()}");
                                    parseFailCount++;
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error($"{GetLogPrefix(context)} - Failed to process Avro record");
                        log.Error($"{GetLogPrefix(context)} - {e.ToString()}");
                        parseFailCount++;
                    }
                }
            }
        }
    }

    log.Info($"{GetLogPrefix(context)} - Parsed {messages.Count} messages with {parseFailCount} failures");

    var measurementsTable = CreateMeasurementsTable();
    var messagesTable = CreateMessagesTable();
    foreach (var message in messages)
    {
        var messageRow = messagesTable.NewRow();
        messageRow["id"] = message.messageId;
        messageRow["deviceId"] = message.deviceId;
        messageRow["timestamp"] = message.timestamp;
        messageRow["size"] = message.messageSize;
        messagesTable.Rows.Add(messageRow);

        foreach (KeyValuePair<string, dynamic> entry in message.values)
        {
            var row = measurementsTable.NewRow();
            row["messageId"] = message.messageId;
            row["deviceId"] = message.deviceId;
            row["timestamp"] = message.timestamp;
            row["field"] = entry.Key;

            switch (entry.Value)
            {
                case bool _:
                    row["booleanValue"] = bool.Parse(entry.Value.ToString());
                    break;

                case int _:
                case Int64 _:
                case double _:
                case float _:
                    row["numericValue"] = float.Parse(entry.Value.ToString());
                    break;

                case null:
                    break;

                default:
                    row["stringValue"] = entry.Value.ToString();
                    break;
            }

            measurementsTable.Rows.Add(row);
        }
    }


    var cs = ConfigurationManager.AppSettings["SQL_CONNECTIONSTRING"];
    using (SqlConnection conn = new SqlConnection(cs))
    {
        conn.Open();

        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn) { BulkCopyTimeout = 120 })
        {
            log.Info($"{GetLogPrefix(context)} - Inserting into table: {messagesTable.TableName}");
            bulkCopy.DestinationTableName = "analytics.Messages";
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                await bulkCopy.WriteToServerAsync(messagesTable);
                stopWatch.Stop();
                log.Info($"{GetLogPrefix(context)} - Added {messagesTable.Rows.Count} rows to the database table {messagesTable.TableName}. Elapsed: {stopWatch.Elapsed}");
            }
            catch (Exception exception)
            {
                stopWatch.Stop();
                log.Error($"{GetLogPrefix(context)} - Elapsed: {stopWatch.Elapsed} - database table {messagesTable.TableName}", exception);
                System.Threading.Interlocked.Decrement(ref counter);
                throw;
            }
        }

        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn) { BulkCopyTimeout = 120 })
        {
            foreach (DataColumn column in measurementsTable.Columns)
            {
                bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
            }

            log.Info($"{GetLogPrefix(context)} - Inserting into table: {measurementsTable.TableName}");
            bulkCopy.DestinationTableName = "stage.Measurements";
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                await bulkCopy.WriteToServerAsync(measurementsTable);
                stopWatch.Stop();
                log.Info($"{GetLogPrefix(context)} - Added {measurementsTable.Rows.Count} rows to the database table {measurementsTable.TableName}. Elapsed: {stopWatch.Elapsed}");
            }
            catch (Exception exception)
            {
                stopWatch.Stop();
                log.Error($"{GetLogPrefix(context)} - Elapsed: {stopWatch.Elapsed} - database table {measurementsTable.TableName}", exception);
                System.Threading.Interlocked.Decrement(ref counter);
                throw;
            }
        }

        System.Threading.Interlocked.Decrement(ref counter);
    }
}

private static string GetLogPrefix(ExecutionContext context)
{
    return $"{System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")} - {context.InvocationId}";
}

// The length for the columns matches the length inside database
private static DataTable CreateMeasurementsTable()
{
    var table = new DataTable("Measurements");

    table.Columns.Add(new DataColumn("messageId", typeof(Guid)));
    table.Columns.Add(new DataColumn("deviceId", typeof(string)) { MaxLength = 200 });
    table.Columns.Add(new DataColumn("timestamp", typeof(DateTime)));
    table.Columns.Add(new DataColumn("field", typeof(string)) { MaxLength = 255 });
    table.Columns.Add(new DataColumn("numericValue", typeof(decimal)));
    table.Columns.Add(new DataColumn("stringValue", typeof(string)));
    table.Columns.Add(new DataColumn("booleanValue", typeof(bool)));

    return table;
}

private static DataTable CreateMessagesTable()
{
    var table = new DataTable("Messages");

    table.Columns.Add(new DataColumn("id", typeof(Guid)));
    table.Columns.Add(new DataColumn("deviceId", typeof(string)) { MaxLength = 200 });
    table.Columns.Add(new DataColumn("timestamp", typeof(DateTime)));
    table.Columns.Add(new DataColumn("size", typeof(int)));

    return table;
}

public struct Message
{
    public Guid messageId;
    public DateTime timestamp;
    public string deviceId;
    public IDictionary<string, dynamic> values;
    public int messageSize;
}