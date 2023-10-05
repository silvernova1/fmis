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

        public string? IsEarlyProcurement { get; set; }

        public string? ModeOfProcurement { get; set; }

        public DateTime? Advertising { get; set; }

        public DateTime? Submission { get; set; }

        public DateTime? NoticeOfAward { get; set; }

        public DateTime? ContractSigning { get; set; }

        public string? FundSource { get; set; }

        public string? Total { get; set; }

        public string? Mooe { get; set; }

        public string? Co { get; set; }

        public string? Remarks { get; set; }

        [ForeignKey("Expense_id")]
        public Expense? Expense { get; set; }
    }
}
