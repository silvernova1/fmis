using fmis.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Data
{
    public class SectionsContext : DbContext
    {
        public SectionsContext(DbContextOptions<SectionsContext> options)
         : base(options)
        {
        }

        public DbSet<Sections> Sections { get; set; }
    }
}