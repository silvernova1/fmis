﻿using fmis.Models.UserModels;
using Microsoft.EntityFrameworkCore;

namespace fmis.Data
{
    public class fmisContext : DbContext
    {
        public fmisContext(DbContextOptions<fmisContext> options)
            : base(options)
        {
        }

        public DbSet<FmisUser> users { get; set; }
    }
}
