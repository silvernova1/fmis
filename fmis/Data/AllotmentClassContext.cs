using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;

namespace fmis.Data
{
    public class AllotmentClassContext : DbContext
    {

        public AllotmentClassContext(DbContextOptions<AllotmentClassContext> options)
            : base(options)
        {
        }

        public DbSet<fmis.Models.John.AllotmentClass> AllotmentClass { get; set; }

    }
}