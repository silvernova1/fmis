using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;
using System.Threading;

namespace fmis.Data
{
    public class RespoCenterTrustFundContext : DbContext
    {

        public RespoCenterTrustFundContext(DbContextOptions<RespoCenterTrustFundContext> options)
                  : base(options)
        {
        }

        public DbSet<fmis.Models.RespoCenterTrustFund> RespoCenterTrustFund { get; set; }
    }
}
