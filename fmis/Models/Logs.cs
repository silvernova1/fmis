using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models
{
    public class Logs
    {
        [Key]
        public int LogsId { get; set; }
        public int created_id { get; set; }
        public string created_name { get; set; }
        public string created_designation { get; set; }
        public string created_division { get; set; }
        public string created_section { get; set; }
        public int FundSourceId { get; set; }
        public int SubAllotmentId { get; set; }
        public string type { get; set; } //the type are funds_realignment,sub_allotment_realignment,obligation_realignment
        public decimal amount { get; set; }
    }
}
