﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Data;
using Microsoft.AspNetCore.Authorization;

namespace fmis.Controllers.Budget.John
{
    [Authorize(Policy = "BudgetAdmin")]
    public class OrsReportsController : Controller
    {
        private MyDbContext db = new MyDbContext();

        public String GetOrsCode(String ors_allotment)
        {
            var allotment = db.Obligation.Where(p => p.Id.ToString() == ors_allotment).FirstOrDefault();
            return allotment.Dv ?? "";
        }


    }
}
