using Microsoft.EntityFrameworkCore;
using MovieApp.Models;
using MovieApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MovieApp.Test
{
    public class AdminTest
    {
        private JAMDContext _context;

        string str1 = "3009F8A3-C9E3-449B-AC44-24C18B3920D4";
        string str2 = "8A0CB704-DEB6-4323-8EDA-34D752EB3B79";

        AdminRepository repo;
        JAMDContext context;

        public AdminTest()
        {
            var builder = new DbContextOptionsBuilder<JAMDContext>()
               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());

            context = new JAMDContext(builder.Options);

            var admins = Enumerable.Range(1, 9)
                .Select(i => new TblAdmin { AdminId = Guid.Parse(i.ToString() + str1.Substring(1)), UserId = Guid.Parse(i.ToString() + str2.Substring(1)), User = new TblUser { UserId = Guid.Parse(i.ToString() + str2.Substring(1)) } });
            context.TblAdmin.AddRange(admins);

            int changed = context.SaveChanges();
            _context = context;
            repo = new AdminRepository(_context);
        }

        [Fact]
        public async Task TestGetSingleAdmin()
        {
            string expected = "2A0CB704-DEB6-4323-8EDA-34D752EB3B79";
            string adminToTest = "2009F8A3-C9E3-449B-AC44-24C18B3920D4";
            var result = await repo.GetSingle(Guid.Parse(adminToTest));
            Assert.Equal(expected, result.UserId.ToString().ToUpper());
        }

        [Fact]
        public async Task TestGetAllAdmins()
        {
            string expected = "2A0CB704-DEB6-4323-8EDA-34D752EB3B79";
            var result = await repo.GetAll();
            Assert.Equal(expected, result.ElementAt(1).UserId.ToString().ToUpper());
        }

        [Fact]
        public async Task TestAddAdmin()
        {
            var admin = new TblAdmin();
            admin.AdminId = Guid.NewGuid();
            admin.UserId = Guid.NewGuid();
            Assert.True(await repo.Add(admin));
        }

        [Fact]
        public async Task TestDeleteAdmin()
        {
            var admin = new TblAdmin();
            admin.AdminId = Guid.NewGuid();
            admin.UserId = Guid.NewGuid();
            await repo.Add(admin);
            Assert.True(await repo.Delete(admin.AdminId));
        }

        [Fact]
        public async Task TestGetAllUsers()
        {
            string expected = "2A0CB704-DEB6-4323-8EDA-34D752EB3B79";
            var result = repo.GetAllUsers().ToList();
            
            Assert.Equal(expected, result[1].UserId.ToString().ToUpper());
        }

        [Fact]
        public async Task TestDoesExist()
        {
            Guid id = Guid.Parse("3009F8A3-C9E3-449B-AC44-24C18B3920D4");
            Assert.True(repo.DoesExist(id));
        }

        [Fact]
        public async Task TestIsAdmin()
        {
            string userId = "8A0CB704-DEB6-4323-8EDA-34D752EB3B79";
            Assert.True(repo.IsAdmin(userId));
        }
    }
}
