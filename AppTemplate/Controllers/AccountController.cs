using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;
using AppTemplate.Controllers.FormModels;
using AppTemplate.Data.Infrastructure;
using AppTemplate.Data.Repositories;
using AppTemplate.Mailers;
using AppTemplate.Services;
using AppTemplate.Tasks;

namespace AppTemplate.Controllers {
  public class AccountController : Controller {
    private readonly IUserMailer _userMailer;
    private readonly IFormsAuthentication _formsAuthentication;
    private IAccountTasks _accountTasks;

    public AccountController(IAccountTasks accountTasks, IUserMailer userMailer,
                             IFormsAuthentication formsAuthentication) {
      _accountTasks = accountTasks;
      _userMailer = userMailer;
      _formsAuthentication = formsAuthentication;
    }

    [Authorize]
    public ActionResult Index() {
      return View();
    }

    private string GeneratePassword(int passwordLength) {
      const string allowedChars = "abcdefghijkmnopqrstuvwxyz123456789";
      var randNum = new Random();
      var chars = new char[passwordLength];

      for (int i = 0; i < passwordLength; i++) {
        chars[i] = allowedChars[(int) ((allowedChars.Length)*randNum.NextDouble())];
      }

      return new string(chars);
    }

    protected void AddValidationErrors(IEnumerable<DbEntityValidationResult> errors) {
      AddValidationErrors(errors, string.Empty);
    }

    protected void AddValidationErrors(IEnumerable<DbEntityValidationResult> errors, string prefix) {
      foreach (var error in errors.SelectMany(x => x.ValidationErrors)) {
        ModelState.AddModelError(string.Format("{0}{1}", prefix, error.PropertyName), error.ErrorMessage);
      }
    }

    public ActionResult Login() {
      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Login(string email, string password, bool? rememberMe, string returnUrl) {

      var authenticationResult = _accountTasks.AuthenticateUser(email, password);

      if (authenticationResult == AccountTasks.AuthenticationResult.Success) {

        _formsAuthentication.SignIn(email, rememberMe.GetValueOrDefault());

        if (!String.IsNullOrEmpty(returnUrl)) {
          return Redirect(returnUrl);
        }
        return RedirectToAction("Index", "Home");
      }
      return View();
    }

    [Authorize]
    public ActionResult LogOut() {
      _formsAuthentication.SignOut();

      return RedirectToAction("Login", "Account");
    }

    [Authorize]
    public ActionResult ChangePassword() {
      return View();
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult ChangePassword(ChangePasswordForm changePasswordForm) {

      if (ModelState.IsValid) {
        var changePasswordResult = _accountTasks.ChangePassword(User.Identity.Name, changePasswordForm.CurrentPassword, changePasswordForm.NewPassword);
        if (changePasswordResult == AccountTasks.ChangePasswordResult.Success) {
          TempData["SuccessMessage"] = "Password has been updated";
          return RedirectToAction("Index");
        }
        ModelState.AddModelError("CurrentPassword", "Invalid");
      }
      return View(changePasswordForm);
    }

    public ActionResult ForgotPassword() {
      return View();
    }

    [HttpPost]
    public ActionResult ForgotPassword(ForgotPasswordForm forgotPasswordForm) {
      if (ModelState.IsValid) {
        var tempPassword = _accountTasks.ForgotPassword(forgotPasswordForm.Email);

        if (!string.IsNullOrEmpty(tempPassword)) {
          // Send Email
          _userMailer.PasswordReset(forgotPasswordForm.Email, tempPassword).Send();

          TempData["SuccessMessage"] = "A Temporary Password has been sent to your email";
          return RedirectToAction("Login");

        }
        ModelState.AddModelError("Email", "We can't find this address, you might want to consider <a href=\"/Account/Register/\">Registering an Account.</a>");
      }
      return View();
    }
  }
}
