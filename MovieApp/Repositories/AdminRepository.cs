using Microsoft.EntityFrameworkCore;
using MovieApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApp.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private JAMDContext _context;

        public AdminRepository(JAMDContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TblAdmin>> GetAll()
        {
            return await _context.TblAdmin.Include(x => x.User).ToListAsync();
        }

        public DbSet<TblUser> GetAllUsers()
        {
            return _context.TblUser;
        }

        public async Task<TblAdmin> GetSingle(Guid id)
        {
            return await _context.TblAdmin.Include(x => x.User).FirstOrDefaultAsync(x => x.AdminId == id);
        }

        public async Task<bool> Add(TblAdmin item)
        {
            await _context.TblAdmin.AddAsync(item);
            var result = await _context.SaveChangesAsync();
            if (result == 1)
                return true;
            else
                return false;
        }

        public async Task<bool> Delete(Guid id)
        {
            TblAdmin admin = _context.TblAdmin.FirstOrDefault(x => x.AdminId == id);
            _context.TblAdmin.Remove(admin);
            var result = await _context.SaveChangesAsync();
            if (result == 1)
                return true;
            else
                return false;
        }

        public bool DoesExist(Guid id)
        {
            TblAdmin admin = _context.TblAdmin.FirstOrDefault(x => x.AdminId == id);
            if (admin != null)
            {
                return true;
            }
            return false;
        }

        public bool IsAdmin(string id)
        {
            if (_context.TblAdmin.FirstOrDefault(m => m.UserId == Guid.Parse(id)) != null)
            {
                return true;
            }
            return false;
        }
    }
}
