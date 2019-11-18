using Microsoft.EntityFrameworkCore;
using MovieApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MovieApp.Repositories
{
    public class UserRepository : IUserRepository
    {
        private JAMDContext _context;

        private const int SALT_SIZE = 128 / 8;
        private const string KEY_STRING = "T6H7I8S9ISASECRETFORPASS9W8O7R6D";

        public UserRepository(JAMDContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TblUser>> GetAll()
        {
            return await _context.TblUser.OrderBy(m => m.UserName).ToListAsync();
        }

        public async Task<TblUser> GetSingle(Guid id)
        {
            return await _context.TblUser.FirstOrDefaultAsync(x => x.UserId == id);
        }

        public async Task<bool> Add(TblUser item)
        {
            await _context.TblUser.AddAsync(item);
            var result = await _context.SaveChangesAsync();
            if (result == 1)
                return true;
            else
                return false;
        }

        public async Task<bool> Delete(Guid id)
        {
            TblUser user = _context.TblUser.FirstOrDefault(x => x.UserId == id);
            _context.TblUser.Remove(user);
            // Check if one is admin
            TblAdmin tblAdmin = _context.TblAdmin.FirstOrDefault(m => m.UserId == id);
            if (tblAdmin != null)
            {
                _context.TblAdmin.Remove(tblAdmin);
            }
            var result = await _context.SaveChangesAsync();

            if (result == 1)
                return true;
            else
                return false;
        }

        public async Task<bool> Update(TblUser item)
        {
            _context.TblUser.Update(item);
            var result = await _context.SaveChangesAsync();
            if (result == 1)
                return true;
            else
                return false;
        }

        public bool DoesExist(Guid id)
        {
            TblUser user = _context.TblUser.FirstOrDefault(x => x.UserId == id);
            if (user != null)
            {
                return true;
            }
            return false;
        }

        public bool DoesUserNameExist(string username)
        {
            TblUser user = _context.TblUser.FirstOrDefault(x => x.UserName == username);
            if (user != null)
            {
                return true;
            }
            return false;
        }

        public async Task<TblUser> GetLoginInfo(string username, string password)
        {
            return await _context.TblUser.FirstOrDefaultAsync(x => x.UserName == username && DecryptString(x.Password) == password);
        }

        // Encrypt password
        public string EncryptString(string text)
        {
            var key = Encoding.UTF8.GetBytes(KEY_STRING);

            using (var aesAlg = Aes.Create())
            {
                using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }

                        var iv = aesAlg.IV;

                        var decryptedContent = msEncrypt.ToArray();

                        var result = new byte[iv.Length + decryptedContent.Length];

                        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                        Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

                        return Convert.ToBase64String(result);
                    }
                }
            }
        }

        // Decrypt password
        public string DecryptString(string cipherText)
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            var iv = new byte[SALT_SIZE];
            var cipher = new byte[fullCipher.Length - iv.Length];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, fullCipher.Length - iv.Length);
            var key = Encoding.UTF8.GetBytes(KEY_STRING);

            using (var aesAlg = Aes.Create())
            {
                using (var decryptor = aesAlg.CreateDecryptor(key, iv))
                {
                    string result;
                    using (var msDecrypt = new MemoryStream(cipher))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                result = srDecrypt.ReadToEnd();
                            }
                        }
                    }

                    return result;
                }
            }
        }
    }
}
