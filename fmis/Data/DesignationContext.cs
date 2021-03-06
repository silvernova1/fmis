using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;

namespace fmis.Data
{
    public class DesignationContext : DbContext
    {

        public DesignationContext(DbContextOptions<DesignationContext> options)
            : base(options)
        {
        }

        public DbSet<fmis.Models.Designation> Designation { get; set; }
    }
}
