using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.ModelBinding;
using System.Web.Http.ModelBinding.Binders;
using System.Web.Http.ValueProviders;
using AspnetWebapiBootcamp.Controllers;
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

            var provider = new SimpleModelBinderProvider(typeof(GeoPoint), new GeoPointModelBinder());
            config.Services.Insert(typeof(ModelBinderProvider), 0, provider);
            config.Services.Add(typeof(ValueProviderFactory), new HeaderValueProviderFactory());
            
//            config.ParameterBindingRules.Add(p =>
//            {
//                if (p.ParameterType == typeof(ETag) &&
//                    p.ActionDescriptor.SupportedHttpMethods.Contains(HttpMethod.Get))
//                {
//                    return new ETagParameterBinding(p, ETagMatch.IfNoneMatch);
//                }
//                return null;
//            });
        }
    }
}