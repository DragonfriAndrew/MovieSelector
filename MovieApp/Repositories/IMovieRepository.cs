using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.Models;

namespace MovieApp.Repositories
{
    public interface IMovieRepository
    {
        Task<bool> Add(TblMovie item);
        Task<bool> Delete(Guid id);
        Task<IEnumerable<TblMovie>> GetAll();
        Task<TblMovie> GetSingle(Guid id);
        Task<bool> Update(TblMovie item);
        bool DoesExist(Guid id);
        bool IsAdmin(String token);
        List<TblComment> GetComments(Guid id);
    }
}