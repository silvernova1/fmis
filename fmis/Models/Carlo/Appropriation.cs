﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models
{
    public class Appropriation
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int Code { get; set; }
    }
}
