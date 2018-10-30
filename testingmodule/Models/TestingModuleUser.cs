using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;

namespace TestingModule.Models
{
    public class TestingModuleUser : IUser<int>
    {
        internal const string RoleClaimType = ClaimTypes.Role;
        internal const string NameClaimType = ClaimTypes.Name;
        internal const string SurnameClaimType = ClaimTypes.Surname;
        internal const string UserNameClaimType = ClaimTypes.NameIdentifier;

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<TestingModuleUser, int> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            ClaimsIdentity userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            userIdentity.AddClaim(new Claim(RoleClaimType, Role));
            //userIdentity.AddClaim(new Claim(UserNameClaimType, UserName));
            if (Name != null)
                userIdentity.AddClaim(new Claim(NameClaimType, Name));
            if (Surname != null)
                userIdentity.AddClaim(new Claim(SurnameClaimType, Surname));
            userIdentity.AddClaim(new Claim(UserNameClaimType, UserName));
            return userIdentity;
        }

        public int Id { get; internal set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}