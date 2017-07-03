using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TestingModule
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //Modules
            routes.MapRoute(
                name: "Modules",
                url: "admin/disciplines/{Disciplineid}/Lectures/{LectureId}/Modules",
                defaults: new { controller = "Admin", action = "Modules" });
            routes.MapRoute(
                name: "AddModule",
                url: "admin/disciplines/{Disciplineid}/Lectures/{LectureId}/Modules/NewLModule",
                defaults: new { controller = "Admin", action = "NewModule" });
            routes.MapRoute(
                name: "DeleteModule",
                url: "admin/disciplines/{Disciplineid}/Lectures/{LectureId}/Modules/{ModuleId}/Delete",
                defaults: new { controller = "Admin", action = "DeleteModule" });

            //Lectures
            routes.MapRoute(
                name: "AddLecture",
                url: "admin/disciplines/{Disciplineid}/Lectures/NewLecture",
                defaults: new { controller = "Admin", action = "NewLecture" });
            routes.MapRoute(
                name: "DeleteLecture",
                url: "admin/disciplines/{Disciplineid}/Lectures/{LectureId}/Delete",
                defaults: new { controller = "Admin", action = "DeleteLecture" });
            routes.MapRoute(
                name: "Lectures",
                url: "admin/disciplines/{Disciplineid}/Lectures",
                defaults: new { controller = "Admin", action = "Lectures" });

            //Disciplines
            routes.MapRoute(
                name: "DeleteDiscipline",
                url: "admin/disciplines/{Disciplineid}/Delete",
                defaults: new { controller = "Admin", action = "DeleteDiscipline" });

            
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Admin", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
