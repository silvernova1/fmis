﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models.John;

namespace fmis.Models
{
    public class Appropriation : BaseEntityTimeStramp
    {
        public int AppropriationId { get; set; }
        public string AppropriationSource { get; set; }
        public List<FundSource> FundSources { get; set; }
    }
}
