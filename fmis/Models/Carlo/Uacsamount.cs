using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models
{
    public class Uacsamount
    {
        public int Id { get; set; }
        public int ObligationId { get; set; }
        public string Account_title { get; set; }
        public string Expense_code { get; set; }
        public float Amount { get; set; }
        public float Total_disbursement { get; set; }
        public float Total_net_amount { get; set; }
        public float Total_tax_amount { get; set; }
        public float Total_others { get; set; }
        public string status { get; set; }
        public string token { get; set; }
      /*  public Uacs uacs { get; set; }*/

 

    }
}