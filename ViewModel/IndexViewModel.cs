using System.ComponentModel.DataAnnotations;

namespace fmis.ViewModel
{
	public class IndexViewModel
	{
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        //[Display(Name = "Remember me")]
        public bool RememberMe { get; set; } = false;

        public string ReturnUrl { get; set; }
    }
}
