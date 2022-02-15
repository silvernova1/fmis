using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using fmis.Models.John;
using fmis.Models.silver;

namespace fmis.Models
{
    public class RespoCenter
    {
        [Key]
        public int RespoId { get; set; }
        public string Respo { get; set; }
        public string RespoCode { get; set; }
        public string RespoHead { get; set; }
        public string RespoHeadPosition { get; set; }
    }
}