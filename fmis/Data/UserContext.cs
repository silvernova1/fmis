using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using fmis.Models;
using fmis.Models.UserModels;

namespace fmis.Data
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options)
         : base(options)
        {
        }

        public DbSet<FmisUser> FmisUsers { get; set; }
    }
}