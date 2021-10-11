using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;

namespace fmis.Data
{
    public class Yearly_referenceContext : DbContext
    {

        public Yearly_referenceContext(DbContextOptions<Yearly_referenceContext> options)
            : base(options)
        {
        }

        public DbSet<fmis.Models.Yearly_reference> Yearly_reference { get; set; }
    }
}