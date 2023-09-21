using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models.Accounting
{
    public class DvDeduction
    {
        [Key]
        public int DvDeductionId { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Amount { get; set; }  

        public int? DeductionId { get; set; }
        public Deduction Deduction { get; set; }
        public int? DvId { get; set; }
        public Dv Dv { get; set; }
    }
}
