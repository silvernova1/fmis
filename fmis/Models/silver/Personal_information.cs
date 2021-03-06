using fmis.Models.silver;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models
{

    public class Personal_Information : BaseEntityTimeStramp
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Pid { get; set; }
        [StringLength(100)]
        public string userid { get; set; }
        public string full_name { get; set; }
        [StringLength(255)]
        public string division { get; set; }
        [StringLength(100)]
        public string section { get; set; }
        [StringLength(100)]
        public string designation { get; set; }
        public string password { get; set; }
        public string username { get; set; }
        public Requesting_office Requesting_office { get; set; }
        public BudgetAllotment Budget_allotment { get; set; }
        public Ors_head Ors_head { get; set; }
    }
}
