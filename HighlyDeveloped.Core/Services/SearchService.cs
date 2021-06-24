namespace HighlyDeveloped.Core.Services
{
    using System;
    using Examine.Search;
    using Examine;
    using HighlyDeveloped.Core.Interfaces;
    using System.Collections.Generic;
    using Umbraco.Core.Models.PublishedContent;
    using System.Linq;
    using Umbraco.Web;
    using Lucene.Net.Analysis;
    using HighlyDeveloped.Core.Extensions;
    using static Umbraco.Core.Constants;

    public class SearchService : ISearchService
    {
        private readonly IUmbracoContextAccessor umbracoContextAccessor;

        public SearchService(IUmbracoContextAccessor umbracoContextAccessor)
        {
            this.umbracoContextAccessor = umbracoContextAccessor;
        }
        public IEnumerable<IPublishedContent> GetPageOfContentSearchResults(string searchTerm, string category, int pageNumber, out long totalItemCount, string[] docTypeAliases, int pageSize = 10)
        {
            var pageOfResults = GetPageOfSearchResults(searchTerm, category, pageNumber, out totalItemCount, null, "content");

            var items = new List<IPublishedContent>();
            if (pageOfResults != null && pageOfResults.Any())
            {
                foreach (var item in pageOfResults)
                {
                    var page = umbracoContextAccessor.UmbracoContext.Content
                        .GetById(int.Parse(item.Id));
                    if (page != null)
                    {
                        items.Add(page);
                    }
                }
            }

            return items;
        }

        public IEnumerable<ISearchResult> GetPageOfSearchResults(string searchTerm, string category, int pageNumber, out long totalItemCount, string[] docTypeAliases, string searchType, int pageSize = 10)
        {
            int skip = pageNumber > 1 ? (pageNumber - 1) * pageSize : 0;

            string[] terms = !string.IsNullOrEmpty(searchTerm) && searchTerm.Contains(" ")
                ? searchTerm.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                : !string.IsNullOrWhiteSpace(searchTerm) ? new string[] { searchTerm } : null;

            if (terms != null && terms.Any() && ExamineManager.Instance.TryGetIndex("ExternalIndex", out var index))
            {
                terms = terms.Where(x => !StopAnalyzer.ENGLISH_STOP_WORDS_SET.Contains(x.ToLower()) &&
                    x.Length > 2).ToArray();

                var searcher = index.GetSearcher();
                var criteria = searcher.CreateQuery(searchType);
                /*var query = criteria.GroupedNot(new string[] { "umbracoNaviHide" },
                    new string[] { "1" });*/
                
                var query = criteria.GroupedOr(new string[] { "__NodeTypeAlias" },
                    new string[] { "newsArticles", "newsArticle", "searchPage", "login", "about", "register", "home", "contentPage" });
                    /*.Or()
                    .GroupedOr(new string[] { "__NodeTypeAlias" },);*/

                if (docTypeAliases != null && docTypeAliases.Any())
                {
                    query.And(q => q.GroupedOr(new[] { "__NodeTypeAlias" }, docTypeAliases));
                }

                if (!string.IsNullOrWhiteSpace(category))
                {
                    query.And().Field("searchableCategories", category);
                }

                var allResults = query.Execute();

                totalItemCount = allResults.TotalItemCount;

                var pageOfResults = allResults.Skip(skip).Take(pageSize);

                return pageOfResults;
            }

            totalItemCount = 0;
            return Enumerable.Empty<ISearchResult>();
        }
    }
}