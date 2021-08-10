using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;

namespace fmis.Data
{
    public class Requesting_officeContext : DbContext
    {

        public Requesting_officeContext(DbContextOptions<Requesting_officeContext> options)
            : base(options)
        {
        }

        public DbSet<fmis.Models.Requesting_office> Requesting_office { get; set; }
    }
}