using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models.John;

namespace fmis.Models
{
    public class Obligation : BaseEntityTimeStramp
    {
        public int Id { get; set; }
        public int source_id { get; set; }
        public string source_type { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Date { get; set; }
        public string Dv { get; set; }
        public string Pr_no { get; set; }
        public string Po_no { get; set; }
        public string Payee { get; set; }
        public string Address { get; set; }
        public string Particulars { get; set; }
        public int Ors_no { get; set; }
        public float Gross { get; set; }

        public string exp_code1 { get; set; }
        public float amount_1 { get; set; }
        public string exp_code2 { get; set; }
        public float amount_2 { get; set; }
        public string exp_code3 { get; set; }
        public float amount_3 { get; set; }
        public string exp_code4 { get; set; }
        public float amount_4 { get; set; }
        public string exp_code5 { get; set; }
        public float amount_5 { get; set; }
        public string exp_code6 { get; set; }
        public float amount_6 { get; set; }
        public string exp_code7 { get; set; }
        public float amount_7 { get; set; }
        public string exp_code8 { get; set; }
        public float amount_8 { get; set; }
        public string exp_code9 { get; set; }
        public float amount_9 { get; set; }
        public string exp_code10 { get; set; }
        public float amount_10 { get; set; }
        public string exp_code11 { get; set; }
        public float amount_11 { get; set; }
        public string exp_code12 { get; set; }
        public float amount_12 { get; set; }


        public decimal remaining_balance { get; set; }
        public int Created_by { get; set; }
        public string status { get; set; }
        public string obligation_token { get; set; }
        public ICollection<ObligationAmount> ObligationAmounts { get; set; }
        public ICollection<FundSource> FundSource { get; set; }
        public ICollection<Sub_allotment> SubAllotment { get; set; }
        public ICollection<Uacs> Uacs { get; set; }


        /*[DataType(DataType.Date)]
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
        public DateTime Time_released { get; set; }*/
    }
}


