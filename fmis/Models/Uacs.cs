using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models
{
    public class Uacs
    {
        public int Id { get; set; }
        public string Account_title { get; set; }
        public int Expense_code { get; set; }
        [DataType(DataType.Date)]
        public DateTime Created_at { get; set; }
        [DataType(DataType.Date)]
        public DateTime Updated_at { get; set; }
       
      
    }
}
