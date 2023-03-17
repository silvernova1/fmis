using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using fmis.Models;
using fmis.Models.UserModels;
using MySql.Data.MySqlClient;

namespace fmis.Data
{
    public class fmisContext : DbContext
    {
        public fmisContext(DbContextOptions<fmisContext> options)
            : base(options)
        {
        }

        public DbSet<FmisUser> users { get; set; }
    }
}
