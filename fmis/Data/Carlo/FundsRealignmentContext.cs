using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;

namespace fmis.Data.Carlo
{
    public class FundsRealignmentContext : DbContext
    {
        public FundsRealignmentContext(DbContextOptions<FundsRealignmentContext> options)
            : base(options)
        {
        }

        public DbSet<FundsRealignment> FundsRealignment { get; set; }
    }
}
