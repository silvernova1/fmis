using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace fmis.Data
{
    public class UserContext : IdentityDbContext<fmisUser>
    {
        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            this.SeedUsers(builder);
        }

        private void SeedUsers(ModelBuilder builder)
        {
            fmisUser user = new fmisUser()
            {
                UserName = "Budget",
                Email = "Budget@gmail.com",
                LockoutEnabled = false,
                PhoneNumber = "09208658303",
            };

            PasswordHasher<fmisUser> passwordHasher = new PasswordHasher<fmisUser>();
            user.PasswordHash = passwordHasher.HashPassword(user, "Admin@123");

            builder.Entity<fmisUser>().HasData(user);
        }
    }
}
