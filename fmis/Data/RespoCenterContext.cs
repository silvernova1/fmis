using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;
using System.Threading;

namespace fmis.Data
{
    public class RespoCenterContext : DbContext
    {

        public RespoCenterContext(DbContextOptions<RespoCenterContext> options)
                  : base(options)
        {
        }

        public DbSet<fmis.Models.RespoCenter> RespoCenter { get; set; }
    }
}
