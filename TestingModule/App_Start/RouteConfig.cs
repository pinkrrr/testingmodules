using System.Web.Mvc;
using System.Web.Routing;

namespace TestingModule
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //lectors
            routes.MapRoute(
                name: "DeleteLector",
                url: "admin/lectors/{LectorId}/Delete",
                defaults: new { controller = "Admin", action = "DeleteLector" });

            //Students
            routes.MapRoute(
                name: "Students",
                url: "admin/specialities/{specialityId}/Groups/{GroupId}/Students",
                defaults: new { controller = "Admin", action = "Students" });
            routes.MapRoute(
                name: "AddStudent",
                url: "admin/specialities/{specialityId}/Groups/{GroupId}/Students/NewStudent",
                defaults: new { controller = "Admin", action = "NewStudent" });
            routes.MapRoute(
                name: "EditStudent",
                url: "admin/specialities/{specialityId}/Groups/{GroupId}/Students/NewStudent",
                defaults: new { controller = "Admin", action = "EditStudent" });
            routes.MapRoute(
                name: "DeleteStudent",
                url: "admin/specialities/{specialityId}/Groups/{GroupId}/Students/{StudentId}/Delete",
                defaults: new { controller = "Admin", action = "DeleteStudent" });

            //Groups
            routes.MapRoute(
                name: "AddGroup",
                url: "admin/specialities/{Specialityid}/Groups/NewGroup",
                defaults: new { controller = "Admin", action = "NewGroup" });
            routes.MapRoute(
                name: "EditGroup",
                url: "admin/specialities/{Specialityid}/Groups/EditGroup",
                defaults: new { controller = "Admin", action = "EditGroup" });
            routes.MapRoute(
                name: "DeleteGroup",
                url: "admin/specialities/{Specialityid}/Groups/{GroupId}/Delete",
                defaults: new { controller = "Admin", action = "DeleteGroup" });
            routes.MapRoute(
                name: "Groups",
                url: "admin/specialities/{Specialityid}/Groups",
                defaults: new { controller = "Admin", action = "Groups" });

            //Specialities
            routes.MapRoute(
                name: "DeleteSpeciality",
                url: "admin/specialities/{Specialityid}/Delete",
                defaults: new { controller = "Admin", action = "DeleteSpeciality" });

            //Questions
            routes.MapRoute(
                name: "Questions",
                url: "admin/disciplines/{Disciplineid}/Lectures/{LectureId}/Modules/{ModuleId}/Questions",
                defaults: new { controller = "Admin", action = "Questions" });
            routes.MapRoute(
                name: "AddQuestion",
                url: "admin/disciplines/{disciplineId}/Lectures/{LectureId}/Modules/{moduleId}/Questions/NewQuestion",
                defaults: new { controller = "Admin", action = "NewQuestion" });
            routes.MapRoute(
                name: "EditQuestion",
                url: "admin/disciplines/{disciplineId}/Lectures/{LectureId}/Modules/{moduleId}/Questions/EditQuestion",
                defaults: new { controller = "Admin", action = "EditQuestion" });
            routes.MapRoute(
                name: "DeleteQuestion",
                url: "admin/disciplines/{disciplineId}/Lectures/{LectureId}/Modules/{moduleId}/Questions/{questionId}/Delete",
                defaults: new { controller = "Admin", action = "DeleteQuestion" });
            routes.MapRoute(
                name: "AddAnswer",
                url: "admin/disciplines/{disciplineId}/Lectures/{LectureId}/Modules/{moduleId}/Questions/NewAnswer",
                defaults: new { controller = "Admin", action = "NewAnswer" });
            routes.MapRoute(
                name: "EditAnswer",
                url: "admin/disciplines/{disciplineId}/Lectures/{LectureId}/Modules/{moduleId}/Questions/EditAnswer",
                defaults: new { controller = "Admin", action = "EditAnswer" });
            routes.MapRoute(
                name: "DeleteAswer",
                url: "admin/disciplines/{disciplineId}/Lectures/{LectureId}/Modules/{moduleId}/Answers/{answerId}/Delete",
                defaults: new { controller = "Admin", action = "DeleteAnswer" });

            //Modules
            routes.MapRoute(
                name: "Modules",
                url: "admin/disciplines/{Disciplineid}/Lectures/{LectureId}/Modules",
                defaults: new { controller = "Admin", action = "Modules" });
            routes.MapRoute(
                name: "AddModule",
                url: "admin/disciplines/{Disciplineid}/Lectures/{LectureId}/Modules/NewModule",
                defaults: new { controller = "Admin", action = "NewModule" });
            routes.MapRoute(
                name: "EditModule",
                url: "admin/disciplines/{Disciplineid}/Lectures/{LectureId}/Modules/EditModule",
                defaults: new { controller = "Admin", action = "EditModule" });
            routes.MapRoute(
                name: "DeleteModule",
                url: "admin/disciplines/{Disciplineid}/Lectures/{LectureId}/Modules/{ModuleId}/Delete",
                defaults: new { controller = "Admin", action = "DeleteModule" });

            //DisciplineStudents
            routes.MapRoute(
                name: "DisciplineStudents",
                url: "admin/disciplines/{Disciplineid}/DisciplineStudents",
                defaults: new { controller = "Admin", action = "DisciplineStudents" });

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
            routes.MapRoute(
                name: "EditLecture",
                url: "admin/disciplines/{Disciplineid}/Lectures/Edit",
                defaults: new { controller = "Admin", action = "EditLecture" });

            //Disciplines
            routes.MapRoute(
                name: "DeleteDiscipline",
                url: "admin/disciplines/{Disciplineid}/Delete",
                defaults: new { controller = "Admin", action = "DeleteDiscipline" });

            
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Admin", action = "Index", id = UrlParameter.Optional });

            routes.MapMvcAttributeRoutes();
        }
    }
}
