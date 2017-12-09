using System.ComponentModel.Composition;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.PowerApp
{
    [Export(typeof(IAction))]
    public class WranglePowerApp : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string paFileName = request.DataStore.GetValue("PowerAppFileName");
            string paSqlConnectionId = request.DataStore.GetValue("PowerAppSqlConnectionId");

            string paOriginal = request.Info.App.AppFilePath + $"/service/PowerApp/{paFileName}";
            string paWrangledDirectoryName = Path.GetRandomFileName();

            string paWrangledDirectoryPath = request.Info.App.AppFilePath + $"/Temp/{paWrangledDirectoryName}";
            string paWrangled = $"{paWrangledDirectoryPath}/{paFileName}";

            Directory.CreateDirectory(paWrangledDirectoryPath);
            File.Copy(paOriginal, paWrangled, true);

            using (FileStream paStream = new FileStream(paWrangled, FileMode.Open))
            {
                using (ZipArchive pa = new ZipArchive(paStream, ZipArchiveMode.Update))
                {
                    ZipArchiveEntry paProperties = pa.CreateEntry(PowerAppUtility.POWERAPP_PROPERTIES);
                    using (StreamWriter paWriter = new StreamWriter(paProperties.Open()))
                    {
                        paWriter.Write($"{{\"Author\":\"\",\"Name\":\"TwitterTemplate.msapp\",\"Id\":\"a8b23524-032c-43b3-a6fc-07b85188ae47\",\"FileID\":\"a43aa036-e3ab-48e5-a5b4-038565be3ea9\",\"LocalConnectionReferences\":\"{{\\\"cbe235ee-eb8b-5329-1c73-812825f07146\\\":{{\\\"connectionRef\\\":{{\\\"displayName\\\":\\\"SQL Server\\\",\\\"iconUri\\\":\\\"https://az818438.vo.msecnd.net/icons/sql.png\\\",\\\"id\\\":\\\"/providers/microsoft.powerapps/apis/shared_sql\\\",\\\"sharedConnectionId\\\":\\\"/providers/microsoft.powerapps/apis/shared_sql/connections/{paSqlConnectionId}\\\",\\\"isOnPremiseConnection\\\":false,\\\"bypassConsent\\\":false,\\\"parameterHints\\\":{{}},\\\"apiTier\\\":\\\"Standard\\\"}},\\\"dataSources\\\":[\\\"[pbist_twitter].[twitter_query]\\\",\\\"[pbist_twitter].[twitter_query_details]\\\",\\\"[pbist_twitter].[twitter_query_readable]\\\"],\\\"dependencies\\\":{{}},\\\"id\\\":\\\"cbe235ee-eb8b-5329-1c73-812825f07146\\\",\\\"connectionInstanceId\\\":\\\"/providers/microsoft.powerapps/apis/shared_sql/connections/{paSqlConnectionId}\\\"}}}}\",\"DocumentLayoutWidth\":1366,\"DocumentLayoutHeight\":768,\"DocumentLayoutOrientation\":\"landscape\",\"DocumentLayoutMaintainAspectRatio\":false,\"DocumentLayoutLockOrientation\":true,\"OriginatingVersion\":\"1.193\",\"DocumentAppType\":\"DesktopOrTablet\",\"AppCreationSource\":\"AppFromScratch\",\"AppDescription\":\"\",\"LastControlUniqueId\":164}}");
                    }
                }
            }

            string paUrl = request.Info.ServiceRootUrl + request.Info.ServiceRelativePath + request.Info.App.AppRelativeFilePath + $"/Temp/{paWrangledDirectoryName}/{paFileName}";

            return new ActionResponse(ActionStatus.Success, paUrl);
        }
    }
}