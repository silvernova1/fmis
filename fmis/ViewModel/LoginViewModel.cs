﻿using System.ComponentModel.DataAnnotations;

namespace fmis.ViewModel
{
    public class LoginViewModel
    {

        [Required]
        public string Name { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        [Required(ErrorMessage = "You must enter your username")]
        [Display(Name = "Email")]
        public string Username { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}