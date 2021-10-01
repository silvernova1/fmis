using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models.Carlo
{
    public class FundsRealignment
    {
        public int Id { get; set; }
        public int Realignment_from { get; set; }
        public int Realignment_to { get; set; }
        public string status { get; set; }
        public string token { get; set; }
       
    }

}
