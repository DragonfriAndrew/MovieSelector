using MovieApp.Models;
using System;
using System.Threading.Tasks;

namespace MovieApp.Repositories
{
    public interface ICommentRepository
    {
        Task<TblComment> GetSingle(Guid id);

        Task<bool> Add(TblComment item);

        Task<bool> Delete(Guid id);

        bool DoesExist(Guid id);
    }
}