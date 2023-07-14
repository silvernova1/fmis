using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using fmis.Models;

namespace fmis.Models
{
    public class RequestingOfficeTrustFund
    {
        [Key]
        public int HeadnameId { get; set; }
        public string Headname{ get; set; }
        public string Division { get; set; }
        public string Section { get; set; }
        public string Headinformation { get; set; }

    }
}

