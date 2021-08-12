using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;

namespace fmis.Data
{
    public class Budget_allotmentContext : DbContext
    {

        public Budget_allotmentContext(DbContextOptions<Budget_allotmentContext> options)
            : base(options)
        {
        }

        public DbSet<fmis.Models.Budget_allotment> Budget_allotment { get; set; }
    }
}