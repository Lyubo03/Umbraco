using System.ComponentModel.DataAnnotations;

namespace HighlyDeveloped.Core.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "Username")]
        [Required]
        public string Username { get; set; }
        [Display(Name = "Password")]
        [Required]
        public string Password { get; set; }
        public string RedirectUrl { get; set; }
    }
}