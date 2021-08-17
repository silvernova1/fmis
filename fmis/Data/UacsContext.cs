using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;

namespace fmis.Data
{
    public class UacsContext : DbContext
    {

        public UacsContext(DbContextOptions<UacsContext> options)
            : base(options)
        {
        }

        public DbSet<fmis.Models.Uacs> Uacs { get; set; }

    }
}