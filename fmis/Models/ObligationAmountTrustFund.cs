﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace fmis.Models
{
    public class ObligationAmountTrustFund : BaseEntityTimeStramp
    {
        public int Id { get; set; }
        public int ObligationTrustFundId { get; set; }
        public int UacsId { get; set; }
        public Int64 Expense_code { get; set; }
        public decimal Amount { get; set; }
        public float Total_disbursement { get; set; }
        public float Total_net_amount { get; set; }
        public float Total_tax_amount { get; set; }
        public float Total_others { get; set; }
        public string status { get; set; }
        public string obligation_token { get; set; }
        public string obligation_amount_token { get; set; }
        public FundSourceTrustFund FundSourceTrustFund { get; set; }

    }
}