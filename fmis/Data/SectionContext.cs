using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using fmis.Models;

namespace fmis.Data
{
    public class SectionContext : DbContext
    {
        public SectionContext(DbContextOptions<SectionContext> options)
         : base(options)
        {
        }

        public DbSet<fmis.Models.Section> Section { get; set; }
    }
}