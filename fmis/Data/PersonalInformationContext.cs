using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;

namespace fmis.Data
{
    public class PersonalInformationContext : DbContext
    {

        public PersonalInformationContext(DbContextOptions<PersonalInformationContext> options)
            : base(options)
        {
        }

        public DbSet<fmis.Models.Personal_Information> Personal_Information { get; set; }
    }
}
