using Mvc.Mailer;

namespace AppTemplate.Mailers
{ 
    public interface IUserMailer
    {
			MvcMailMessage Register(string to, string username, string tempPassword);
      MvcMailMessage PasswordReset(string to, string tempPassword);
	}
}