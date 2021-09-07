using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;


namespace fmis.Models
{
    public class Sub_allotment
    {
        [Key]
        public int Id { get; set; }
        public int Prexe_code { get; set; }
        public string Suballotment_code { get; set; }
        public string Suballotment_title { get; set; }
        public int Ors_head { get; set; }
        public string Responsibility_number { get; set; }
        public string Description { get; set; }

        public List<Ors_head> Ors_Heads { get; set; }
        /*public IList<FundSource> FundSource { get; set; }*/
    }
}
