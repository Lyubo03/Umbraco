namespace HighlyDeveloped.Core.Controllers
{
    using System.Web.Mvc;
    using Umbraco.Web.Mvc;

    public class EditArticleController : SurfaceController
    {
        public ActionResult DeleteArticle()
        {
            return RedirectToCurrentUmbracoPage();
        }
    }
}