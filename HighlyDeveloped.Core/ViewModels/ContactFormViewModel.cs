namespace HighlyDeveloped.Core.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class ContactFormViewModel
    {
        [Required]
        [MaxLength(80, ErrorMessage = "Please try and limit to 80 characters")]
        public string Name { get; set; }
        [Required]
        [EmailAddress (ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }
        [Required]
        [MaxLength(500, ErrorMessage = "Please try and limit to 500 characters")]
        public string Comments { get; set; }
        [MaxLength(255, ErrorMessage = "Please try and limit to 255 characters")]
        public string Subject { get; set; }
        public string RecaptchaSiteKey { get; set; }
    }
}