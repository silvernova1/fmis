using fmis.Data;
using fmis.Models.UserModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace fmis.Services
{
    public interface IUserService
    {
        Task<FmisUser> ValidateUserCredentialsAsync(string username, string password);
        string HashPassword(FmisUser user, string password);

        //IndexUser
        Task<IndexUser> ValidateIndexUserCredentialsAsync(string username, string password);
        string HashPasswordIndexUser(IndexUser user, string password);
    }

    public class UserService : IUserService
    {
        private readonly PasswordHasher<FmisUser> _hasher = new PasswordHasher<FmisUser>();
        private readonly PasswordHasher<IndexUser> _hasherIndexUser = new PasswordHasher<IndexUser>();
        private readonly MyDbContext _context;
        private readonly fmisContext _fcontext;

        public UserService(MyDbContext context, fmisContext fcontext)
        {
            _context = context;
            _fcontext = fcontext;

        }

        public async Task<FmisUser> ValidateUserCredentialsAsync(string username, string password)
        {
            FmisUser user = null;
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            user = await _fcontext.users.FirstOrDefaultAsync(x => x.Username == username);

            if (string.IsNullOrEmpty(user.Username)) return null;

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                user = null;
            }
            return user;
        }

        public string HashPassword(FmisUser user, string password)
        {
            return _hasher.HashPassword(user, password);
        }

        //IndexUser

        public async Task<IndexUser> ValidateIndexUserCredentialsAsync(string username, string password)
        {
            IndexUser indexUser = null;
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            indexUser = await _context.IndexUser.FirstOrDefaultAsync(x => x.Username == username);

            if (string.IsNullOrEmpty(indexUser.Username)) return null;

            if (!BCrypt.Net.BCrypt.Verify(password, indexUser.Password))
            {
                indexUser = null;
            }
            return indexUser;
        }



        public string HashPasswordIndexUser(IndexUser user, string password)
        {
            return _hasherIndexUser.HashPassword(user, password);
        }
    }
}
