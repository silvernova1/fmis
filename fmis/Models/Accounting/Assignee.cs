using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models.Accounting
{
    public class Assignee : BaseEntityTimeStramp
    {
        public int AssigneeId { get; set; }
        public string FullName { get; set; }
        public string Description { get; set; }
        public string Designation { get; set; }
    }
}
