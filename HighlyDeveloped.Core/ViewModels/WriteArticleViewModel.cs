namespace HighlyDeveloped.Core.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web;
    // TODO: Validation pls! :D
    public class WriteArticleViewModel
    {
        [DisplayName("Title")]
        [MinLength(6)]
        [MaxLength(100)]
        [Required]
        public string Title { get; set; }

        [DisplayName("Auhtor")]
        [MinLength(6)]
        [MaxLength(50)]
        [Required]
        public string Auhtor { get; set; }

        [DisplayName("Post Date")]
        [Required]
        public DateTime PostDate { get; set; }

        [DisplayName("Lead In")]
        [Required]
        [MinLength(3)]
        [MaxLength(250)]
        public string LeadIn { get; set; }

        [DisplayName("Article Image")]
        [Required]
        public HttpPostedFileBase ArticleImage { get; set; }

        [DisplayName("Article Content")]
        [Required]
        [MinLength(10)]
        [MaxLength(2500)]
        public string ArticleContent { get; set; }

        [DisplayName("Tags")]
        public string Tags { get; set; }
    }
}