﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace fmis.Models.Accounting
{
    public partial class Dv : BaseEntityTimeStramp
    {
        public int DvId { get; set; }
        public string DvNo { get; set; }
        public string? PaymentOf { get; set; }
        public decimal? GrossAmount { get; set; }
        public decimal? TotalDeduction { get; set; }
        public decimal? NetAmount { get; set; }

        public List<DvDeduction>? dvDeductions { get; set; }

        [RegularExpression(@"^[^<>/]*$", ErrorMessage = "HTML tags are not allowed.")]
        public string Particulars { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Date { get; set; }

        [ForeignKey("FundClusterId")]
        public int FundClusterId { get; set; }
        [JsonIgnore]
        public FundCluster FundCluster { get; set; }

        [ForeignKey("PayeeId")]
        public int PayeeId { get; set; }
        [JsonIgnore]
        public Payee Payee { get; set; }

        public string PayeeDesc { get; set; }

        [ForeignKey("RespoCenterId")]
        public int RespoCenterId { get; set; }
        [JsonIgnore]
        public RespoCenter RespoCenter { get; set; }

        [ForeignKey("AssigneeId")]
        public int AssigneeId { get; set; }
        [JsonIgnore]
        public Assignee Assignee { get; set; }

        public string DvType { get; set; }
        public string? DvSupType { get; set; }
        public string UserId { get; set; }

        [JsonIgnore]
        public InfraAdvancePayment? InfraAdvancePayment { get; set; }

        [JsonIgnore]
        public List<InfraRetention>? InfraRetentions { get; set; }

        [JsonIgnore]
        public List<InfraProgress>? InfraProgress { get; set; }

        public IndexOfPayment indexOfPayment { get; set; }


    }
}
