using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;

namespace fmis.Data
{
    public class PrexcContext : DbContext
    {

        public PrexcContext(DbContextOptions<PrexcContext> options)
            : base(options)
        {
        }

        public DbSet<fmis.Models.Prexc> Prexc { get; set; }
    }
}