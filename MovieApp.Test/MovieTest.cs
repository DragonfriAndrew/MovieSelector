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
    public class MovieTest
    {
        private JAMDContext _context;

        string str1 = "3009F8A3-C9E3-449B-AC44-24C18B3920D4";

        MovieRepository repo;
        JAMDContext context;

        public MovieTest()
        {
            var builder = new DbContextOptionsBuilder<JAMDContext>()
               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());

            context = new JAMDContext(builder.Options);

            var comments = Enumerable.Range(1, 9)
                .Select(i => new TblComment { CommentId = Guid.NewGuid(), UserId = Guid.Parse(str1), MovieId = Guid.Parse(str1), Comment = $"Awesome-{i}" });
            context.TblComment.AddRange(comments);

            var user = new TblUser { UserId = Guid.Parse(str1), UserName = "TestUser" };
            context.TblUser.Add(user);

            var movies = Enumerable.Range(1, 9)
                .Select(i => new TblMovie { MovieId = Guid.Parse(i.ToString() + str1.Substring(1)), MovieName = $"Movie-{i}" });
            context.TblMovie.AddRange(movies);

            var admin = context.TblAdmin.Add(new TblAdmin { AdminId = Guid.NewGuid(), UserId = Guid.Parse(str1) });

            int changed = context.SaveChanges();
            _context = context;
            repo = new MovieRepository(_context);
        }

        [Fact]
        public async Task TestGetSingleMovie()
        {
            string expected = "Movie-2";
            string movieToTest = "2009F8A3-C9E3-449B-AC44-24C18B3920D4";
            var result = await repo.GetSingle(Guid.Parse(movieToTest));
            Assert.Equal(expected, result.MovieName);
        }

        [Fact]
        public async Task TestGetAllMovies()
        {
            string expected = "Movie-2";
            var result = await repo.GetAll();
            Assert.Equal(expected, result.ElementAt(1).MovieName.ToString());
        }

        [Fact]
        public async Task TestAddMovie()
        {
            var movie = new TblMovie();
            movie.MovieId = Guid.NewGuid();
            Assert.True(await repo.Add(movie));
        }

        [Fact]
        public async Task TestDeleteMovie()
        {
            var movie = new TblMovie();
            movie.MovieId = Guid.NewGuid();
            await repo.Add(movie);
            Assert.True(await repo.Delete(movie.MovieId));
        }

        [Fact]
        public async Task TestUpdateMovie()
        {
            string strToTest = "2009F8A3-C9E3-449B-AC44-24C18B3920D4";
            var movie = await repo.GetSingle(Guid.Parse(strToTest));
            movie.MovieName = "Movie-Two";
            Assert.True(await repo.Update(movie));
        }

        [Fact]
        public async Task TestDoesExists()
        {
            Guid id = Guid.Parse("3009F8A3-C9E3-449B-AC44-24C18B3920D4");
            Assert.True(repo.DoesExist(id));
        }

        [Fact]
        public async Task TestIsAdmin()
        {
            string token = "testToken";
            CurrentUser.Users.Add(token, new TblUser { UserId = Guid.Parse(str1) });
            Assert.True(repo.IsAdmin(token));
        }

        [Fact]
        public async Task TestGetComments()
        {
            string expected = "Awesome-3";
            var result = repo.GetComments(Guid.Parse(str1));
            Assert.Equal(expected, result[2].Comment);
        }
    }
}
