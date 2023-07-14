
using fmis.Models.UserModels;
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
        public IEnumerable<FmisUser> Members { get; set; }
        public IEnumerable<FmisUser> NonMembers { get; set; }

    }
}
