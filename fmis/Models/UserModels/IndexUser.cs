using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models.UserModels
{
    public class IndexUser
    {
        public int Id { get; set; }
       // [Remote("SaveUsers", "Users", ErrorMessage = "This username is already in use.")]
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
