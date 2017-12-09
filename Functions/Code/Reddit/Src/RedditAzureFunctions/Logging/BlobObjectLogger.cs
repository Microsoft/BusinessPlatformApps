using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using RedditCore;
using RedditCore.Logging;
using System;
using System.IO;
using System.Text;

namespace RedditAzureFunctions.Logging
{
    internal class BlobObjectLogger : IObjectLogger
    {
        private readonly IConfiguration configuration;
        private readonly ILog logger;

        internal BlobObjectLogger(IConfiguration configuration, ILog logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        public void Log(object data, string source, string message = null)
        {
            try
            {
                string dataString = (data != null) ? data as string : "null";
                if (dataString == null)
                {
                    dataString = JsonConvert.SerializeObject(data);
                }

                var sasUri = new Uri(this.configuration.ObjectLogSASUrl);
                var accountName = sasUri.Host.TrimEndString(".blob.core.windows.net");
                StorageCredentials creds = new StorageCredentials(sasUri.Query);
                CloudStorageAccount strAcc = new CloudStorageAccount(creds, accountName, endpointSuffix: null, useHttps: true);
                CloudBlobClient blobClient = strAcc.CreateCloudBlobClient();

                //Setup our container we are going to use and create it.
                CloudBlobContainer container = blobClient.GetContainerReference(configuration.ObjectLogBlobContainer);
                container.CreateIfNotExistsAsync();

                // Build my typical log file name.
                DateTime date = DateTime.Now;

                // This creates a reference to the append blob we are going to use.
                CloudAppendBlob appBlob = container.GetAppendBlobReference(
                    $"{date.ToString("yyyy-MM")}/{date.ToString("dd")}/{date.ToString("HH")}-{source}.log");

                // Now we are going to check if todays file exists and if it doesn't we create it.
                if (!appBlob.Exists())
                {
                    appBlob.CreateOrReplace();
                }

                // Add the entry to our log.
                var logMessage = $"{date.ToString("o")}|{message}|{dataString}{Environment.NewLine}";
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(logMessage)))
                {
                    // Append a block.  AppendText is not safe across multiple threads & servers so use AppendBlock instead.
                    appBlob.AppendBlock(ms);
                }
            }
            catch (Exception ex)
            {
                this.logger.Error("Error logging data to the permanent store", ex);
            }
        }
    }
}
