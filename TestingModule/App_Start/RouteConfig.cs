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

            //students
            routes.MapRoute(
                name: "students",
                url: "admin/specialities/{specialityId}/groups/{GroupId}/students",
                defaults: new { controller = "Admin", action = "students" });
            routes.MapRoute(
                name: "DownloadStudentExcel",
                url: "admin/specialities/{specialityId}/groups/{GroupId}/students/download",
                defaults: new { controller = "Admin", action = "DownloadStudentExcel" });
            routes.MapRoute(
                name: "UploadStudentExcel",
                url: "admin/specialities/{specialityId}/groups/{GroupId}/students/upload",
                defaults: new { controller = "Admin", action = "UploadStudentExcel" });
            routes.MapRoute(
                name: "AddStudent",
                url: "admin/specialities/{specialityId}/groups/{GroupId}/students/NewStudent",
                defaults: new { controller = "Admin", action = "NewStudent" });
            routes.MapRoute(
                name: "EditStudent",
                url: "admin/specialities/{specialityId}/groups/{GroupId}/students/NewStudent",
                defaults: new { controller = "Admin", action = "EditStudent" });
            routes.MapRoute(
                name: "DeleteStudent",
                url: "admin/specialities/{specialityId}/groups/{GroupId}/students/{StudentId}/Delete",
                defaults: new { controller = "Admin", action = "DeleteStudent" });

            //groups
            routes.MapRoute(
                name: "AddGroup",
                url: "admin/specialities/{Specialityid}/groups/NewGroup",
                defaults: new { controller = "Admin", action = "NewGroup" });
            routes.MapRoute(
                name: "EditGroup",
                url: "admin/specialities/{Specialityid}/groups/EditGroup",
                defaults: new { controller = "Admin", action = "EditGroup" });
            routes.MapRoute(
                name: "DeleteGroup",
                url: "admin/specialities/{Specialityid}/groups/{GroupId}/Delete",
                defaults: new { controller = "Admin", action = "DeleteGroup" });
            routes.MapRoute(
                name: "groups",
                url: "admin/specialities/{Specialityid}/groups",
                defaults: new { controller = "Admin", action = "groups" });

            //Specialities
            routes.MapRoute(
                name: "DeleteSpeciality",
                url: "admin/specialities/{Specialityid}/Delete",
                defaults: new { controller = "Admin", action = "DeleteSpeciality" });

            //questions
            routes.MapRoute(
                name: "questions",
                url: "admin/disciplines/{disciplineid}/lectures/{LectureId}/modules/{ModuleId}/questions",
                defaults: new { controller = "Admin", action = "questions" });
            routes.MapRoute(
                name: "AddQuestion",
                url: "admin/disciplines/{disciplineId}/lectures/{LectureId}/modules/{moduleId}/questions/NewQuestion",
                defaults: new { controller = "Admin", action = "NewQuestion" });
            routes.MapRoute(
                name: "EditQuestion",
                url: "admin/disciplines/{disciplineId}/lectures/{LectureId}/modules/{moduleId}/questions/EditQuestion",
                defaults: new { controller = "Admin", action = "EditQuestion" });
            routes.MapRoute(
                name: "DeleteQuestion",
                url: "admin/disciplines/{disciplineId}/lectures/{LectureId}/modules/{moduleId}/questions/{questionId}/Delete",
                defaults: new { controller = "Admin", action = "DeleteQuestion" });
            routes.MapRoute(
                name: "AddAnswer",
                url: "admin/disciplines/{disciplineId}/lectures/{LectureId}/modules/{moduleId}/questions/NewAnswer",
                defaults: new { controller = "Admin", action = "NewAnswer" });
            routes.MapRoute(
                name: "EditAnswer",
                url: "admin/disciplines/{disciplineId}/lectures/{LectureId}/modules/{moduleId}/questions/EditAnswer",
                defaults: new { controller = "Admin", action = "EditAnswer" });
            routes.MapRoute(
                name: "DeleteAswer",
                url: "admin/disciplines/{disciplineId}/lectures/{LectureId}/modules/{moduleId}/Answers/{answerId}/Delete",
                defaults: new { controller = "Admin", action = "DeleteAnswer" });

            //modules
            routes.MapRoute(
                name: "modules",
                url: "admin/disciplines/{disciplineid}/lectures/{LectureId}/modules",
                defaults: new { controller = "Admin", action = "modules" });
            routes.MapRoute(
                name: "AddModule",
                url: "admin/disciplines/{disciplineid}/lectures/{LectureId}/modules/NewModule",
                defaults: new { controller = "Admin", action = "NewModule" });
            routes.MapRoute(
                name: "EditModule",
                url: "admin/disciplines/{disciplineid}/lectures/{LectureId}/modules/EditModule",
                defaults: new { controller = "Admin", action = "EditModule" });
            routes.MapRoute(
                name: "DeleteModule",
                url: "admin/disciplines/{disciplineid}/lectures/{LectureId}/modules/{ModuleId}/Delete",
                defaults: new { controller = "Admin", action = "DeleteModule" });

            //disciplinestudents
            routes.MapRoute(
                name: "disciplinestudents",
                url: "admin/disciplines/{disciplineid}/disciplinestudents",
                defaults: new { controller = "Admin", action = "disciplinestudents" });
            routes.MapRoute(
                name: "NewStudentConnections",
                url: "admin/disciplines/{disciplineid}/disciplinestudents/NewStudentConnections",
                defaults: new { controller = "Admin", action = "NewStudentConnections" });

            //lectures
            routes.MapRoute(
                name: "AddLecture",
                url: "admin/disciplines/{disciplineid}/lectures/NewLecture",
                defaults: new { controller = "Admin", action = "NewLecture" });
            routes.MapRoute(
                name: "DeleteLecture",
                url: "admin/disciplines/{disciplineid}/lectures/{LectureId}/Delete",
                defaults: new { controller = "Admin", action = "DeleteLecture" });
            routes.MapRoute(
                name: "lectures",
                url: "admin/disciplines/{disciplineid}/lectures",
                defaults: new { controller = "Admin", action = "lectures" });
            routes.MapRoute(
                name: "EditLecture",
                url: "admin/disciplines/{disciplineid}/lectures/Edit",
                defaults: new { controller = "Admin", action = "EditLecture" });

            //disciplines
            routes.MapRoute(
                name: "Deletediscipline",
                url: "admin/disciplines/{disciplineid}/Delete",
                defaults: new { controller = "Admin", action = "Deletediscipline" });          

            routes.MapRoute(
                name: "Admin",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Admin", action = "Index", id = UrlParameter.Optional });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}",
                defaults: new { controller = "Quiz", action = "Index" });

        }
    }
}
