using fmis.Models.ppmp;
using Microsoft.EntityFrameworkCore;

namespace fmis.Data
{
    public class PpmpContext : DbContext
    {
        public PpmpContext(DbContextOptions<PpmpContext> options)
            : base(options)
        {
        }

        public DbSet<Programs> programs { get; set; }
        public DbSet<Expense> expense { get; set; }
        public DbSet<Item> item { get; set; }

    }
}
