using System.ComponentModel.Composition;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Common.Model.ApiManagement;
using Microsoft.Deployment.Common.Model.Bpst;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Data;
using CsvHelper;
using System.Data.SqlClient;

namespace Microsoft.Deployment.Actions.AzureCustom.APIManagement
{
    [Export(typeof(IAction))]
    public class LoadIPLocationDB : BaseAction
    {
        private readonly Uri GEOLITE_CITY = new Uri("https://geolite.maxmind.com/download/geoip/database/GeoLite2-City-CSV.zip");
        private const int BATCH_SIZE = 10000;

        private static void LoadDataTable(CsvReader reader, DataTable table, int batchSize)
        {
            table.Rows.Clear();
       
                for (int i = 0; i < batchSize && reader.Read(); i++)
                {
                    DataRow dr = table.NewRow();
                    for (int j = 0; j < table.Columns.Count; j++)
                        dr[j] = string.IsNullOrEmpty(reader.GetField(j)) ? DBNull.Value : reader.GetField(table.Columns[j].DataType, j);

                    table.Rows.Add(dr);
                }
        }

        private static DataTable GetBlocksTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("network", typeof(string));
            dt.Columns.Add("geoname_id", typeof(int));
            dt.Columns.Add("registered_country_geoname_id", typeof(int));
            dt.Columns.Add("represented_country_geoname_id", typeof(int));
            dt.Columns.Add("is_anonymous_proxy", typeof(byte));
            dt.Columns.Add("is_satellite_provider", typeof(byte));
            dt.Columns.Add("postal_code", typeof(string));
            dt.Columns.Add("latitude", typeof(string));
            dt.Columns.Add("longitude", typeof(string));
            dt.Columns.Add("accuracy_radius", typeof(int));
            return dt;
        }

        private static DataTable GetLocationsTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("geoname_id", typeof(uint));
            dt.Columns.Add("locale_code", typeof(string));
            dt.Columns.Add("continent_code", typeof(string));
            dt.Columns.Add("continent_name", typeof(string));
            dt.Columns.Add("country_iso_code", typeof(string));
            dt.Columns.Add("country_name", typeof(string));
            dt.Columns.Add("subdivision_1_iso_code", typeof(string));
            dt.Columns.Add("subdivision_1_name", typeof(string));
            dt.Columns.Add("subdivision_2_iso_code", typeof(string));
            dt.Columns.Add("subdivision_2_name", typeof(string));
            dt.Columns.Add("city_name", typeof(string));
            dt.Columns.Add("metro_code", typeof(string));
            dt.Columns.Add("time_zone", typeof(string));

            return dt;
        }
                           
    

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {

            using (SqlConnection targetConnection = new SqlConnection(request.DataStore.GetValue("SqlConnectionString")))
            {
                targetConnection.Open();
                WebRequest zipFileRequest = WebRequest.Create(GEOLITE_CITY);
                using (HttpWebResponse response = (HttpWebResponse)zipFileRequest.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        return new ActionResponse(ActionStatus.Failure);

                    using (Stream zipStream = response.GetResponseStream())
                    {
                        ZipArchive zipFile = new ZipArchive(zipStream, ZipArchiveMode.Read);
                        ZipArchiveEntry blocksIPv4 = null;
                        for (int i = 0; i < zipFile.Entries.Count; i++)
                        {
                            if (zipFile.Entries[i].Name.EndsWith("GeoLite2-City-Blocks-IPv4.csv"))
                            {
                                blocksIPv4 = zipFile.Entries[i];
                                break;
                            }
                        }

                        // TODO: move out to their own action
                        using (SqlCommand cmd = new SqlCommand("TRUNCATE TABLE pbist_apimgmt.[GeoLite2-City-Blocks-IPv4];" +
                                                                "ALTER INDEX [IX_GeoLite2-City-Blocks-IPv4] ON pbist_apimgmt.[GeoLite2-City-Blocks-IPv4] DISABLE;" +
                                                                "ALTER INDEX [IX_GeoLite2-City-Blocks-IPv4_IPPart] ON pbist_apimgmt.[GeoLite2-City-Blocks-IPv4] DISABLE;", targetConnection) { CommandTimeout = 0 })
                        {
                            cmd.ExecuteNonQuery();
                        }

                        SqlBulkCopy bulkCopy = new SqlBulkCopy(targetConnection) { BulkCopyTimeout = 0 };
                        using (StreamReader sr = new StreamReader(blocksIPv4.Open()))
                        {
                            DataTable blocksTable = GetBlocksTable();
                            CsvReader csv = new CsvReader(sr);
                            csv.Configuration.DetectColumnCountChanges = true;
                            csv.Configuration.HasHeaderRecord = true;

                            bulkCopy.DestinationTableName = "pbist_apimgmt.[GeoLite2-City-Blocks-IPv4]";
                            do
                            {
                                LoadDataTable(csv, blocksTable, BATCH_SIZE);
                                bulkCopy.WriteToServer(blocksTable);
                            } while (!sr.EndOfStream);
                        }
                        bulkCopy.Close();

                        // TODO: move out to their own action
                        using (SqlCommand cmd = new SqlCommand("UPDATE pbist_apimgmt.[GeoLite2-City-Blocks-IPv4] SET IPpart=LEFT([network], CHARINDEX('.', [network]) - 1)", targetConnection) { CommandTimeout = 0 })
                        {
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = "ALTER INDEX [IX_GeoLite2-City-Blocks-IPv4] ON pbist_apimgmt.[GeoLite2-City-Blocks-IPv4] REBUILD";
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = "ALTER INDEX [IX_GeoLite2-City-Blocks-IPv4_IPPart] ON pbist_apimgmt.[GeoLite2-City-Blocks-IPv4] REBUILD";
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }

            return new ActionResponse(ActionStatus.Success);
        }
    }
}