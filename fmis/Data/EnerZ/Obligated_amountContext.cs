using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using fmis.Models;

namespace fmis.Data
{
    public class Obligated_amountContext : DbContext
    {
        public Obligated_amountContext(DbContextOptions<Obligated_amountContext> options)
         : base(options)
        {
        }
        public DbSet<fmis.Models.Obligated_amount> Obligated_amount { get; set; }
    }
}