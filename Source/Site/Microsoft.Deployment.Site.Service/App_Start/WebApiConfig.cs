using System.Configuration;
using System.Linq;
using System.Web.Http;
using Microsoft.Deployment.Common;
using Microsoft.Deployment.Common.AppLoad;
using Microsoft.Deployment.Common.Controller;

namespace Microsoft.Deployment.Site.Service
{
    public static class WebApiConfig
    {
        public static CommonControllerModel CommonControllerModel { get; private set; }

        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.MessageHandlers.Add(new OptionsHttpMessageHandler());

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );

            AppFactory appFactory = new AppFactory(true);

            CommonControllerModel = new CommonControllerModel()
            {
                AppFactory = new AppFactory(),
                AppRootFilePath = appFactory.AppPath,
                SiteCommonFilePath = appFactory.SiteCommonPath,
                ServiceRootFilePath = appFactory.SiteCommonPath + "../",
                Source = "API",
            };

            Constants.BpstDeploymentIdDatabase = ConfigurationManager.ConnectionStrings["BpstDeploymentIdDatabase"].ToString();
            Constants.BpstNotifierUrl = ConfigurationManager.ConnectionStrings["BpstNotifierUrl"].ToString();
            Constants.AxLocatorClientId = ConfigurationManager.ConnectionStrings["AxLocatorClientId"].ToString();
            Constants.AxLocatorSecret = ConfigurationManager.ConnectionStrings["AxLocatorSecret"].ToString();
            Constants.FacebookClientSecret = ConfigurationManager.ConnectionStrings["FacebookSecret"].ToString();
            Constants.InformaticaRegistrationCode = ConfigurationManager.ConnectionStrings["InformaticaRegistrationCode"].ToString();
            Constants.SocialGistProvisionKeyUserName = ConfigurationManager.ConnectionStrings["SocialGistRedditUserName"].ToString();
            Constants.SocialGistProvisionKeyPassphrase = ConfigurationManager.ConnectionStrings["SocialGistRedditPassphrase"].ToString();

            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);
            //config.Services.Add(typeof(IExceptionLogger), new AiExceptionLogger());
        }
    }
}