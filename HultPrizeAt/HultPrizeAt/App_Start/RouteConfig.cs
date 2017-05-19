using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HultPrizeAt
{
  public class RouteConfig
  {
    public static void RegisterRoutes(RouteCollection routes)
    {
      //routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

      /*routes.MapRoute(
          name: "Default",
          url: "{controller}/{action}/{id}",
          defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
      );*/


      routes.MapRoute("Meta", "Meta", new { controller = "Meta", action = "Index" });

      routes.MapRoute("Form", "SchoolForm", new { controller = "SchoolForm", action = "Index" });

      routes.MapRoute("Index", "Error", new { controller = "Error", action = "Index" });
      routes.MapRoute("Process", "Home/Process", new { controller = "Home", action = "Process" });


      //routes.MapRoute("Handle", "Home/Handle/{id}", new { controller = "Home", action = "Handle", id = "" });
      routes.MapRoute("User", "{*id}", new { controller = "Home", action = "Index", id = "" });
      routes.MapRoute("Default", "{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional });

    }
  }
}