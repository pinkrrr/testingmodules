﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using Autofac;
using Autofac.Integration.Mvc;
using TestingModule.Additional;
using TestingModule.Models;


namespace TestingModule
{
    public class MvcApplication : HttpApplication
    {
        private readonly Adding _adding;
        public MvcApplication()
        {
            var context = new testingDbEntities();
            _adding = new Adding(context);
        }
        protected void Application_Start()
        {
            var builder = new ContainerBuilder();
            var assembly = Assembly.GetExecutingAssembly();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder
                .RegisterAssemblyTypes(assembly)
                .AssignableTo<IDependency>()
                .AsImplementedInterfaces()
                .AsSelf()
                .InstancePerLifetimeScope();
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //await _timerAssociates.OnStartModuleTimer();
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
            else
            {
                HttpContext con = HttpContext.Current;
                var url = con.Request.Url.ToString();
                _adding.AddNewError(url, exept.Message);
                Server.ClearError();
                Response.Redirect("/Error/ServerError");
            }

        }
        //protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        //{
        //    // This is the page
        //    string cTheFile = HttpContext.Current.Request.Path;
        //    bool val1 = (System.Web.HttpContext.Current.User != null) &&
        //                System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
        //    // Check if I am all ready on login page to avoid crash
        //    if (!cTheFile.Contains("/Account/Login") && !cTheFile.Contains("/__browserLink/requestData") && !val1)
        //    {
        //       //  Extract the form's authentication cookie
        //        string cookieName = FormsAuthentication.FormsCookieName;
        //        HttpCookie authCookie = Context.Request.Cookies[cookieName];

        //       //  If not logged in
        //        if (null == authCookie)
        //        {
        //            Response.Redirect("/Account/Login", true);
        //            Response.End();
        //            return;
        //        }
        //    }
        //}

    }
}
