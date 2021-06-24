namespace HighlyDeveloped.Core.Controllers
{
    using HighlyDeveloped.Core.ViewModels;
    using Umbraco.Core.Logging;
    using System;
    using System.Web.Mvc;
    using Umbraco.Web.Mvc;
    using Umbraco.Web;
    using System.Linq;
    using Umbraco.Core.Services;
    using Umbraco.Core;
    using System.Collections.Generic;
    using Umbraco.Core.Models;

    public class WriteArticleController : SurfaceController
    {
        int folderContentId = 1294;
        string PARTIAL_VIEW_PATH = "~/Views/Partials/";
        IContentTypeBaseServiceProvider contentTypeBaseServiceProvider;
        
        public WriteArticleController(IContentTypeBaseServiceProvider contentTypeBaseServiceProvider)
        {
            this.contentTypeBaseServiceProvider = contentTypeBaseServiceProvider;
        }
       
        public ActionResult RenderWriteArticle()
        {
            var vm = new WriteArticleViewModel();
            return PartialView(PARTIAL_VIEW_PATH + "WriteArticle.cshtml", vm);
        }

        [HttpPost]
        public ActionResult HandleWriteArticle(WriteArticleViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            if (!IsValidContentType(vm.ArticleImage.ContentType))
            {
                ModelState.AddModelError("Error", "Only JPG, JPEG, PNG & GIF files are allowed.");
                return CurrentUmbracoPage();
            }

            try
            {
                //TODO: Add related articles! Important
                //What you need to take in order to fulfill it
                var newsArticles = Umbraco.ContentAtRoot().DescendantsOrSelfOfType("newsArticles").FirstOrDefault();
                var createdArticles = Umbraco.ContentAtRoot().DescendantsOrSelfOfType("newsArticle");

                if (newsArticles != null)
                {
                    var newsArticle = Services.ContentService.Create(vm.Title, newsArticles.Id, "newsArticle");

                    var folderContent = Services.MediaService.GetById(folderContentId);

                    var attachedMedia = Services.MediaService.CreateMediaWithIdentity(vm.ArticleImage.FileName, folderContent.Id, "file");
                    attachedMedia.SetValue(contentTypeBaseServiceProvider, "umbracoFile", vm.ArticleImage.FileName, vm.ArticleImage.InputStream);
                    Services.MediaService.Save(attachedMedia);

                    newsArticle.SetValue("author", vm.Auhtor);
                    newsArticle.SetValue("postDate", vm.PostDate);
                    newsArticle.SetValue("leadIn", vm.LeadIn);
                    newsArticle.SetValue("articleImage", attachedMedia.GetUdi());
                    newsArticle.SetValue("articleContent", vm.ArticleContent);

                    var tagsSplited = vm.Tags
                        .Split(new[] { ' ', '#', ',' }, StringSplitOptions.RemoveEmptyEntries).ToArray();

                    if (tagsSplited != null)
                    {
                        newsArticle.AssignTags("newsCategories", tagsSplited, true);
                    }

                    var relatedArticles = new List<string>();

                    foreach (var article in createdArticles)
                    {
                        var property = article.Properties.SingleOrDefault(x => x.Alias == "newsCategories").Value() as IEnumerable<string>;
                        
                        foreach (var prop in property)
                        {
                            if (tagsSplited.Contains(prop))
                            {
                                relatedArticles.Add($"umb://document/{article.Key.ToString()}");
                            }
                        }
                    }
                    newsArticle.SetValue("articleRelatedContent", string.Join(",", relatedArticles.Take(3)));
                    Services.ContentService.SaveAndPublish(newsArticle);
                }

                return RedirectToCurrentUmbracoPage();
            }
            catch (Exception ex)
            {
                Logger.Error<WriteArticleController>("There was an error in the article submission", ex.Message);
                ModelState.AddModelError("Error", "Sorry there was a problem noting your details. Would you please try again later?");
            }

            return CurrentUmbracoPage();
        }
        private bool IsValidContentType(string contentType)
        {
            return contentType.Equals("image/png") || contentType.Equals("image/gif") ||
                contentType.Equals("image/jpg") || contentType.Equals("image/jpeg");
        }
        private bool IsValidContentLenght(int contentLenght)
        {
            return (contentLenght / 1024 / 1024) < 1;
        }
    }
}