namespace HighlyDeveloped.Core.ViewModels
{
    using System.Collections.Generic;
    using Umbraco.Core.Models.PublishedContent;
    using Umbraco.Web.Models;
    public class SearchContentModel : ContentModel
    {
        public SearchContentModel(IPublishedContent content)
            : base(content)
        {
        }
        public SearchViewModel SearchViewModel { get; set; }
        public IEnumerable<IPublishedContent> SearchResults { get; set; }
    }
}