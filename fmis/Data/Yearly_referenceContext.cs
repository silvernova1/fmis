using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;

namespace fmis.Data
{
    public class YearlyReferenceContext : DbContext
    {

        public YearlyReferenceContext(DbContextOptions<YearlyReferenceContext> options)
            : base(options)
        {
        }

        public DbSet<fmis.Models.YearlyReference> Yearly_reference { get; set; }
    }
}