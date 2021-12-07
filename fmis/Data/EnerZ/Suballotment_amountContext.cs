using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;

namespace fmis.Data
{
    public class Suballotment_amountContext : DbContext
    {
        public Suballotment_amountContext(DbContextOptions<Suballotment_amountContext> options)
            : base(options)
        {
        }
        public DbSet<fmis.Models.Suballotment_amount> Suballotment_amount { get; set; }
    }
}