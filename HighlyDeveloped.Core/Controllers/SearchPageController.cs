namespace HighlyDeveloped.Core.Controllers
{
    using System.Web.Mvc;
    using Umbraco.Web.Mvc;
    using Umbraco.Web.Models;
    using HighlyDeveloped.Core.ViewModels;
    using HighlyDeveloped.Core.Interfaces;
    //RMC tied to the views

    public class SearchPageController : RenderMvcController
    {
        private readonly ISearchService searchService;
        public SearchPageController(ISearchService searchService)
        {
            this.searchService = searchService;
        }
        public ActionResult Index(ContentModel model, string query, string page, string category)
        {
            var searchModel = new SearchContentModel(model.Content);

            var viewModel = new SearchViewModel()
            {
                Query = query,
                Category = category,
                Page = page
            };

            searchModel.SearchViewModel = viewModel;

            if (!int.TryParse(page, out var pageNumber))
            {
                pageNumber = 1;
            }

            var searchResults = searchService
                .GetPageOfContentSearchResults(query, category, 
                pageNumber, out var totalItemCount, null);

            searchModel.SearchResults = searchResults;
            searchModel.SearchViewModel = viewModel;

            return CurrentTemplate(searchModel);
        }
    }
}