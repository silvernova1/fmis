using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;

namespace fmis.Data
{
    public class RequestingOfficeContext : DbContext
    {

        public RequestingOfficeContext(DbContextOptions<RequestingOfficeContext> options)
            : base(options)
        {
        }

        public DbSet<fmis.Models.RequestingOffice> RequestingOffice { get; set; }
    }
}
