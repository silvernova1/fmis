using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace fmis.Models.Accounting
{
    public class InOfPayDeduction : BaseEntityTimeStramp
    {
        [Key]
        public int Deduction { get; set; }
        public int Amount { get; set; }

        [ForeignKey("IndexOfPayment")]
        public int IndexOfPaymentId { get; set; }
        [JsonIgnore]
        public IndexOfPayment IndexOfPayment { get; set; }
    }
}
