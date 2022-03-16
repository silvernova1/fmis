﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using fmis.Models.John;


namespace fmis.Models
{
    public class Utilization : BaseEntityTimeStramp
    {
        public int Id { get; set; }

        [JsonIgnore]
        [ForeignKey("FundSourceTrustFundId")]
        public FundSourceTrustFund FundSourceTrustFund { get; set; }
        public int? FundSourceTrustFundId { get; set; }


        public string source_type { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Date { get; set; }
        public string Dv { get; set; }
        public string Pr_no { get; set; }
        public string Po_no { get; set; }
        public string Payee { get; set; }
        public string Address { get; set; }
        public string Particulars { get; set; }
        public int Ors_no { get; set; }
        public float Gross { get; set; }
        public decimal remaining_balance { get; set; }
        public int Created_by { get; set; }
        public string status { get; set; }
        public string utilization_token { get; set; }
        public ICollection<UtilizationAmount> UtilizationAmount { get; set; }
        public ICollection<FundSource> FundSources { get; set; }
        public ICollection<SubAllotment> SubAllotments { get; set; }
        public ICollection<Uacs> Uacs { get; set; }

    }
}




