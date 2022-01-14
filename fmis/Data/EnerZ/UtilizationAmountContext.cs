using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models.EnerZ;
using fmis.Data;
using fmis.Data.Enerz;


namespace fmis.Data.Enerz
{
    public class UtilizationAmountContext : DbContext
    {

        public UtilizationAmountContext(DbContextOptions<UtilizationAmountContext> options)
            : base(options)
        {
        }

        public DbSet<fmis.Models.EnerZ.UtilizationAmount> UtilizationAmount { get; set; }

    }
}