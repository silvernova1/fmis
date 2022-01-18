﻿using fmis.Models.silver;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models
{
    public class Yearly_reference
    {
        [Key]
        public int YearlyReferenceId { get; set; }
        [Required]
        public string YearlyReference { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }

        public List<BudgetAllotment> BudgetAllotments { get; set; }

       // public BudgetAllotment Budget_allotment { get; set; }

        public Yearly_reference()
        {
            this.Updated_at = DateTime.Now;
        }
    }
}
