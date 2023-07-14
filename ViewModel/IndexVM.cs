using fmis.Models;
using fmis.Models.John;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace fmis.ViewModel
{
    public class IndexVM
    {
        public int? SelectedAllotment { get; set; }
        public IEnumerable<SelectListItem> AllotmentClassList { get; set; }

        public int? SelectedObligation { get; set; }
        public IEnumerable<SelectListItem> ObligationsList { get; set; }
    }
}
