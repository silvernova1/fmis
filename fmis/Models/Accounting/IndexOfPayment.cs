using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models.Accounting
{
    public class IndexOfPayment : BaseEntityTimeStramp
    {
        public int IndexOfPaymentId { get; set; }
        public string CategoryDescription { get; set; }
        public int CategoryId { get; set; }
        public int DeductionId { get; set; }
        public int DvId { get; set; }
        public int DvDate { get; set; }
        public string Particulars { get; set; }
        public int PO  { get; set; }
        public int ProjectId { get; set; }
        public int Invoice { get; set; }
        public int PeriodCover { get; set; }
        public int SO { get; set; }
        public int TravelPeriod { get; set; }
        public int AccountNum { get; set; }
        public int NumOfBill { get; set; }
        public int GrossAmount { get; set; }
        public int TotalDeduction { get; set; }
        public int NetAmount { get; set; }
    }
}
