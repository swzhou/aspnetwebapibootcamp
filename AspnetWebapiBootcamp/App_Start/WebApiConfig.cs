using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using AspnetWebapiBootcamp.Handlers;

namespace AspnetWebapiBootcamp
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.MessageHandlers.Add(new MethodOverrideHandler());
            config.MessageHandlers.Add(new AngularJsonVulnerabilityProtectionHandler());

            // Web API routes
            config.MapHttpAttributeRoutes();
            
            var handlers = new DelegatingHandler[]
            {
                new ApiKeyHandler("secure") 
            };
            var routeHandlers = HttpClientFactory.CreatePipeline(new HttpControllerDispatcher(config), handlers);
            config.Routes.MapHttpRoute(
                name: "SecureAPI",
                routeTemplate: "secure/api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional },
                constraints: null,
                handler: routeHandlers
                );
            
            config.Routes.MapHttpRoute(
                name: "ActionApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );

        }
    }
}