using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Data.Accounting
{
    public class IndexofpaymentContext : DbContext
    {
        public IndexofpaymentContext(DbContextOptions<IndexofpaymentContext> options)
          : base(options)
        {
        }
    }
}
