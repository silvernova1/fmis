using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;
using fmis.Models.silver;

namespace fmis.Data.silver
{
    public class ManageUsersContext : DbContext
    {
        public ManageUsersContext(DbContextOptions<ManageUsersContext> options)
            : base(options)
        {
        }

        public DbSet<ManageUsers> ManageUsers { get; set; }
    }
}
