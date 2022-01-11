﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace fmis.Models
{
    public class Suballotment_amount
    {
        [Key]
        public int Id { get; set; }
        public int UacsId { get; set; }
        public decimal beginning_balance { get; set; }
        public decimal remaining_balance { get; set; }
        public decimal realignment_amount { get; set; }
        public string status { get; set; }
        public string suballotment_amount_token { get; set; }
        public string suballotment_token { get; set; }
        public int? SubAllotmentId { get; set; }
        public int BudgetAllotmentId { get; set; }
        [JsonIgnore]
        public Sub_allotment SubAllotment { get; set; }
        public virtual ICollection<Uacs> Uacs { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }

        public Suballotment_amount()
        {
            this.Updated_At = DateTime.Now;
        }
    }
}
