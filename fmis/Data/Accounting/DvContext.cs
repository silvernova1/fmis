using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Data.Accounting
{
    public class DvContext : DbContext
    {
        public DvContext(DbContextOptions<DvContext> options)
         : base(options)
        {
        }
    }
}
