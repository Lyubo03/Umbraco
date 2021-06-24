namespace HighlyDeveloped.Core.Interfaces
{
    using HighlyDeveloped.Core.ViewModels;

    public interface IEmailService
    {
        void SendContactNotificationToAdmin(ContactFormViewModel vm);
        void SendVeerifyEmailAddressNotification(string memberEmail, string verificationToken);
        void SendResetPasswordNotification(string memberEmail, string resetToken);
        void SendPasswordChangedNotification(string memberEmail);
    }
}