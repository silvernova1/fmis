using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models.EnerZ
{
    public class UtilizationAmount
    {
        public int Id { get; set; }
        public int UtilizationId { get; set; }
        public int UacsId { get; set; }
        public Int64 Expense_code { get; set; }
        public decimal Amount { get; set; }
        public float Total_disbursement { get; set; }
        public float Total_net_amount { get; set; }
        public float Total_tax_amount { get; set; }
        public float Total_others { get; set; }
        public string status { get; set; }
        public string utilization_token { get; set; }
        public string utilization_amount_token { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }

        public UtilizationAmount()
        {
            this.Updated_At = DateTime.Now;
        }
    }
}
