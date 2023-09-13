using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace fmis.Models.ppmp
{
    public class AppModel
    {
        public int Id { get; set; }

        public int UacsId { get; set; }

        public int? Expense_id { get; set; }

        public string ProcurementProject { get; set; }

        public string EndUser { get; set; }

        public string IsEarlyProcurement { get; set; }

        public string ModeOfProcurement { get; set; }

        public DateTime? Advertising { get; set; }

        public DateTime? Submission { get; set; }

        public DateTime? NoticeOfAward { get; set; }

        public DateTime? ContractSigning { get; set; }

        public string FundSource { get; set; }

        public decimal Total { get; set; }

        public decimal Mooe { get; set; }

        public decimal Co { get; set; }

        public string Remarks { get; set; }

        [ForeignKey("Expense_id")]
        public Expense Expense { get; set; }
    }
}
