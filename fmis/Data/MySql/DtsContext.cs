using fmis.Models.UserModels;
using Microsoft.EntityFrameworkCore;

namespace fmis.Data.MySql
{
    public class DtsContext : DbContext
    {
        public DtsContext(DbContextOptions<DtsContext> options)
            : base(options)
        { 
        }

        public DbSet<FmisUser> users { get; set; }

    }
}
