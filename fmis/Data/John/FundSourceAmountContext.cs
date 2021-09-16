using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Data.John
{
    public class FundSourceAmountContext : DbContext
    {

        public FundSourceAmountContext(DbContextOptions<FundSourceAmountContext> options)
            : base(options)
        {
        }

        public DbSet<fmis.Models.John.FundSourceAmount> FundSourceAmount { get; set; }

    }
}