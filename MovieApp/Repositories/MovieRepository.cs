using Microsoft.EntityFrameworkCore;
using MovieApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApp.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private JAMDContext _context;

        public MovieRepository(JAMDContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TblMovie>> GetAll()
        {
            return await _context.TblMovie.OrderByDescending(m => m.Likes).ToListAsync();
        }

        public async Task<TblMovie> GetSingle(Guid id)
        {
            return await _context.TblMovie.Include(x => x.TblComment).FirstOrDefaultAsync(x => x.MovieId == id);
        }

        public async Task<bool> Add(TblMovie item)
        {
            await _context.TblMovie.AddAsync(item);
            var result = await _context.SaveChangesAsync();
            if (result == 1)
                return true;
            else
                return false;
        }

        public async Task<bool> Delete(Guid id)
        {
            TblMovie movie = _context.TblMovie.FirstOrDefault(x => x.MovieId == id);
            _context.TblMovie.Remove(movie);
            var result = await _context.SaveChangesAsync();
            if (result == 1)
                return true;
            else
                return false;
        }

        public async Task<bool> Update(TblMovie item)
        {
            _context.TblMovie.Update(item);
            var result = await _context.SaveChangesAsync();
            if (result == 1)
                return true;
            else
                return false;
        }

        public bool DoesExist(Guid id)
        {
            TblMovie movie = _context.TblMovie.FirstOrDefault(x => x.MovieId == id);
            if (movie != null)
            {
                return true;
            }
            return false;
        }

        public bool IsAdmin(String token)
        {
            if (token == null || _context.TblAdmin.FirstOrDefault(m => m.UserId == CurrentUser.Users[token].UserId) == null)
            {
                return false;
            }
            return true;
        }

        public List<TblComment> GetComments(Guid id)
        {
            var comments = _context.TblComment.Include(x => x.Movie).Where(x => x.MovieId == id).Include(x => x.User).OrderByDescending(x => x.Date).ToList();
            return comments;
        }
    }
}
