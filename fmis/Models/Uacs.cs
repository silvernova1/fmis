﻿using System;
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
        public string Expense_code { get; set; }


        public int FundSourceId { get; set; }
        public Models.John.FundSource FundSource { get; set; }
        
    }
}
