using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;

namespace fmis.Data.silver
{
    public class RequestingOfficeContext : DbContext
    {
        public RequestingOfficeContext(DbContextOptions<RequestingOfficeContext> options)
            : base(options)
        {
        }

        public DbSet<Requesting_office> Requesting_office { get; set; }
    }
}
