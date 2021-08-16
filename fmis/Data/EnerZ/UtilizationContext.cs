using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;

namespace fmis.Data
{
    public class UtilizationContext : DbContext
    {

        public UtilizationContext(DbContextOptions<UtilizationContext> options)
            : base(options)
        {
        }

        public DbSet<fmis.Models.Utilization> Utilization { get; set; }
    }
}
