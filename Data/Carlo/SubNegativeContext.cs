using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Threading;
using fmis.Models.Carlo;

namespace fmis.Data.Carlo
{
    public class SubNegativeContext : DbContext
    {
        public SubNegativeContext(DbContextOptions<SubNegativeContext> options)
          : base(options)
        {
        }

        public DbSet<SubNegative> SubNegative { get; set; }
    }
}
