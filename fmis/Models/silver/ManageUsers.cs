
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models.silver
{
    public class ManageUsers
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string UserRole { get; set; }
    }
}
