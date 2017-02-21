using System.Linq;
using System.Web.Http;
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

            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);
            //config.Services.Add(typeof(IExceptionLogger), new AiExceptionLogger());
        }
    }
}