using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models
{
    public class Ors_head
    {
        public int Id { get; set; }
        public string Head_name { get; set; }
        public string Position { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }
    }
}
