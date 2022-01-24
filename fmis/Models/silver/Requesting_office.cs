using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models
{
    public class Requesting_office : BaseEntityTimeStramp
    {
        [Key]
        public int Id { get; set; }

        public string pi_userid { get; set; }
    }
}
