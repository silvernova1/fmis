using fmis.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models.John.Role
{
    public class RoleEdit
    {
        public IdentityRole Role { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }
        public IEnumerable<fmisUser> Members { get; set; }
        public IEnumerable<fmisUser> NonMembers { get; set; }
        public RoleEdit()
        {
            this.Updated_At = DateTime.Now;
        }
    }
}
