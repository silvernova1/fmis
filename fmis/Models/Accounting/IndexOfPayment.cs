using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models.Accounting
{
    public class IndexOfPayment : BaseEntityTimeStramp
    {
        public int IndexOfPaymentId { get; set; }
        public int DeductionId { get; set; }
        public DateTime DvDate { get; set; }
        public string Particulars { get; set; }
        public string PO { get; set; }
        public int ProjectId { get; set; }
        public string Invoice { get; set; }
        public DateTime PeriodCover { get; set; }
        public string PeriodCovered { get; set; }
        public int SO { get; set; }
        public string TravelPeriod { get; set; }
        public int AccountNum { get; set; }
        public int NumOfBill { get; set; }
        public int GrossAmount { get; set; }
        public int TotalDeduction { get; set; }
        public int NetAmount { get; set; }
        public int DvId { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        [JsonIgnore]
        public Category Category { get; set; }
        [JsonIgnore]
        public Dv Dv { get; set; }
    }
}
