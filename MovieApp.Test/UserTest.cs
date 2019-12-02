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
    public class UserTest
    {
        private JAMDContext _context;

        string str1 = "3009F8A3-C9E3-449B-AC44-24C18B3920D4";

        UserRepository repo;
        JAMDContext context;

        public UserTest()
        {
            var builder = new DbContextOptionsBuilder<JAMDContext>()
               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());

            context = new JAMDContext(builder.Options);

            var users = Enumerable.Range(1, 9)
                .Select(i => new TblUser { UserId = Guid.Parse(i.ToString() + str1.Substring(1)), UserName = $"User-{i}", Password = $"AuYTVzAlMfr/PeVRVUHuWV855SRZIcgv1G4Ia/{i}HQ8E=" });
            context.TblUser.AddRange(users);

            int changed = context.SaveChanges();
            _context = context;
            repo = new UserRepository(_context);
        }

        [Fact]
        public async Task TestGetSingleUser()
        {
            string expected = "User-2";
            string userToTest = "2009F8A3-C9E3-449B-AC44-24C18B3920D4";
            var result = await repo.GetSingle(Guid.Parse(userToTest));
            Assert.Equal(expected, result.UserName);
        }

        [Fact]
        public async Task TestGetAllUsers()
        {
            string expected = "User-2";
            var result = await repo.GetAll();
            Assert.Equal(expected, result.ElementAt(1).UserName.ToString());
        }

        [Fact]
        public async Task TestAddUser()
        {
            var user = new TblUser();
            user.UserId = Guid.NewGuid();
            Assert.True(await repo.Add(user));
        }

        [Fact]
        public async Task TestDeleteUser()
        {
            var user = new TblUser();
            user.UserId = Guid.NewGuid();
            await repo.Add(user);
            Assert.True(await repo.Delete(user.UserId));
        }

        [Fact]
        public async Task TestUpdateUser()
        {
            string strToTest = "2009F8A3-C9E3-449B-AC44-24C18B3920D4";
            var user = await repo.GetSingle(Guid.Parse(strToTest));
            user.UserName = "User-Two";
            Assert.True(await repo.Update(user));
        }

        [Fact]
        public async Task TestDoesExists()
        {
            Guid id = Guid.Parse(str1);
            Assert.True(repo.DoesExist(id));
        }

        [Fact]
        public async Task TaskDoesUserNameExist()
        {
            string name = "User-2";
            Assert.True(repo.DoesUserNameExist(name));
        }

        [Fact]
        public async Task TestGetLoginInfo()
        {
            string username = "User-9";
            string password = "Microsoft@2019";
            TblUser expected = await repo.GetSingle(Guid.Parse("9009F8A3-C9E3-449B-AC44-24C18B3920D4"));
            var result = await repo.GetLoginInfo(username, password);
            Assert.Equal(expected.UserId, result.UserId);
        }

        [Fact]
        public async Task TestEncryptDecryptPassword()
        {
            string password = "1234";
            string encryptedPassword = repo.EncryptString(password);
            Assert.Equal(password, repo.DecryptString(encryptedPassword));
        }
    }
}
