namespace HighlyDeveloped.Core.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Umbraco.Web.Mvc;
    using Umbraco.Core.Logging;
    using HighlyDeveloped.Core.Interfaces;
    using HighlyDeveloped.Core.ViewModels;
    using Umbraco.Core.Models;

    public class LoginController : SurfaceController
    {
        private const string PARTIAL_VIEW_FOLDER = "~/Views/Partials/Login/";
        private readonly IEmailService emailService;

        public LoginController(IEmailService emailService)
        {
            this.emailService = emailService;
        }

        #region Login
        public ActionResult RenderLogin()
        {
            var vm = new LoginViewModel();
            vm.RedirectUrl = HttpContext.Request.Url.AbsolutePath;

            return PartialView(PARTIAL_VIEW_FOLDER + "Login.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HandleLogin(LoginViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            var member = Services.MemberService.GetByUsername(vm.Username);
            if (member == null)
            {
                ModelState.AddModelError("Login", "Cannot find that username in the system");
                return CurrentUmbracoPage();
            }

            if (member.IsLockedOut)
            {
                ModelState.AddModelError("Login", "The account is locked, please use forgotten password to reset");
                return CurrentUmbracoPage();
            }

            var isEmailVerified = member.GetValue<bool>("emailVerified");
            if (!isEmailVerified)
            {
                ModelState.AddModelError("Login", "Please verify your email before logging in.");
                return CurrentUmbracoPage();
            }
            
            if (!Members.Login(vm.Username, vm.Password))
            {
                ModelState.AddModelError("Login", "The username/password you provided is not correct");
                return CurrentUmbracoPage();
            }

            if (!string.IsNullOrEmpty(vm.RedirectUrl))
            {
                return Redirect(vm.RedirectUrl);
            }
            return Redirect("/");
        }
        #endregion

        #region Forgotten Password
        public ActionResult RenderForgottenPassword()
        {
            var vm = new ForgottenPasswordViewModel();

            return PartialView(PARTIAL_VIEW_FOLDER + "ForgottenPassword.cshtml", vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HandleForgottenPassword(ForgottenPasswordViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            var member = Services.MemberService.GetByEmail(vm.EmailAddress);
            if (member == null)
            {
                ModelState.AddModelError("Forgotten Password", "Member with such an email doesn not exist!");
                return CurrentUmbracoPage();
            }

            var resetToken = Guid.NewGuid().ToString();

            member.SetValue("resetLinkToken", resetToken);
            member.SetValue("resetExpiryDate", DateTime.UtcNow.AddMinutes(30));
            Services.MemberService.Save(member);

            emailService.SendResetPasswordNotification(member.Email, resetToken);

            Logger.Info<LoginController>($"Send a password reset to {vm.EmailAddress}");

            TempData["status"] = "OK";

            return RedirectToCurrentUmbracoPage();
        }
        #endregion

        #region Reset Password
        public ActionResult RenderResetPassword()
        {
            var vm = new ResetPasswordViewModel();
            return PartialView(PARTIAL_VIEW_FOLDER + "ResetPassword.cshtml", vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HandleResetPassword(ResetPasswordViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }
            
            var resetToken = Request.QueryString["token"];
            if (string.IsNullOrEmpty(resetToken) )
            {
                Logger.Warn<LoginController>("Reset Password - no token found");
                ModelState.AddModelError("Error","Invalid Token");
                return CurrentUmbracoPage();
            }

            var member = Services.MemberService.GetMembersByPropertyValue("resetLinkToken", resetToken).SingleOrDefault() as Member;
            if (member == null)
            {
                ModelState.AddModelError("Error", "That link is no longer valid");
                return CurrentUmbracoPage();
            }

            var expiryDate = member.GetValue<DateTime>("resetExpiryDate");
            var currentTime = DateTime.UtcNow;
            if (expiryDate.CompareTo(currentTime) < 0)
            {
                ModelState.AddModelError("Error", "Sorry the link has expired, please use the Forgotten Password page to generate a new one");
                return CurrentUmbracoPage();
            }

            Services.MemberService.SavePassword(member, vm.Password);
            member.SetValue("resetLinkToken", string.Empty);
            member.SetValue("resetExpiryDate", string.Empty);
            member.IsLockedOut = false;
            Services.MemberService.Save(member);

            emailService.SendPasswordChangedNotification(member.Email);
            
            TempData["status"] = "OK";
            Logger.Info<LoginController>($"User {member.Username} has changed their password.");
            
            return RedirectToCurrentUmbracoPage();
        }
        #endregion
    }
}