using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;


namespace fmis.Data
{
    public class UtilizationAmountContext : DbContext
    {

        public UtilizationAmountContext(DbContextOptions<UtilizationAmountContext> options)
            : base(options)
        {
        }

        public DbSet<fmis.Models.UtilizationAmount> UtilizationAmount { get; set; }

    }
}