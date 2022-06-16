using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Areas.Identity.Data;
using fmis.Models.Budget;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace fmis.Data
{
    public class UserContext : IdentityDbContext<fmisUser>
    {
        public UserContext()
        {
        }

        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
        }
        public DbSet<fmisUser> BudgetYears { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<fmisUser>().ToTable("AspNetUsers");
        }

        public static UserContext Create()
        {
            return new UserContext();
        }
    }
}
