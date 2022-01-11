using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using fmis.Models.John;

namespace fmis.Models.silver
{
    public class BudgetAllotment
    {
        [Key]
        public int BudgetAllotmentId { get; set; }
        public string Allotment_series { get; set; }
        public string Allotment_title { get; set; }
        public string Allotment_code { get; set; }
        public decimal beginning_balance { get; set; }
        public decimal obiligate_amount { get; set; }
        public decimal realignment_amount { get; set; }
        public decimal remaining_balance { get; set; }
        public DateTime Updated_at { get; set; }
        public DateTime Created_at { get; set; }
        [JsonIgnore]
        public ICollection<FundSource> FundSources { get; set; }
        [JsonIgnore]
        public ICollection<Sub_allotment> Sub_allotments { get; set; }
        [JsonIgnore]
        public ICollection<Personal_Information> Personal_Information { get; set; }

        [ForeignKey("Yearly_reference")]
        public int YearlyReferenceId { get; set; }
        [JsonIgnore]
        public Yearly_reference Yearly_reference { get; set; }
        public BudgetAllotment()
        {
            this.Updated_at = DateTime.Now;
        }
    }
}
