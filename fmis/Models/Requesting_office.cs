using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models
{
    public class Requesting_office
    {
        public int Id { get; set; }
        [DataType(DataType.Date)]
        public string Head_name { get; set; }
        public string Position { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }
      
    }
}
