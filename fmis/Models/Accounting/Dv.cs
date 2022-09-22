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
        public float GrossAmount { get; set; }
        public float TotalDeduction { get; set; }
        public float NetAmount { get; set; }
        public string Particulars { get; set; }

<<<<<<< HEAD

=======
>>>>>>> 47ef77797eafc615613dd23258f02bdaaa268e4f
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Date { get; set; }

        [ForeignKey("FundClusterId")]
        public int FundClusterId { get; set; }
        [JsonIgnore]
        public FundCluster FundCluster { get; set; }

        [ForeignKey("PayeeId")]
        public int PayeeId { get; set; }
        [JsonIgnore]
        public Payee Payee { get; set; }

        [ForeignKey("RespoCenterId")]
        public int RespoCenterId { get; set; }
        [JsonIgnore]
        public RespoCenter RespoCenter { get; set; }

        [ForeignKey("DeductionId")]
        public int DeductionId { get; set; }
        [JsonIgnore]
        public Deduction Deduction { get; set; }

<<<<<<< HEAD
        [ForeignKey("AssigneeId")]
        public int AssigneeId { get; set; }
        [JsonIgnore]
        public Assignee Assignee { get; set; }

=======
>>>>>>> 47ef77797eafc615613dd23258f02bdaaa268e4f

    }
}
