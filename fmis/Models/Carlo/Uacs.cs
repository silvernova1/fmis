﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models.John;
using fmis.Models.silver;

namespace fmis.Models
{
    public class Uacs : BaseEntityTimeStramp
    {
        public int UacsId { get; set; }
        public string Account_title { get; set; }
        public string Expense_code { get; set; }
        public string uacs_type { get; set; }
        public string status { get; set; }
        public string token { get; set; }
    }
}