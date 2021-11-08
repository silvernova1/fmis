using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models
{
    public class Utilization
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Date { get; set; }
        public string Dv { get; set; }
        public string Pr_no { get; set; }
        public string Po_no { get; set; }
        public string Payer { get; set; }
        public string Address { get; set; }
        public string Particulars { get; set; }
        public int Ors_no { get; set; }
        public string Fund_source { get; set; }
        public float Gross { get; set; }
        public int Created_by { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Date_recieved { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        public DateTime Time_recieved { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Date_released { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        public DateTime Time_released { get; set; }
        public string status { get; set; }
        public string token { get; set; }
    }
}




