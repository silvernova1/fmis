using fmis.Models;
using fmis.Models.Maiff;
using Microsoft.EntityFrameworkCore;

namespace fmis.Data
{
    public class MaiffDvContext : DbContext
    {
        public MaiffDvContext(DbContextOptions<MaiffDvContext> options)
           : base(options)
        {
        }

        public DbSet<MaiffDv> MaiffDv { get; set; }
    }
}
