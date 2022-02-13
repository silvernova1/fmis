using fmis.Models.silver;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models
{
    public class Yearly_reference : BaseEntityTimeStramp
    {
        [Key]
        public int YearlyReferenceId { get; set; }

        public string YearlyReference { get; set; }

        //public List<BudgetAllotment> BudgetAllotments { get; set; }

       // public BudgetAllotment Budget_allotment { get; set; }

    }
}
