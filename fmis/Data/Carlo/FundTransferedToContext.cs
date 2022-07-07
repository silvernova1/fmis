using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Threading;
using fmis.Models.Carlo;

namespace fmis.Data.Carlo
{
    public class FundTransferedToContext : DbContext
    {
        public FundTransferedToContext(DbContextOptions<FundTransferedToContext> options)
           : base(options)
        {
        }

        public DbSet<FundTransferedTo> FundTransferedTo { get; set; }
    }
}
