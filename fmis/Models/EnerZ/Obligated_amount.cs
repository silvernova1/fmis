using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models
{
    public class Obligated_amount
    {
        public int Id { get; set; }
        public int Obligation_id { get; set; }
        public int Expenses_title { get; set; }
        public int Code { get; set; }
        public float Amount { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }

    }
}
