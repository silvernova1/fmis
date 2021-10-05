using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models.Carlo;

namespace fmis.Data.Carlo
{
    public class FundsRealignmentContext : DbContext
    {
        public FundsRealignmentContext(DbContextOptions<FundsRealignmentContext> options)
            : base(options)
        {
        }

        public DbSet<fmis.Models.Carlo.FundsRealignment> FundsRealignment { get; set; }
    }
}
