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

        //Pu User
        Task<(PuUser user, string errorMessage)> ValidatePuUserCredentialsAsync(string username, string password);
        string HashPasswordPuUser(PuUser user, string password);
    }

    public class UserService : IUserService
    {
        private readonly PasswordHasher<FmisUser> _hasher = new PasswordHasher<FmisUser>();
        private readonly PasswordHasher<IndexUser> _hasherIndexUser = new PasswordHasher<IndexUser>();
        private readonly PasswordHasher<PuUser> _hasherPuUser = new PasswordHasher<PuUser>();
        private readonly MyDbContext _context;
        private readonly fmisContext _fcontext;

        public UserService(MyDbContext context, fmisContext fcontext)
        {
            _context = context;
            _fcontext = fcontext;

        }

        public class PuUserValidationException : Exception
        {
            public PuUserValidationException(string message) : base(message)
            {
            }
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

        //Pu User

        public async Task<(PuUser user, string errorMessage)> ValidatePuUserCredentialsAsync(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return (null, "Username or password is empty");
            }

            PuUser puUser = await _context.PuUser.FirstOrDefaultAsync(x => x.Username == username);

            if (puUser == null || !BCrypt.Net.BCrypt.Verify(password, puUser.Password))
            {
                return (null, "User not found in the database or invalid password");
            }

            return (puUser, null);
        }



        public string HashPasswordPuUser(PuUser user, string password)
        {
            return _hasherPuUser.HashPassword(user, password);
        }
    }
}
