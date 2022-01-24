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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //builder.Entity<fmisUser>(b => { b.HasMany(p => p.BudgetYears); });
        }
        //public DbSet<BudgetYear> BudgetYears { get; set; }

        public static UserContext Create()
        {
            return new UserContext();
        }
    }
}
