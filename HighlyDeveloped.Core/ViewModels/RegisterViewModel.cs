namespace HighlyDeveloped.Core.ViewModels
{
    using DataAnnotationsExtensions;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Please enter your first name")]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter your last name")]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please choose a username")]
        [DisplayName("Username")]
        [MinLength(6)]
        public string Username { get; set; }
        [Required(ErrorMessage = "Please enter your email")]
        [DisplayName("Email")]
        public string EmailAddress { get; set; }

        [UIHint("Password")]
        [Required(ErrorMessage = "Please enter your password")]
        [DisplayName("Password")]
        [MinLength(8, ErrorMessage = "Please make your password at least 8 characters")]
        public string Password { get; set; }
        [UIHint("ConfirmPassword")]
        [Required(ErrorMessage = "Please enter your password confirmation")]
        [DisplayName("ConfirmPassword")]
        [EqualTo("ConfirmPassword", ErrorMessage = "Please ensure your passwords match")]

        public string ConfirmPassword { get; set; }
        [DisplayName("Role")]
        public string Role { get; set; }
    }
}