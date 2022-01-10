using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;
using fmis.Models.silver;

namespace fmis.Models
{
    public class Sub_allotment  
    {
        [Key]
        public int SubAllotmentId { get; set; }
        public string Suballotment_title { get; set; }
        public string Description { get; set; }
        public string Suballotment_code { get; set; }
        public string Responsibility_number { get; set; }
        public int Budget_allotmentBudgetAllotmentId { get; set; }
        [ForeignKey("Prexc")]
        public int prexcId { get; set; }
        public Prexc prexc { get; set; }
        public decimal Remaining_balance { get; set; }
        public decimal Beginning_balance { get; set; }
        public decimal obligated_amount { get; set; }
        public decimal realignment_amount { get; set; }
        public string token { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }
        public BudgetAllotment Budget_allotment { get; set; }
        public ICollection<Suballotment_amount> SubAllotmentAmounts { get; set; }
        public ICollection<SubAllotment_Realignment> SubAllotmentRealignment { get; set; }
        public ICollection<Uacs> Uacs { get; set; }
        public Sub_allotment()
        {
            this.Updated_At = DateTime.Now;
        }
    }

}

