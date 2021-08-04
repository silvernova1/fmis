using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;

namespace fmis.Data
{
    public class Ors_HeadContext : DbContext
    {

        public Ors_HeadContext(DbContextOptions<Ors_HeadContext> options)
            : base(options)
        {
        }

        public DbSet<fmis.Models.Ors_Head> Ors_Head { get; set; }
    }
}