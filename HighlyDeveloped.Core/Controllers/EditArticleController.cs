namespace HighlyDeveloped.Core.Controllers
{
    using System.Web.Mvc;
    using Umbraco.Core.Services;
    using Umbraco.Web.Mvc;
    public class EditArticleController : SurfaceController
    {
        private readonly IContentService contentService;

        public EditArticleController(IContentService contentService)
        {
            this.contentService = contentService;
        }

        [HttpPost]
        public ActionResult DeleteArticle(int id)
        {
            var content = contentService.GetById(id);

            if (content != null)
            {
                contentService.Delete(content);

                return Redirect("/");
            }
            return RedirectToCurrentUmbracoPage();
        }
    }
}