using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebApi
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            string[] NS;
            NS = "WebAPI.Controllers.Page".Split('|');
            routes.MapRoute(
              "Page", // 路由名称  
              "Page/{controller}/{action}/{id}", // 带有参数的 URL  
              new { controller = "Page", action = "Index", id = UrlParameter.Optional }, NS
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}

