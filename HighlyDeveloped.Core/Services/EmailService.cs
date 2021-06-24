namespace HighlyDeveloped.Core.Services
{
    using Umbraco.Core.Logging;
    using HighlyDeveloped.Core.Interfaces;
    using System;
    using System.Linq;
    using Umbraco.Core.Models.PublishedContent;
    using Umbraco.Core.Services;
    using Umbraco.Web;
    using HighlyDeveloped.Core.ViewModels;
    using System.Web;
    using System.Net.Mail;

    public class EmailService : IEmailService
    {
        private UmbracoHelper _umbraco;
        private IContentService _contentService;
        private ILogger _logger;

        public EmailService(UmbracoHelper umbraco, IContentService contentService, ILogger iLogger)
        {
            _umbraco = umbraco;
            _contentService = contentService;
            _logger = iLogger;
        }

        /// <summary>
        /// Sending of the email to an admin when a new contact form comes in
        /// </summary>
        /// <param name="vm"></param>
        public void SendContactNotificationToAdmin(ContactFormViewModel vm)
        {
            //Get email template from Umbraco for "this" notification is
            var emailTemplate = GetEmailTemplate("New Contact Form Notification");
            if (emailTemplate == null)
            {
                throw new Exception("Template not found");
            }

            //Get the template data
            var subject = emailTemplate.Value<string>("emailTemplateSubjectLine");
            var htmlContent = emailTemplate.Value<string>("emailTemplateHtmlContent");
            var textContent = emailTemplate.Value<string>("emailTemplateTextContent");

            //Mail merge the necessary fields
            //#name##
            MailMerge("name", vm.Name, ref htmlContent, ref textContent);
            
            //#email##
            MailMerge("email", vm.Email, ref htmlContent, ref textContent);
            
            //#comment##
            MailMerge("comment", vm.Comments, ref htmlContent, ref textContent);
            //Send email out to whoever
            //Read email FROM and TO addresses
            //Get site settings

            SendEmail(vm.Email, subject, htmlContent, textContent);
        }

        public void SendVeerifyEmailAddressNotification(string memberEmail, string verificationToken)
        {
            //Get Template
            var emailTemplate = GetEmailTemplate("Verify Email");

            if (emailTemplate == null)
            {
                throw new Exception("Template not found");
            }

            var subject = emailTemplate.Value<string>("emailTemplateSubjectLine");
            var htmlContent = emailTemplate.Value<string>("emailTemplateHtmlContent");
            var textContent = emailTemplate.Value<string>("emailTemplateTextContent");
            
            //Build the url to be the absolute url to the verify page
            var url = HttpContext.Current
                .Request
                .Url
                .AbsoluteUri
                .Replace(HttpContext.Current.Request.Url.AbsolutePath, string.Empty)
                + $"/verify?token={verificationToken}";

            MailMerge("verify-url", url, ref htmlContent, ref textContent);

            //Log the Email
            SendEmail(memberEmail, subject, htmlContent, textContent);
        }
        
        private void MailMerge(string token, string value, ref string htmlContent, ref string textContent)
        {
            htmlContent = htmlContent.Replace($"##{token}##", value);
            textContent = htmlContent.Replace($"##{token}##", value);
        }

        /// <summary>
        /// Returns the email template as IPublishedContent where the title matches the template name
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns></returns>
        private IPublishedContent GetEmailTemplate(string templateName)
        {
            var template = _umbraco.ContentAtRoot()
                                .DescendantsOrSelfOfType("emailTemplate")
                                .Where(w => w.Name == templateName)
                                .FirstOrDefault();

            return template;
        }

        public void SendResetPasswordNotification(string memberEmail, string resetToken)
        {
            var emailTemplate = GetEmailTemplate("Reset Password");
            if (emailTemplate == null)
            {
                throw new Exception("Template not found");
            }

            var subject = emailTemplate.Value<string>("emailTemplateSubject");
            var htmlContent = emailTemplate.Value<string>("emailTemplateHtmlContent");
            var textContent = emailTemplate.Value<string>("emailTemplateTextContent");

            var url = HttpContext.Current
                .Request
                .Url
                .AbsoluteUri
                .Replace(HttpContext.Current.Request.Url.AbsolutePath, string.Empty)
                + $"/reset-password?token={resetToken}";

            MailMerge("reset-url", url, ref htmlContent, ref textContent);
            SendEmail(memberEmail, subject, htmlContent, textContent);
        }

        public void SendPasswordChangedNotification(string memberEmail)
        {
            var emailTemplate = GetEmailTemplate("Password Changed");
            if (emailTemplate == null)
            {
                throw new Exception("Template not found");
            }

            var subject = emailTemplate.Value<string>("emailTemplateSubject");
            var htmlContent = emailTemplate.Value<string>("emailTemplateHtmlContent");
            var textContent = emailTemplate.Value<string>("emailTemplateTextContent");

            SendEmail(memberEmail, subject, htmlContent, textContent);
        }

        private void SendEmail(string toAddresses, string subject, string htmlContent, string textContent)
        {
            var siteSettings = _umbraco.ContentAtRoot().DescendantsOrSelfOfType("siteSettings").FirstOrDefault();
            if (siteSettings == null)
            {
                throw new Exception("There are no site settings");
            }

            var fromAddress = siteSettings.Value<string>("emailSettingsFromAddress");
            if (string.IsNullOrEmpty(fromAddress))
            {
                throw new Exception("There needs to be a from address in site settings");
            }

            //Debug Mode
            var debugMode = siteSettings.Value<bool>("testMode");
            var testEmailAccounts = siteSettings.Value<string>("emailTestAccounts");

            var recipients = toAddresses;

            if (debugMode)
            {
                //Change the To - testEmailAccounts
                recipients = testEmailAccounts;
                //Alter subject line - to show in test mode
                subject += "(TEST MODE) - " + toAddresses;
            }

            //Log the email in umbraco
            var emails = _umbraco.ContentAtRoot().DescendantsOrSelfOfType("emails").FirstOrDefault();
            var newEmail = _contentService.Create(toAddresses, emails.Id, "Email");
            newEmail.SetValue("emailSubject", subject);
            newEmail.SetValue("emailToAddress", recipients);
            newEmail.SetValue("emailHtmlContent", htmlContent);
            newEmail.SetValue("emailTextContent", textContent);
            newEmail.SetValue("emailSent", false);
            _contentService.SaveAndPublish(newEmail);


            var mimeType = new System.Net.Mime.ContentType("text/html");
            var alternateView = AlternateView.CreateAlternateViewFromString(htmlContent, mimeType);

            var smtpMessage = new MailMessage();
            smtpMessage.AlternateViews.Add(alternateView);

            foreach (var recipient in recipients.Split(','))
            {
                if (!string.IsNullOrEmpty(recipient))
                {
                    smtpMessage.To.Add(recipient);
                }
            }

            smtpMessage.From = new MailAddress(fromAddress);
            smtpMessage.Subject = subject;
            smtpMessage.Body = textContent;
            
            using (var smtp = new SmtpClient())
            {
                try
                {
                    smtp.EnableSsl = true;
                    smtp.Send(smtpMessage);
                    newEmail.SetValue("emailSent", true);
                    newEmail.SetValue("emailSentDate", DateTime.Now);
                    _contentService.SaveAndPublish(newEmail);
                }
                catch (Exception exc)
                {
                    //Log the error
                    _logger.Error<EmailService>("Problem sending the email", exc);
                    throw exc;
                }
            }
        }
    }
}