using Recipe.Core;
using System.Net.Http.Headers;
using System.Web.Http;
using Unity.WebApi;

namespace Test.API
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(Register);
        }

        static void Register(HttpConfiguration config)
        {
            config.DependencyResolver = new UnityDependencyResolver(IoC.Container);
            config.MapHttpAttributeRoutes();
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
        }
    }
}
