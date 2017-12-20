using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Recipe.Core.Helper
{
    public class VersionInterceptor : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string appVersionHeader = HttpContext.Current.Request.Headers["AppVersion"];

            if (!string.IsNullOrEmpty(appVersionHeader))
            {
                string currentRequestOSVersion = PlatformHelper.GetUserPlatform(HttpContext.Current.Request);

                string appVersion = System.Configuration.ConfigurationManager.AppSettings["AppVersion"];
                string specificAppVersion = System.Configuration.ConfigurationManager.AppSettings[currentRequestOSVersion + "AppVersion"];

                if (string.IsNullOrEmpty(appVersion) ||
                    (!string.IsNullOrEmpty(appVersionHeader) && (specificAppVersion != appVersionHeader || appVersion != appVersionHeader)))
                {
                    if (string.IsNullOrEmpty(appVersion))
                    {
                        this.ThrowAPIException(System.Net.HttpStatusCode.InternalServerError, "App version is not defined in web config");
                    }
                    else if (specificAppVersion != appVersionHeader)
                    {
                        this.ThrowAPIException(System.Net.HttpStatusCode.UpgradeRequired, currentRequestOSVersion + " App version is not matched.");
                    }
                    else if (appVersion != appVersionHeader)
                    {
                        this.ThrowAPIException(System.Net.HttpStatusCode.UpgradeRequired, "App version is not matched.");
                    }
                }
            }

            return base.SendAsync(request, cancellationToken);
        }

        #region Private Functions
        public void ThrowAPIException(HttpStatusCode code, string errorMessage)
        {
            var resp = new HttpResponseMessage(code);

            var error = Serializer.CreateObject(code, errorMessage, null);

            resp.Content = new ObjectContent(error.GetType(), errorMessage, new JsonMediaTypeFormatter());

            Exception ex = new Exception(errorMessage);

            throw new HttpResponseException(resp);
        }
        #endregion
    }
}
