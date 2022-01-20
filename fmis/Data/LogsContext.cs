using fmis.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Data
{
    public class LogsContext : DbContext
    {
        public LogsContext(DbContextOptions<LogsContext> options)
            : base(options)
        {
        }

        public DbSet<Logs> Logs { get; set; }
    }
}
