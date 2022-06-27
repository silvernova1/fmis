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
        private readonly MyDbContext _context;

        public UserService(MyDbContext context)
        {
            _context = context;
        }

        public async Task<FmisUser> ValidateUserCredentialsAsync(string username, string password)
        {
            FmisUser user = null;
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            user = await _context.FmisUsers.FirstOrDefaultAsync(x => x.Username == username);

            if (user is null) return null;

            try
            {
                var result = _hasher.VerifyHashedPassword(user, user.Password, password);
                if (result.Equals(PasswordVerificationResult.Success))
                {
                    return user;
                }
            }
            catch(Exception ex) { Console.WriteLine(ex.Message); }

            return null;
        }

        public string HashPassword(FmisUser user, string password)
        {
            return _hasher.HashPassword(user, password);
        }
    }
}
