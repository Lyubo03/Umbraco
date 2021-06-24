namespace HighlyDeveloped.Core.Controllers
{
    using HighlyDeveloped.Core.Interfaces;
    using HighlyDeveloped.Core.ViewModels;
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Umbraco.Core.Logging;
    using Umbraco.Web.Mvc;

    public class RegisterController : SurfaceController
    {
        private const string PARTIAL_VIEW_FOLDER = "~/Views/Partials";
        private readonly IEmailService emailService;
        public RegisterController(IEmailService emailService)
        {
            this.emailService = emailService;
        }

        #region Register Form
        public ActionResult RenderRegister()
        {
            var vm = new RegisterViewModel();

            var roles = Services.MemberService.GetAllRoles().Where(x => !x.Contains("Admin"));
            ViewData["userRoles"] = roles;
            return PartialView(PARTIAL_VIEW_FOLDER + "/Register.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HandleRegister(RegisterViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            var existingMember = Services.MemberService.GetByEmail(vm.EmailAddress);
            if (existingMember != null)
            {
                ModelState.AddModelError("Account Error", "There is an user with that email address already");
                return CurrentUmbracoPage();
            }

            existingMember = Services.MemberService.GetByUsername(vm.Username);
            if (existingMember != null)
            {
                ModelState.AddModelError("Account Error", "There is an user with that username already");
                return CurrentUmbracoPage();
            }

            var newMember = Services.MemberService.CreateMember(vm.Username, vm.EmailAddress, $"{vm.FirstName} {vm.LastName}", "Member");
            newMember.PasswordQuestion = "";
            newMember.RawPasswordAnswerValue = "";
            Services.MemberService.Save(newMember);
            Services.MemberService.SavePassword(newMember, vm.Password);

            //Assign Role
            var membersCount = Services.MemberService.Count();
            if (membersCount == 1) Services.MemberService.AssignRole(newMember.Id, "Admin User");

            else if(vm.Role == "Journalist User") Services.MemberService.AssignRole(newMember.Id, "Journalist User");
            
            else Services.MemberService.AssignRole(newMember.Id, "Normal User");

            var token = Guid.NewGuid().ToString();
            newMember.SetValue("emailVerifyToken", token);
            Services.MemberService.Save(newMember);
            try
            {
                 emailService.SendVeerifyEmailAddressNotification(newMember.Email, token);
            }
            catch (Exception ex)
            {
                Logger.Error<RegisterController>("There was an error with register form submission", ex.Message);
                ModelState.AddModelError("Error", "There was an submitting the registration form!");
            }

            TempData["status"] = "OK";

            return RedirectToCurrentUmbracoPage();
        }
        #endregion
        #region Verification
        public ActionResult RenderEmailVerification(string token)
        {
            token = Request.QueryString["token"];
            var member = Services.MemberService.GetMembersByPropertyValue("emailVerifyToken", token).SingleOrDefault();
            if (member != null)
            {
                var alreadyVerified = member.GetValue<bool>("emailVerified");
                if (alreadyVerified)
                {
                    ModelState.AddModelError("Verified", "You've already verified your email address.");
                    return CurrentUmbracoPage();
                }
                member.SetValue("emailVerified", true);
                member.SetValue("emailVerifiedDate", DateTime.UtcNow);
                Services.MemberService.Save(member);

                TempData["status"] = "OK";
                return PartialView(PARTIAL_VIEW_FOLDER + "/EmailVerification.cshtml");
            }
            ModelState.AddModelError("Error", "Apologies there has been some problem!");
            return CurrentUmbracoPage();
        }
        #endregion
    }
}