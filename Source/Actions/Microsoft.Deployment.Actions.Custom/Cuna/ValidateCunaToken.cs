using Microsoft.Deployment.Common;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace Microsoft.Deployment.Actions.Custom.Cuna
{
    [Export(typeof(IAction))]
    public class ValidateCunaToken : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            try
            {
                var token = request.DataStore.GetValue("CunaSamlAuthToken");
                token = token?.Trim();

                if (!string.IsNullOrWhiteSpace(token))
                {
                    var decodedToken = DecodeToken(token);
                    if (IsValidToken(decodedToken))
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(decodedToken);
                        XmlElement nameId = doc.SelectSingleNode("//*[local-name()='NameID']") as XmlElement;
                        request.DataStore.AddToDataStore("userId", nameId.InnerText, DataStoreType.Private);
                        XmlElement contract = doc.SelectSingleNode("//*[local-name()='Attribute' and @Name='Contract']/*[local-name()='AttributeValue']") as XmlElement;
                        request.DataStore.AddToDataStore("contractId", contract.InnerText, DataStoreType.Private);
                        return new ActionResponse(ActionStatus.Success);
                    }
                }
            }
            catch(Exception ex)
            {
                return new ActionResponse(ActionStatus.Failure, new ActionResponseExceptionDetail("CunaAuthTokenValidationFailed", ex.Message));
            }
            return new ActionResponse(ActionStatus.Failure,null, null, "CunaAuthTokenValidationFailed");
        }

        private string DecodeToken(string encodedToken)
        {
            var htmlDecoded = HttpUtility.HtmlDecode(encodedToken);
            byte[] data = Convert.FromBase64String(htmlDecoded);
            string decodedToken = Encoding.UTF8.GetString(data);
            return decodedToken;
        }

        private bool IsValidToken(string token)
        {
            XmlDocument doc = new XmlDocument { PreserveWhitespace = true };
            doc.LoadXml(token);

            XmlElement signature = doc.SelectSingleNode("//*[local-name()='Signature']") as XmlElement;
            SignedXml signedXml = new SignedXml(signature.ParentNode as XmlElement);
            signedXml.LoadXml(signature);

            X509Certificate2 cert = new X509Certificate2(GetCert());
            
            return signedXml.CheckSignature(cert, true);
        }

        private byte[] GetCert()
        {
            var certificateString = Constants.CunaTokenValidateCertificate;
            return Encoding.ASCII.GetBytes(certificateString);
        }
    }
}
