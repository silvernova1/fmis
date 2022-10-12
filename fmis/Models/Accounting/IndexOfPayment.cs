﻿using Newtonsoft.Json;
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

        [DataType(DataType.Date)]
        public DateTime DvDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime PeriodCover { get; set; }

        [DataType(DataType.Date)]
        public DateTime PeriodCoverFromTo { get; set; }

        [DataType(DataType.Date)]
        public DateTime TravelPeriod { get; set; }

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
