using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;

namespace fmis.Data
{
    public class UacsamountContext : DbContext
    {

        public UacsamountContext(DbContextOptions<UacsamountContext> options)
            : base(options)
        {
        }

        public DbSet<fmis.Models.Uacsamount> Uacsamount { get; set; }

    }
}