﻿using fmis.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
                .Where(x => x.Entity is BaseEntityTimeStamp && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in entities)
            {
                var now = DateTime.UtcNow; // current datetime

                if (entity.State == EntityState.Added)
                {
                    ((BaseEntityTimeStamp)entity.Entity).CreatedAt = now;
                }
                ((BaseEntityTimeStamp)entity.Entity).UpdatedAt = now;
            }
        }
    }
}
