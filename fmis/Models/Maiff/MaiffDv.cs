using fmis.Models.Accounting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
namespace fmis.Models.Maiff

{
    public class MaiffDv 
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Payee { get; set; }
        public string Address { get; set; }
        public DateTime MonthYearFrom { get; set; }
        public DateTime MonthYearTo { get; set; }
        public string SaaNumber { get; set; }
        public double Amount1 { get; set; }
        public double Amount2 { get; set; }
        public double Amount3 { get; set; }
        public double TotalAmount { get; set; }
        public string Deduction1 { get; set; }
        public string Deduction2 { get; set; }
        public double DeductionAmount1 { get; set; }
        public double DeductionAmount2 { get; set; }
        public double TotalDeductionAmount { get; set; }
        public double OverallTotalAmount { get; set; }



    }
}
