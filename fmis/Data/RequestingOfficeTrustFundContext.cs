using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;
using System.Threading;

namespace fmis.Data
{
    public class RequestingOfficeTrustFundContext : DbContext
    {

        public RequestingOfficeTrustFundContext(DbContextOptions<RequestingOfficeTrustFundContext> options)
                  : base(options)
        {
        }

        public DbSet<fmis.Models.RequestingOfficeTrustFund> RequestingOfficeTrustFund { get; set; }
    }
}
