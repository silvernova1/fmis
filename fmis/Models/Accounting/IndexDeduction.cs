﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models.Accounting
{
    public class IndexDeduction
    {
        [Key]
        public int IndexDeductionId { get; set; }
        public float Amount { get; set; }

        public int? DeductionId { get; set; }
        public Deduction Deduction { get; set; }
        public int? IndexOfPaymentId { get; set; }
        public IndexOfPayment IndexOfPayment { get; set; }
    }
}
