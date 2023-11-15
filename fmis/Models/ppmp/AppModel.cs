using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace fmis.Models.ppmp
{
    public class AppModel
    {
        public int Id { get; set; }

        public string? Uacs { get; set; }

        public string? ProcurementProject { get; set; }

        public string? EndUser { get; set; }

        public string? EarlyProcured { get; set; }

        public string? NotEarlyProcurered { get; set; }

        public string? ModeOfProcurement { get; set; }

        public DateTime? Advertising { get; set; }

        public DateTime? Submission { get; set; }

        public DateTime? NoticeOfAward { get; set; }

        public DateTime? ContractSigning { get; set; }

        public string? FundSource { get; set; }

        public decimal Total { get; set; }

        public decimal Mooe { get; set; }

        public decimal Co { get; set; }

        public string? Remarks { get; set; }

        public int? Expense_id { get; set; }

        public string? Tranche { get; set; }

        [ForeignKey("Expense_id")]
        public AppExpense? AppExpense { get; set; }
    }
}
