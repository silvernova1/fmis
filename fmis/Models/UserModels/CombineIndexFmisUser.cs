using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models.UserModels
{
    public class CombineIndexFmisUser
    {

        public List<FmisUser> Users { get; set; }
        public List<IndexUser> ListUser { get; set; }
        public List<PuUser> PuUser { get; set; }
    }
}
