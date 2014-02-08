using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NerdDinner
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute( "UpcomingDinners", "Dinners/Page/{page}", new { controller = "Dinners", action = "Index" } );
            
            routes.MapRoute( 
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with params
                new { controller = "Home", action = "Index", id = "" } // Param defaults
            );
        }
    }
}