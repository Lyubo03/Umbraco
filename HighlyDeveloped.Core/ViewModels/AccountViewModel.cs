using DataAnnotationsExtensions;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HighlyDeveloped.Core.ViewModels
{
    public class AccountViewModel
    {
        [DisplayName("Full Name")]
        [Required(ErrorMessage = "Please enter your name")]
        public string Name { get; set; }
        public string Username { get; set; }
        [DisplayName("Email ")]
        [Email(ErrorMessage = "Please enter your email address")]
        [Required(ErrorMessage = "Please enter your email address")]
        public string Email { get; set; }
        [UIHint("Password")]
        [DisplayName("Password")]
        [MinLength(8, ErrorMessage = "Please make your password at least 8 characters")]
        public string Password { get; set; }
        [UIHint("ConfirmPassword")]
        [DisplayName("ConfirmPassword")]
        [EqualTo("ConfirmPassword", ErrorMessage = "Please ensure your passwords match")]
        [MinLength(8, ErrorMessage = "Please make your password at least 8 characters")]
        public string ConfirmPassword { get; set; }
    }
}