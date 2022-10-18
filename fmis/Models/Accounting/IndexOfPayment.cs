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
        public int NumberOfBill { get; set; }

        public List<IndexDeduction> indexDeductions { get; set; }

        public double GrossAmount { get; set; }
        public double TotalDeduction { get; set; }
        public double NetAmount { get; set; }

        public string Particulars { get; set; }
        public string PoNumber { get; set; }
        public int ProjectId { get; set; }
        public string InvoiceNumber { get; set; }
        public int SoNumber { get; set; }

        public DateTime DvDate { get; set; }

        public DateTime? PeriodCover { get; set; }

        public string date { get; set; }

        public string travel_period { get; set; }

        [ForeignKey("CategoryId")]
        public int CategoryId { get; set; }
        [JsonIgnore]
        public Category Category { get; set; }

        [ForeignKey("DvId")]
        public int DvId { get; set; }
        [JsonIgnore]
        public Dv Dv { get; set; }


    }
}
