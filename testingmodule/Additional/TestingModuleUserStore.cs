using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using TestingModule.Models;

namespace TestingModule.Additional
{
    public class TestingModuleUserStore : IUserStore<TestingModuleUser, int>
    {
        public testingDbEntities Context { get; }

        public TestingModuleUserStore()
        {
            Context = new testingDbEntities();
        }

        public void Dispose()
        {
            Context.Dispose();
        }

        public Task CreateAsync(TestingModuleUser user)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(TestingModuleUser user)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(TestingModuleUser user)
        {
            throw new NotImplementedException();
        }

        public async Task<TestingModuleUser> FindByIdAsync(int userId)
        {
            var roles = await Context.Roles.ToListAsync();
            var account = await Context.Accounts.SingleOrDefaultAsync(a => a.Id == userId);
            if (account == null) return null;

            TestingModuleUser user = new TestingModuleUser
            {
                UserName = account.Login,
                Role = roles.Where(r => r.Id == account.RoleId).Select(r => r.Name).SingleOrDefault()
            };
            if (user.Role == RoleName.Student)
            {
                Student student = await Context.Students.SingleOrDefaultAsync(s => s.AccountId == account.Id);
                if (student == null) return null;
                user.Id = student.Id;
                user.UserName = student.Name;
                user.Surname = student.Surname;
            }
            else
                user.Id = account.Id;
            return user;
        }

        public async Task<TestingModuleUser> FindByNameAsync(string userName)
        {
            var roles = await Context.Roles.ToListAsync();
            var account = await Context.Accounts.SingleOrDefaultAsync(a => a.Login == userName);
            if (account == null) return null;

            TestingModuleUser user = new TestingModuleUser
            {
                UserName = account.Login,
                Role = roles.Where(r => r.Id == account.RoleId).Select(r => r.Name).SingleOrDefault()
            };
            if (user.Role == RoleName.Student)
            {
                Student student = await Context.Students.SingleOrDefaultAsync(s => s.AccountId == account.Id);
                if (student == null) return null;
                user.Id = student.Id;
                user.UserName = student.Name;
                user.Surname = student.Surname;
            }
            else if (user.Role == RoleName.Lecturer)
            {
                Lector lector = await Context.Lectors.SingleOrDefaultAsync(s => s.AccountId == account.Id);
                if (lector == null) return null;
                user.Id = lector.Id;
                user.UserName = lector.Name;
                user.Surname = lector.Surname;
            }
            else
                user.Id = account.Id;
            return user;
        }
    }
}