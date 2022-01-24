using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models.John
{
    public class AllotmentClass
    {
        public int Id { get; set; }
        public string Allotment_Class { get; set; }
        public string Account_Code { get; set; }
        [DataType(DataType.Date)]
        public DateTime Created_At { get; set; }
        [DataType(DataType.Date)]
        public DateTime Updated_At { get; set; }

        public AllotmentClass()
        {
            this.Updated_At = DateTime.Now;
        }
    }
    
}
