using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;
using fmis.Models.John;
using System.Threading;

namespace fmis.Data
{
    public class SubAllotmentContext : DbContext
    {
        public DbSet<SubAllotment> SubAllotment { get; set; }
        public DbSet<Uacs> Uacs { get; set; }

        public SubAllotmentContext(DbContextOptions<SubAllotmentContext> options)
            : base(options)
        {
        }

        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            AddTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries()
                .Where(x => x.Entity is BaseEntityTimeStramp && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in entities)
            {
                //var now = DateTime.UtcNow; // current datetime
                var now = DateTime.Now;

                if (entity.State == EntityState.Added)
                {
                    ((BaseEntityTimeStramp)entity.Entity).CreatedAt = now;
                }
                ((BaseEntityTimeStramp)entity.Entity).UpdatedAt = now;
            }
        }
    }
}