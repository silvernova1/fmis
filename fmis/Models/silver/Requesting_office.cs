using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models
{
    public class Requesting_office
    {
        [Key]
        public int Id { get; set; }
        public string Head_name { get; set; }
        public string Position { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }

        [ForeignKey("Personal_information")]
        public int Pid { get; set; }
        public Personal_Information Personal_information { get; set; }

    }
}
