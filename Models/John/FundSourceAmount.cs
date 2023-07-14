using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace fmis.Models.John
{
    public class FundSourceAmount : BaseEntityTimeStramp
    {
        [Key]
        public int FundSourceAmountId { get; set; }
        public int UacsId { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal beginning_balance { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal remaining_balance { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal realignment_amount { get; set; }
        public string status { get; set; }
        public string fundsource_amount_token { get; set; }
        public string fundsource_token { get; set; }
        public int BudgetAllotmentId { get; set; }
        public int? FundSourceId { get; set; }
        [JsonIgnore]
        public FundSource FundSource { get; set; }
        public Uacs Uacs { get; set; }
    }
}
