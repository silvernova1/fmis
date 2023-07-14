using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using fmis.Models;

namespace fmis.Models
{
    public class RespoCenterTrustFund
    {
        [Key]
        public int RespocentertrustfundId { get; set; }
        public string Respocentertrustfund { get; set; }
        public string RespocentertrustfundCode { get; set; }
    }
}
