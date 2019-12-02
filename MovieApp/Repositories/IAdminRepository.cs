using Microsoft.EntityFrameworkCore;
using MovieApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieApp.Repositories
{
    public interface IAdminRepository
    {
        Task<bool> Add(TblAdmin item);
        Task<bool> Delete(Guid id);
        Task<IEnumerable<TblAdmin>> GetAll();
        DbSet<TblUser> GetAllUsers();
        Task<TblAdmin> GetSingle(Guid id);
        bool DoesExist(Guid id);
        bool IsAdmin(String id);
    }
}