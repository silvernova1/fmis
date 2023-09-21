using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models.UserModels
{
    public class CombineIndexFmisUser
    {

        public List<UserModels.FmisUser> Users { get; set; }
        public List<UserModels.IndexUser> ListUser { get; set; }
    }
}
