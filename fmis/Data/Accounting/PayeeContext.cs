using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Data.Accounting
{
    public class PayeeContext : DbContext
    {
        public PayeeContext(DbContextOptions<PayeeContext> options)
         : base(options)
        {
        }
    }
}
