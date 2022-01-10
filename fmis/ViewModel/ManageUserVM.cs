using fmis.Models;
using fmis.Models.silver;
using System.Collections.Generic;

namespace fmis.ViewModel
{
    public class ManageUserVM
    {
        public IEnumerable<Personal_Information> Personal_Information { get; set; }
        public IEnumerable<ManageUsers> ManageUsers { get; set; }
    }
}
