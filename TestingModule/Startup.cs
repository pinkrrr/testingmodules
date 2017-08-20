using System;
using System.Threading.Tasks;
using System.Web.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(TestingModule.Startup))]

namespace TestingModule
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login/")
            });
            //AntiForgeryConfig.SuppressXFrameOptionsHeader = true;
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
        }
    }
}
