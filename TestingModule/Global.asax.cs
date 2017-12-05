using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using TestingModule.Additional;
using TestingModule.Models;


namespace TestingModule
{
    public class MvcApplication : HttpApplication
    {
        protected async void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            await new TimerAssociates().OnStartModuleTimer();
        }

        /*protected void Application_Error(object sender, EventArgs e)
        {
            Exception exept = Server.GetLastError();
            HttpException httpEx = exept as HttpException;
            if (httpEx.GetHttpCode() == 404)
            {
                Server.ClearError();
                Response.Redirect("/Error/NotFound");
            }
            else
            {
                HttpContext con = HttpContext.Current;
                var url = con.Request.Url.ToString();
                new Additional.Adding().AddNewError(url, exept.Message);
                Server.ClearError();
                Response.Redirect("/Error/ServerError");
            }

        }*/
        /*
        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            // This is the page
            string cTheFile = HttpContext.Current.Request.Path;
            bool val1 = (System.Web.HttpContext.Current.User != null) &&
                        System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            // Check if I am all ready on login page to avoid crash
            if (!cTheFile.Contains("/Account/Login") && !cTheFile.Contains("/__browserLink/requestData") && !val1)
            {
                // Extract the form's authentication cookie
                string cookieName = FormsAuthentication.FormsCookieName;
                HttpCookie authCookie = Context.Request.Cookies[cookieName];

                // If not logged in
                if (null == authCookie)
                {
                    Response.Redirect("/Account/Login", true);
                    Response.End();
                    return;
                }
            }
        }
        */
    }
}
