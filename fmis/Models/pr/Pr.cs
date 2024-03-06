using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace fmis.Models.pr
{
    public class Pr
    {
        public int Id { get; set; }

        public string Prno { get; set; }

        public string UserId { get; set; }

        public DateTime PrnoDate { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal GrandTotal { get; set; }

        public string Purpose { get; set; }

        public string FundCharging { get; set; }

        public string CashAdvance { get; set; }

        public string RequestedBy { get; set; }

        public string RecoApproval { get; set; }

        public string Rmop { get; set; }

        public string RouteNumber { get; set; }

        public bool IsReceiveOnPU { get; set; } = false;

        public virtual List<PrItems> PrItems { get; set; }
    }
}
