using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;

namespace fmis.Data
{
    public class DivisionContext : DbContext
    {

        public DivisionContext(DbContextOptions<DivisionContext> options)
            : base(options)
        {
        }

        public DbSet<fmis.Models.Division> Division { get; set; }
    }
}