using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models
{
    public class Suballotment_amount
    {
        [Key]
        public int Id { get; set; }
        public string Expenses { get; set; }
        /*public string Amount { get; set; }*/
        public float Amount { get; set; }
        public float Remsubamount { get; set; }
        public string status { get; set; }
        public string token { get; set; }
        public int FundSourceId { get; set; }
        public int BudgetId { get; set; }

       /* public Sub_allotment Sub_allotment { get; set; }
        public Budget_allotment Budget_allotment { get; set; }*/

    }
}
