using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;
using System.Threading;

namespace fmis.Data
{
    public class RequestingHeadContext : DbContext
    {

        public RequestingHeadContext(DbContextOptions<RequestingHeadContext> options)
                  : base(options)
        {
        }

        public DbSet<fmis.Models.RequestingHead> RequestingHead { get; set; }
    }
}
