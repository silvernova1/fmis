using System;
using System.Collections.Generic;

namespace fmis.Models.pr
{
    public class Pr
    {
        public int Id { get; set; }

        public string Prno { get; set; }

        public DateTime PrnoDate { get; set; }

        public decimal GrandTotal { get; set; }

        public string Purpose { get; set; }

        public string FundCharging { get; set; }

        public string CashAdvance { get; set; }

        public string RequestedBy { get; set; }

        public string RecoApproval { get; set; }

        public virtual ICollection<PrItems> PrItems { get; set; }
    }
}
