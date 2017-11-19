using System.Web.Mvc;
using System.Web.Routing;


namespace TestingModule
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();

            //lectors
            routes.MapRoute(
                name: "DeleteLector",
                url: "admin/lectors/{LectorId}/Delete",
                defaults: new { controller = "admin", action = "DeleteLector" });

            //students
            routes.MapRoute(
                name: "students",
                url: "admin/specialities/{SpecialityId}/groups/{GroupId}/students",
                defaults: new { controller = "admin", action = "students" });
            routes.MapRoute(
                name: "DownloadStudentExcel",
                url: "admin/specialities/{SpecialityId}/groups/{GroupId}/students/download",
                defaults: new { controller = "admin", action = "DownloadStudentExcel" });
            routes.MapRoute(
                name: "UploadStudentExcel",
                url: "admin/specialities/{SpecialityId}/groups/{GroupId}/students/upload",
                defaults: new { controller = "admin", action = "UploadStudentExcel" });
            routes.MapRoute(
                name: "AddStudent",
                url: "admin/specialities/{SpecialityId}/groups/{GroupId}/students/NewStudent",
                defaults: new { controller = "admin", action = "NewStudent" });
            routes.MapRoute(
                name: "EditStudent",
                url: "admin/specialities/{SpecialityId}/groups/{GroupId}/students/EditStudent",
                defaults: new { controller = "admin", action = "EditStudent" });
            routes.MapRoute(
                name: "DeleteStudent",
                url: "admin/specialities/{SpecialityId}/groups/{GroupId}/students/{StudentId}/Delete",
                defaults: new { controller = "admin", action = "DeleteStudent" });

            //groups
            routes.MapRoute(
                name: "AddGroup",
                url: "admin/specialities/{SpecialityId}/groups/NewGroup",
                defaults: new { controller = "admin", action = "NewGroup" });
            routes.MapRoute(
                name: "EditGroup",
                url: "admin/specialities/{SpecialityId}/groups/EditGroup",
                defaults: new { controller = "admin", action = "EditGroup" });
            routes.MapRoute(
                name: "DeleteGroup",
                url: "admin/specialities/{SpecialityId}/groups/{GroupId}/Delete",
                defaults: new { controller = "admin", action = "DeleteGroup" });
            routes.MapRoute(
                name: "groups",
                url: "admin/specialities/{SpecialityId}/groups",
                defaults: new { controller = "admin", action = "groups" });

            //Specialities
            routes.MapRoute(
                name: "DeleteSpeciality",
                url: "admin/specialities/{SpecialityId}/Delete",
                defaults: new { controller = "admin", action = "DeleteSpeciality" });

            //questions
            routes.MapRoute(
                name: "questions",
                url: "admin/disciplines/{disciplineid}/lectures/{LectureId}/modules/{ModuleId}/questions",
                defaults: new { controller = "admin", action = "questions" });
            routes.MapRoute(
                name: "AddQuestion",
                url: "admin/disciplines/{disciplineId}/lectures/{LectureId}/modules/{moduleId}/questions/NewQuestion",
                defaults: new { controller = "admin", action = "NewQuestion" });
            routes.MapRoute(
                name: "EditQuestion",
                url: "admin/disciplines/{disciplineId}/lectures/{LectureId}/modules/{moduleId}/questions/EditQuestion",
                defaults: new { controller = "admin", action = "EditQuestion" });
            routes.MapRoute(
                name: "DeleteQuestion",
                url: "admin/disciplines/{disciplineId}/lectures/{LectureId}/modules/{moduleId}/questions/{questionId}/Delete",
                defaults: new { controller = "admin", action = "DeleteQuestion" });
            routes.MapRoute(
                name: "AddAnswer",
                url: "admin/disciplines/{disciplineId}/lectures/{LectureId}/modules/{moduleId}/questions/NewAnswer",
                defaults: new { controller = "admin", action = "NewAnswer" });
            routes.MapRoute(
                name: "EditAnswer",
                url: "admin/disciplines/{disciplineId}/lectures/{LectureId}/modules/{moduleId}/questions/EditAnswer",
                defaults: new { controller = "admin", action = "EditAnswer" });
            routes.MapRoute(
                name: "DeleteAswer",
                url: "admin/disciplines/{disciplineId}/lectures/{LectureId}/modules/{moduleId}/Answers/{answerId}/Delete",
                defaults: new { controller = "admin", action = "DeleteAnswer" });

            //modules
            routes.MapRoute(
                name: "modules",
                url: "admin/disciplines/{disciplineid}/lectures/{LectureId}/modules",
                defaults: new { controller = "admin", action = "modules" });
            routes.MapRoute(
                name: "AddModule",
                url: "admin/disciplines/{disciplineid}/lectures/{LectureId}/modules/NewModule",
                defaults: new { controller = "admin", action = "NewModule" });
            routes.MapRoute(
                name: "EditModule",
                url: "admin/disciplines/{disciplineid}/lectures/{LectureId}/modules/EditModule",
                defaults: new { controller = "admin", action = "EditModule" });
            routes.MapRoute(
                name: "DeleteModule",
                url: "admin/disciplines/{disciplineid}/lectures/{LectureId}/modules/{ModuleId}/Delete",
                defaults: new { controller = "admin", action = "DeleteModule" });

            //disciplinestudents
            routes.MapRoute(
                name: "disciplinestudents",
                url: "admin/disciplines/{disciplineid}/disciplinestudents",
                defaults: new { controller = "admin", action = "disciplinestudents" });
            routes.MapRoute(
                name: "NewStudentConnections",
                url: "admin/disciplines/{disciplineid}/disciplinestudents/NewStudentConnections",
                defaults: new { controller = "admin", action = "NewStudentConnections" });

            //lectures
            routes.MapRoute(
                name: "AddLecture",
                url: "admin/disciplines/{disciplineid}/lectures/NewLecture",
                defaults: new { controller = "admin", action = "NewLecture" });
            routes.MapRoute(
                name: "DeleteLecture",
                url: "admin/disciplines/{disciplineid}/lectures/{LectureId}/Delete",
                defaults: new { controller = "admin", action = "DeleteLecture" });
            routes.MapRoute(
                name: "lectures",
                url: "admin/disciplines/{disciplineid}/lectures",
                defaults: new { controller = "admin", action = "lectures" });
            routes.MapRoute(
                name: "EditLecture",
                url: "admin/disciplines/{disciplineid}/lectures/Edit",
                defaults: new { controller = "admin", action = "EditLecture" });

            //disciplines
            routes.MapRoute(
                name: "Deletediscipline",
                url: "admin/disciplines/{disciplineid}/Delete",
                defaults: new { controller = "admin", action = "Deletediscipline" });

            routes.MapRoute(
                name: "admin",
                url: "admin/{action}",
                defaults: new { controller = "admin", action = "Index" });

            routes.MapRoute(
                name: "NotFound",
                url: "notfound",
                defaults: new { controller = "Error", action = "NotFound" });

            routes.MapRoute(
                name: "ServerError",
                url: "servererror",
                defaults: new { controller = "Error", action = "ServerError" });

            routes.MapRoute(
                name: "Resolved",
                url: "Error/Dashboard/{exeptionId}/Resolved",
                defaults: new { controller = "Error", action = "Resolved" });

            routes.MapRoute(
                name: "Dashboard",
                url: "dashboard",
                defaults: new { controller = "Error", action = "Dashboard" });

            routes.MapRoute(
                name: "StudentIndex",
                url: "Student",
                defaults: new { controller = "Student", action = "Index" });
            routes.MapRoute(
                name: "StudentLectures",
                url: "Student/{disciplineid}/lectures",
                defaults: new { controller = "student", action = "StudentLectures" });
            routes.MapRoute(
                name: "StudentModules",
                url: "Student/{disciplineid}/lectures/{lectureId}/modules",
                defaults: new { controller = "student", action = "StudentModules" });

            routes.MapRoute(
                name: "Login",
                url: "login",
                defaults: new { controller = "Account", action = "Login" });

            routes.MapRoute(
                name: "Logout",
                url: "logout",
                defaults: new { controller = "Account", action = "Logout" });

            routes.MapRoute(
                name: "GetLecturesByDiscipline",
                url: "admin/GetLecturesByDiscipline",
                defaults: new { controller = "admin", action = "GetLecturesByDiscipline" });
            routes.MapRoute(
                name: "StartModule",
                url: "admin/StartModule/{moduleId}",
                defaults: new { controller = "admin", action = "StartModule" });
            routes.MapRoute(
                name: "StopModule",
                url: "admin/StopModule/{moduleId}",
                defaults: new { controller = "admin", action = "StopModule" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional });

            

        }
    }
}
