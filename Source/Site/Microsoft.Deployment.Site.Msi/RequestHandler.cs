using System;
using System.Security.Cryptography.X509Certificates;
using CefSharp;

namespace Installer
{
    public class RequestHandler : IRequestHandler
    {
        public bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
        {
            return false;
        }

        bool IRequestHandler.OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
        {
            return OnOpenUrlFromTab(browserControl, browser, frame, targetUrl, targetDisposition, userGesture);
        }

        protected virtual bool OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
        {
            return false;
        }

        bool IRequestHandler.OnCertificateError(IWebBrowser browserControl, IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback)
        {
            if (!callback.IsDisposed)
            {
                using (callback)
                {
                    //To allow certificate
                    //callback.Continue(true);
                    //return true;
                }
            }

            return false;
        }

        void IRequestHandler.OnPluginCrashed(IWebBrowser browserControl, IBrowser browser, string pluginPath)
        {
        }

        CefReturnValue IRequestHandler.OnBeforeResourceLoad(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
        {
            // Example of how to set Referer
            // Same should work when setting any header
            try
            {
                if ((request.Url.StartsWith("https://msi/") || request.Url.StartsWith("https://bpsolutiontemplates.com/")) && request.Url != "https://bpsolutiontemplates.com/?name=Microsoft-SCCMTemplate")
                {
                    var appdir = System.Reflection.Assembly.GetEntryAssembly().Location.Replace("Microsoft.Bpst.App.Msi.exe", "");

                    request.Url=  request.Url.Replace("https://msi/", appdir + "\\");
                    request.Url = request.Url.Replace("https://bpsolutiontemplates.com/", appdir + "\\");
                }

                if (request.Url.StartsWith("http://localhost") && !request.Url.Contains("api"))
                {
                    var uriSplit = request.Url.Split('/');
                    var uri = uriSplit[0] + "/" + uriSplit[1] + "/" + uriSplit[2] + "/";
                    request.Url = request.Url.Replace(uri, System.Reflection.Assembly.GetEntryAssembly().Location.Replace("Microsoft.Deployment.Msi.exe", ""));
                }

                if (request.Url.StartsWith("mailto", StringComparison.OrdinalIgnoreCase))
                {
                    return CefReturnValue.Cancel;
                }

                if (browser.IsPopup)
                {
                    System.Diagnostics.Process.Start(request.Url);
                    browser.CloseBrowser(true);
                    return CefReturnValue.Cancel;
                }

                if (!callback.IsDisposed)
                {
                    using (callback)
                    {
                        if (request.Method == "POST")
                        {
                            using (var postData = request.PostData)
                            {
                                if (postData != null)
                                {
                                    var elements = postData.Elements;

                                    var charSet = request.GetCharSet();

                                    foreach (var element in elements)
                                    {
                                        if (element.Type == PostDataElementType.Bytes)
                                        {
                                            var body = element.GetBody(charSet);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {

            }

            return CefReturnValue.Continue;
        }

        bool IRequestHandler.GetAuthCredentials(IWebBrowser browserControl, IBrowser browser, IFrame frame, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
        {
            callback.Dispose();
            return false;
        }

        void IRequestHandler.OnRenderProcessTerminated(IWebBrowser browserControl, IBrowser browser, CefTerminationStatus status)
        {
        }

        bool IRequestHandler.OnQuotaRequest(IWebBrowser browserControl, IBrowser browser, string originUrl, long newSize, IRequestCallback callback)
        {
            if (!callback.IsDisposed)
            {
                using (callback)
                {
                    //Accept Request to raise Quota
                    //callback.Continue(true);
                    //return true;
                }
            }

            return false;
        }

        void OnResourceRedirect(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, ref string newUrl)
        {
            request.Url = newUrl;
            browserControl.Load(newUrl);
        }

        bool IRequestHandler.OnProtocolExecution(IWebBrowser browserControl, IBrowser browser, string url)
        {
            return url.StartsWith("mailto", StringComparison.OrdinalIgnoreCase);
        }

        void IRequestHandler.OnRenderViewReady(IWebBrowser browserControl, IBrowser browser)
        {
        }

        bool IRequestHandler.OnResourceResponse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            return false;
        }

        void IRequestHandler.OnResourceLoadComplete(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
        {
        }

        public bool OnSelectClientCertificate(IWebBrowser browserControl, IBrowser browser, bool isProxy, string host, int port, X509Certificate2Collection certificates, ISelectClientCertificateCallback callback)
        {
            return false;
        }

        public IResponseFilter GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            return null;
        }

        void IRequestHandler.OnResourceRedirect(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response, ref string newUrl)
        {
            request.Url = newUrl;
            browserControl.Load(newUrl);
        }

        public bool CanGetCookies(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request)
        {
            return false;
        }

        public bool CanSetCookie(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, Cookie cookie)
        {
            return false;
        }
    }
}