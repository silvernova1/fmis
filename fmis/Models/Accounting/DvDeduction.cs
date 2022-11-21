using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models.Accounting
{
    public class DvDeduction
    {
        [Key]
        public int DvDeductionId { get; set; }
        public float Amount { get; set; }  

        public int? DeductionId { get; set; }
        public Deduction Deduction { get; set; }
        public int? DvId { get; set; }
        public Dv Dv { get; set; }
    }
}
