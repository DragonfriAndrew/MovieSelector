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
    public class CommentTest
    {

        private JAMDContext _context;
        string str1 = "3009F8A3-C9E3-449B-AC44-24C18B3920D4";

        CommentRepository repo;
        JAMDContext context;

        public CommentTest() {

            var builder = new DbContextOptionsBuilder<JAMDContext>()
             .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());

            context = new JAMDContext(builder.Options);

            var comments = Enumerable.Range(1, 9)
                .Select(i => new TblComment { CommentId = Guid.Parse(i.ToString() + str1.Substring(1)), Comment = $"Comment-" + i });

            context.TblComment.AddRange(comments);

            int changed = context.SaveChanges();
            _context = context;
            repo = new CommentRepository(_context);
        }

        [Fact]
        public async Task TestAddComment()
        {
            var comment = new TblComment();

            comment.Comment = "Soo awesome";

            comment.CommentId = Guid.NewGuid();
            comment.UserId = Guid.NewGuid();
            comment.CommentId = Guid.NewGuid();
            Assert.True(await repo.Add(comment));
        }    

        [Fact]
        public async Task TestDeleteComment()
        {
            Assert.True(await repo.Delete(Guid.Parse(str1)));
        }
    }
}
