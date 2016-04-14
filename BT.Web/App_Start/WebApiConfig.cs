using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using BT.Web.ApiControllers.Base;
using Utils;

namespace BT.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();
            config.EnableCors();
            config.Filters.Add(new ExceptionFilter());

            bool showTestData;
            if (Boolean.TryParse(ConfigTools.GetAppStr("ShowTestData"), out showTestData)==false)
            {
                showTestData = false;
            }
            if (showTestData)
            {
             config.Routes.MapHttpRoute(
                    name: "DefaultApi",
                    routeTemplate: "api/{controller}/{id}",
                    defaults: new { id = RouteParameter.Optional },
                    namespaces: new string[]{"BT.Web.ApiControllers"}
                    );
            }
            else
            {
                config.Routes.MapHttpRoute(
                    name: "DefaultApi",
                    routeTemplate: "api/{controller}/{id}",
                    defaults: new { id = RouteParameter.Optional },
                      namespaces: new string[] { "BT.Web.ApiControllers.Test" }
                    );
            }
       
        }
     
    }
    /// <summary>
    /// 重载MapHttpRoute 扩展多2个方法支持namespace
    /// </summary>
    public static class HttpRouteCollectionEx
    {
        public static IHttpRoute MapHttpRoute(this HttpRouteCollection routes, string name, string routeTemplate, object defaults, string[] namespaces)
        {
            return routes.MapHttpRoute(name, routeTemplate, defaults, null, null, namespaces);
        }
        public static IHttpRoute MapHttpRoute(this HttpRouteCollection routes, string name, string routeTemplate, object defaults, object constraints, HttpMessageHandler handler, string[] namespaces)
        {
            if (routes == null)
            {
                throw new ArgumentNullException("routes");
            }
            var routeValue = new HttpRouteValueDictionary(new { Namespace = namespaces });//设置路由值
            var route = routes.CreateRoute(routeTemplate, new HttpRouteValueDictionary(defaults), new HttpRouteValueDictionary(constraints), routeValue, handler);
            routes.Add(name, route);
            return route;
        }
    }
}
