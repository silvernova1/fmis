using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;

namespace fmis.Models
{
    public class Ors_head
    {
        [Key]
        public int Id { get; set; }
        public string Head_name { get; set; }
        public string Position { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }

        [ForeignKey("Personal_Information")]
        public int Pid { get; set; }
        public Personal_Information Personal_Information { get; set; }
        [ForeignKey("Designation")]
        public int Did { get; set; }
        public Designation Designation { get; set; }
        public Sub_allotment Sub_Allotment { get; set; }

    }
}
