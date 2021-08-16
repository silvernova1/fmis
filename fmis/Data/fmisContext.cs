using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using fmis.Models;

namespace fmis.Data
{
    public class fmisContext : DbContext
    {
        public fmisContext (DbContextOptions<fmisContext> options)
            : base(options)
        {
        }

        public DbSet<fmis.Models.Prexc> Prexc { get; set; }
    }
}
