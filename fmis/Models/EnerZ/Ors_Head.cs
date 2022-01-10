﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models
{
    public class Ors_head
    {
        [Key]
        public int Id { get; set; }
        public string Personalinfo_userid { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }

        public Ors_head()
        {
            this.Updated_at = DateTime.Now;
        }
    }
}
