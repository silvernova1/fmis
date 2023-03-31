using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace fmis.Models.Accounting 
{
    public class IndexOfPayment : BaseEntityTimeStramp
    {
        public int IndexOfPaymentId { get; set; }
        public string AccountNumber { get; set; }
        public int? NumberOfBill { get; set; }
        public ICollection<BillNumber> BillNumbers { get; set; }
        public List<IndexDeduction> indexDeductions { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal TotalDeduction { get; set; }
        public decimal NetAmount { get; set; }
        public string Particulars { get; set; }
        public string PoNumber { get; set; }
        public int? ProjectId { get; set; }
        public string InvoiceNumber { get; set; }
        public int? SoNumber { get; set; }

        public int fundSource { get; set; }
        public int? allotmentClassType { get; set; }
        public string orsNo { get; set; }

        [ForeignKey("ObligationId")]
        public int? ObligationId { get; set; }
        [JsonIgnore]
        public Obligation Obligation { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DvDate { get; set; }
        public string PeriodCover { get; set; }
        public string date { get; set; }
        public string travel_period { get; set; }
        [ForeignKey("CategoryId")]
        public int? CategoryId { get; set; }
        [JsonIgnore]
        public Category Category { get; set; }
        [ForeignKey("DvId")]
        public int? DvId { get; set; }
        [JsonIgnore]
        public Dv Dv { get; set; }
        public string UserId { get; set; }


        [ForeignKey("payeeId")]
        public int? payeeId { get; set; }
        public Payee payee { get; set; }

        //public IEnumerable<SelectListItem> DvList { get; set; }

        /*public int IndexFundSourceId { get; set; }
        public virtual IndexFundSource IndexFundSource { get; set; }*/

    }
}
