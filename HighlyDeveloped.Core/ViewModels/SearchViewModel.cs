namespace HighlyDeveloped.Core.ViewModels
{
    using System.Collections;
    using System.ComponentModel.DataAnnotations;

    public class SearchViewModel
    {
        [Display(Name = "Search Term")]
        [Required(ErrorMessage = "You must eneter a search term")]
        public string Query{ get; set; }
        [Display(Name = "Category")]
        public string Category { get; set; }
        [Display(Name = "Page Number")]
        public string Page { get; set; }
        public IEnumerable PagedResults { get; set; }
    }
}