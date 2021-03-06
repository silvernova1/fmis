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
    public class Obligation : BaseEntityTimeStramp
    {
        public int Id { get; set; }

        [JsonIgnore]
        [ForeignKey("FundSourceId")]
        public FundSource FundSource { get; set; }
        public int? FundSourceId { get; set; }

        [JsonIgnore]
        [ForeignKey("SubAllotmentId")]
        public SubAllotment SubAllotment { get; set; }
        public int? SubAllotmentId { get; set; }

        public string source_type { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Date { get; set; }
        public string Dv { get; set; }
        public string Pr_no { get; set; }
        public string Po_no { get; set; }
        public string Payee { get; set; }
        public string Address { get; set; }
        public string Particulars { get; set; }
        public string Ors_no { get; set; }
        public float Gross { get; set; }
        public decimal remaining_balance { get; set; }
        public int Created_by { get; set; }
        public string status { get; set; }
        public string obligation_token { get; set; }
        public ICollection<ObligationAmount> ObligationAmounts { get; set; }
        public ICollection<Uacs> Uacs { get; set; }
    }
}


