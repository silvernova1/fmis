using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using fmis.Models.Carlo;
using Microsoft.EntityFrameworkCore;
using fmis.Models;
using Microsoft.EntityFrameworkCore.Migrations;

namespace fmis.Data
{
    public class SaobContext : DbContext
    {

        public SaobContext(DbContextOptions<SaobContext> options)
            : base(options)
        {
        }

        public DbSet<fmis.Models.Saob> Saob { get; set; }
    }
}