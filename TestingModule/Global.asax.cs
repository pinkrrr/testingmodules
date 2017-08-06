using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace TestingModule
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exept = Server.GetLastError();
            HttpException httpEx = exept as HttpException;
            if (httpEx.GetHttpCode() == 404)
            {
                Server.ClearError();
                Response.Redirect("/Error/NotFound");
            }
            else if (httpEx.GetHttpCode() == 500)
            {
                HttpContext con = HttpContext.Current;
                var url = con.Request.Url.ToString();
                new Additional.Adding().AddNewError(url);
                Server.ClearError();
                Response.Redirect("/Error/ServerError");
            }
            
        }
    }
}
