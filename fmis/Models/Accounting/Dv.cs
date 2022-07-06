using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace fmis.Models.Accounting
{
    public class Dv : BaseEntityTimeStramp
    {
        public int DvId { get; set; }
        public string DvNo { get; set; }
        public string Payee { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Date { get; set; }
        public string Particulars { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N}")]
        public decimal Amount { get; set; }

        [ForeignKey("FundClusterId")]
        public int FundClusterId { get; set; }
        [JsonIgnore]
        public FundCluster FundCluster { get; set; }


        public IndexOfPayment IndexOfPayment { get; set; }

    }
}
