namespace HighlyDeveloped.Core.Controllers
{
    using System.Web.Mvc;
    using Umbraco.Web.Mvc;
    using HighlyDeveloped.Core.ViewModels;
    using System.Web.Security;
    using Umbraco.Core.Models;

    public class AccountController : SurfaceController
    {
        private const string PARTIAL_VIEW_FOLDER = "~/Views/Partials/MyAccount/";
        [HttpGet]
        public ActionResult RenderMyAccount()
        {
            var vm = new AccountViewModel();

            if (!Umbraco.MemberIsLoggedOn())
            {
                ModelState.AddModelError("Error", "You are not currently logged in.");
                return CurrentUmbracoPage();
            }

            //Built in membership provider in ASP.NET
            var member = Services.MemberService.GetByUsername(Membership.GetUser().UserName) as Member;
            if (member == null)
            {
                ModelState.AddModelError("Error", "We could not find you in the system");
                return CurrentUmbracoPage();
            }
            vm.Name = member.Name;
            vm.Email = member.Email;
            vm.Username = member.Username;

            return PartialView(PARTIAL_VIEW_FOLDER + "MyAccount.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HandleUpdateDetails(AccountViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            if (!Umbraco.MemberIsLoggedOn() || Membership.GetUser() == null)
            {
                ModelState.AddModelError("Error", "You are not logged on.");
                return CurrentUmbracoPage();
            }

            var member = Services.MemberService.GetByUsername(Membership.GetUser().UserName);
            if (member == null)
            {
                ModelState.AddModelError("Error", "You are not logged on.");
                return CurrentUmbracoPage();
            }

            member.Username = vm.Username;
            member.Email = vm.Email;

            Services.MemberService.Save(member);
            TempData["status"] = "Updated Details";
            return RedirectToCurrentUmbracoPage();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HandleUpdatePassword(AccountViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            if (!Umbraco.MemberIsLoggedOn() || Membership.GetUser() == null)
            {
                ModelState.AddModelError("Error", "You are not logged on.");
                return CurrentUmbracoPage();
            }

            var member = Services.MemberService.GetByUsername(Membership.GetUser().UserName);
            if (member == null)
            {
                ModelState.AddModelError("Error", "You are not logged on.");
                return CurrentUmbracoPage();
            }
            try
            {
                if (vm.Password != null)
                {
                    Services.MemberService.SavePassword(member, vm.Password);
                }
            }
            catch (MembershipPasswordException ex)
            {
                ModelState.AddModelError("Error", "There is a problem with your password");
                return CurrentUmbracoPage();
            }

            TempData["status"] = "Updated Password";
            return RedirectToCurrentUmbracoPage();
        }
    }
}