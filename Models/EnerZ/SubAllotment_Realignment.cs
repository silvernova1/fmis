using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace fmis.Models
{
    public class SubAllotment_Realignment : BaseEntityTimeStramp
    {
        public int Id { get; set; }
        public int? SubAllotmentAmountId { get; set; } //realignment from
        public int Realignment_to { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Realignment_amount { get; set; }
        public string status { get; set; }
        public string token { get; set; }
        public int? SubAllotmentId { get; set; }
        public Suballotment_amount SubAllotmentAmount { get; set; }
        [JsonIgnore]
        public SubAllotment SubAllotment { get; set; }
    }
}
