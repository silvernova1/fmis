using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace fmis.Models
{
    public class Suballotment_amount : BaseEntityTimeStramp
    {
        [Key]
        public int SubAllotmentAmountId { get; set; }
        public int UacsId { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal beginning_balance { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal remaining_balance { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal realignment_amount { get; set; }
        public string status { get; set; }
        public string suballotment_amount_token { get; set; }
        public string suballotment_token { get; set; }
        public int? SubAllotmentId { get; set; }
        public int BudgetAllotmentId { get; set; }
        [JsonIgnore]
        public SubAllotment SubAllotment { get; set; }
        public Uacs Uacs { get; set; }
    }
}
