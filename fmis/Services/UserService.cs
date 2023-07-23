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
    }

    public class UserService : IUserService
    {
        private readonly PasswordHasher<FmisUser> _hasher = new PasswordHasher<FmisUser>();
        private readonly fmisContext _context;

        public UserService(fmisContext context)
        {
            _context = context;

        }

        public async Task<FmisUser> ValidateUserCredentialsAsync(string username, string password)
        {
            FmisUser user = null;
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            user = await _context.users.FirstOrDefaultAsync(x => x.Username == username);

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
    }
}
