<<<<<<< HEAD
﻿using fmis.Models;
using fmis.Models.Accounting;
=======
﻿using fmis.Models.Accounting;
>>>>>>> 3772890c2b9b3e6f22114bd63f5c7c04675c94eb
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace fmis.Data.Accounting
{
    public class IndexofpaymentContext : DbContext
    {
        public IndexofpaymentContext(DbContextOptions<IndexofpaymentContext> options)
          : base(options)
        {
        }
<<<<<<< HEAD
        public DbSet<IndexOfPayment> IndexOfPayment { get; set; }
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
                var now = DateTime.Now;

                if (entity.State == EntityState.Added)
                {
                    ((BaseEntityTimeStramp)entity.Entity).CreatedAt = now;
                }
                ((BaseEntityTimeStramp)entity.Entity).UpdatedAt = now;
            }
        }
=======
        public DbSet<IndexOfPayment> Indexofpayment { get; set; }
>>>>>>> 3772890c2b9b3e6f22114bd63f5c7c04675c94eb
    }
}
