using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;

namespace fmis.Data
{
    public class ObligationContext : DbContext
    {

        public ObligationContext(DbContextOptions<ObligationContext> options)
            : base(options)
        {
        }

        public DbSet<fmis.Models.Obligation> Obligation { get; set; }

    }
}