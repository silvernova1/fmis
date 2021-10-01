using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;


namespace fmis.Models
{
    public class Sub_allotment
    {
        [Key]
        public int SubId { get; set; }
        public int Prexe_code { get; set; }
        public string Suballotment_code { get; set; }
        public string Suballotment_title { get; set; }
        public int Ors_head { get; set; }
        public string Responsibility_number { get; set; }
        public string Description { get; set; }
        public int Budget_allotmentBudgetAllotmentId { get; set; }

        [ForeignKey("Prexc")]
        public int Id { get; set; }
        public Prexc Prexc { get; set; }


        public Budget_allotment Budget_allotment { get; set; }
        public List<Suballotment_amount> Suballotment_amount { get; set; }

    }
}
