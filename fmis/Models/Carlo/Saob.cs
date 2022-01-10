using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models
{
    public class Saob
    {
        public int Id { get; set; }
        [DataType(DataType.Date)]
        public DateTime datefrom { get; set; }
        [DataType(DataType.Date)]
        public DateTime dateto { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }

        public Saob()
        {
            this.Updated_At = DateTime.Now;
        }
    }
}
