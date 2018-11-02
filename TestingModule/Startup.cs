using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;
using TestingModule.Additional;
using TestingModule.Models;

[assembly: OwinStartup(typeof(TestingModule.Startup))]

namespace TestingModule
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            /*app.CreatePerOwinContext(() => new TestingModuleUserManager(new TestingModuleUserStore()));
            app.CreatePerOwinContext<TestingModuleSignInManager>((options, context) =>
                new TestingModuleSignInManager(context.GetUserManager<TestingModuleUserManager>(),
                    context.Authentication));*/

            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login/"),
                ReturnUrlParameter = "returnUrl",
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<TestingModuleUserManager, TestingModuleUser, int>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentityCallback: (manager, user) => user.GenerateUserIdentityAsync(manager),
                        getUserIdCallback: id => Convert.ToInt32(id.GetUserId()))
                }
            });
            var hubConfiguration = new HubConfiguration { EnableDetailedErrors = true };
            app.MapSignalR(hubConfiguration);
        }
    }
}
