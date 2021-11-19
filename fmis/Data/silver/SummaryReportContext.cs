using fmis.Models.silver;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Data.silver
{
   
     public class SummaryReportContext : DbContext
    {
        public SummaryReportContext(DbContextOptions<SummaryReportContext> options)
            : base(options)
        {
        }

        public DbSet<SummaryReport> SummaryReport { get; set; }
    }
}
