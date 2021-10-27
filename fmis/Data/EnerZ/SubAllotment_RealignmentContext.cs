using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models.Carlo;

namespace fmis.Data
{
    public class SubAllotment_RealignmentContext : DbContext
    {
        public SubAllotment_RealignmentContext(DbContextOptions<SubAllotment_RealignmentContext> options)
            : base(options)
        {
        }

        public DbSet<fmis.Models.SubAllotment_Realignment> SubAllotment_Realignment { get; set; }
    }
}
