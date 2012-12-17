using Mvc.Mailer;

namespace AppTemplate.Mailers
{ 
    public class UserMailer : MailerBase, IUserMailer 	
	{
		public UserMailer()
		{
			MasterName="_Layout";
		}

    public virtual MvcMailMessage Register(string to, string username, string tempPassword)
		{
      ViewBag.Username = username;
      ViewBag.TempPassword = tempPassword;
      return Populate(x =>
			{
				x.Subject = "Register";
				x.ViewName = "Register";
				x.To.Add(to);
			});
		}

    public virtual MvcMailMessage PasswordReset(string to, string tempPassword)
		{
      ViewBag.TempPassword = tempPassword;
			return Populate(x =>
			{
				x.Subject = "PasswordReset";
				x.ViewName = "PasswordReset";
        x.To.Add(to);
			});
		}

	}
}