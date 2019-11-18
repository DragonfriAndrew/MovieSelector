using Microsoft.EntityFrameworkCore;
using MovieApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApp.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private JAMDContext _context;

        public CommentRepository(JAMDContext context)
        {
            _context = context;
        }

        public async Task<TblComment> GetSingle(Guid id)
        {
            return await _context.TblComment.FirstOrDefaultAsync(x => x.CommentId == id);
        }

        public async Task<bool> Add(TblComment item)
        {
            await _context.TblComment.AddAsync(item);
            var result = await _context.SaveChangesAsync();
            if (result == 1)
                return true;
            else
                return false;
        }

        public async Task<bool> Delete(Guid id)
        {
            TblComment movie = _context.TblComment.FirstOrDefault(x => x.CommentId == id);
            _context.TblComment.Remove(movie);
            var result = await _context.SaveChangesAsync();
            if (result == 1)
                return true;
            else
                return false;
        }

        public bool DoesExist(Guid id)
        {
            TblComment movie = _context.TblComment.FirstOrDefault(x => x.CommentId == id);
            if (movie != null)
            {
                return true;
            }
            return false;
        }
    }
}
