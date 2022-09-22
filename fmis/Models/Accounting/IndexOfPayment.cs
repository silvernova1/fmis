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
        public float GrossAmount { get; set; }
        public float TotalDeduction { get; set; }
        public float NetAmount { get; set; }
        public string Particulars { get; set; }
        public string PoNumber { get; set; }
        public int ProjectId { get; set; }
        public string InvoiceNumber { get; set; }
        public int SoNumber { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DvDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime PeriodCover { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime PeriodCoverFromTo { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public string TravelPeriod { get; set; }

 


        [ForeignKey("CategoryId")]
        public int CategoryId { get; set; }
        [JsonIgnore]
        public Category Category { get; set; }

        [ForeignKey("DeductionId")]
        public int DeductionId { get; set; }
        [JsonIgnore]
        public Deduction Deduction { get; set; }
  
    }
}
