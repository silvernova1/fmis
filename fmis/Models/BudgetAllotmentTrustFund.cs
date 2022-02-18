using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using fmis.Models.John;

namespace fmis.Models
{
    public class BudgetAllotmentTrustFund
    {
        [Key]
        public int BudgetAllotmentTrustFundId { get; set; }

        [ForeignKey("Yearly_reference")]
        public int YearlyReferenceId { get; set; }
        [JsonIgnore]
        public Yearly_reference Yearly_reference { get; set; }

        [JsonIgnore]
        public ICollection<FundSource> FundSources { get; set; }
    }
}
