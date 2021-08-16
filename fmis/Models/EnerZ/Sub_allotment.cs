using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models
{
    public class Sub_allotment
    {
        public int Id { get; set; }
        public int Prexe_code { get; set; }
        public string Suballotment_code { get; set; }
        public string Suballotment_title { get; set; }
        public int Orc_head { get; set; }
        public string Responsibility_number { get; set; }
        public string Description { get; set; }
    }
}
