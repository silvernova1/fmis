using fmis.Models.Accounting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Data.Accounting
{
    public class DeductionContext : DbContext
    {
        public DeductionContext(DbContextOptions<DeductionContext> options)
         : base(options)
        {
        }
        public DbSet<Deduction> Deduction { get; set; }
    }
}
