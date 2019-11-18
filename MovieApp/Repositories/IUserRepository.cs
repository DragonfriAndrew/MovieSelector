using MovieApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieApp.Repositories
{
    public interface IUserRepository
    {
        Task<bool> Add(TblUser item);
        Task<bool> Delete(Guid id);
        Task<IEnumerable<TblUser>> GetAll();
        Task<TblUser> GetSingle(Guid id);
        Task<bool> Update(TblUser item);
        bool DoesExist(Guid id);
        bool DoesUserNameExist(string username);
        Task<TblUser> GetLoginInfo(string username, string password);
        string EncryptString(string text);
        string DecryptString(string text);
    }
}