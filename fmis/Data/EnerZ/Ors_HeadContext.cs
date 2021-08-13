using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;

namespace fmis.Data
{
    public class Ors_headContext : DbContext
    {

        public Ors_headContext(DbContextOptions<Ors_headContext> options)
            : base(options)
        {
        }

        public DbSet<fmis.Models.Ors_head> Ors_head { get; set; }
    }
}
