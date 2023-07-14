using fmis.Models.Accounting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Data.Accounting
{
    public class AssigneeContext : DbContext
    {
        public AssigneeContext(DbContextOptions<AssigneeContext> options)
           : base(options)
        {
        }
        public DbSet<Assignee> Assignee { get; set; }
    }
}
