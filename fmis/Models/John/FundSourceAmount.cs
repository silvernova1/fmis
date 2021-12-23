using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models.John
{
    public class FundSourceAmount
    {
        [Key]
        public int Id { get; set; }
        public string Account_title { get; set; }
        public decimal Amount { get; set; }
        public string status { get; set; }
        public string fundsource_amount_token { get; set; }
        public string fundsource_token { get; set; }
        public int FundSourceId { get; set; }
        public int BudgetId { get; set; }
        /*public FundSource FundSource { get; set; }*/
        /*public Budget_allotment Budget_allotment { get; set; }*/

    }
}
