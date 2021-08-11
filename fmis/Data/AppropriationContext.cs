using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;

namespace fmis.Data
{
    public class AppropriationContext : DbContext
    {

        public AppropriationContext(DbContextOptions<AppropriationContext> options)
            : base(options)
        {
        }

        public DbSet<fmis.Models.Appropriation> Appropriation { get; set; }
    }
}