using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models.John
{
    public class AllotmentClass : BaseEntityTimeStramp 
    {
        public int Id { get; set; }
        public string Allotment_Class { get; set; }
        public string Account_Code { get; set; }
        /*[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd-HH:mm}")]*/
    }
    
}
