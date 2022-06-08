using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace fmis.Models.Accounting
{
    public class FundCluster : BaseEntityTimeStramp
    {
        public int FundClusterId { get; set; }
        public string FundClusterDescription { get; set; }
   /*     public ICollection<Dv> Dvs { get; set; }*/
    }
}
