using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AppTemplate.Controllers.FormModels;
using AppTemplate.Domain;
using AppTemplate.Mailers;
using AppTemplate.Tasks;

namespace AppTemplate.Controllers {
  [SimpleAuthorize(Roles = "administrator")]
  public class UsersController : Controller {

    private readonly IUserMailer _userMailer;
    private readonly IUserTasks _userTasks;
    private readonly IAccountTasks _accountTasks;

    public UsersController(IUserTasks userTasks, IUserMailer userMailer, IAccountTasks accountTasks) {
      _userTasks = userTasks;
      _userMailer = userMailer;
      _accountTasks = accountTasks;
    }

    public ActionResult Index() {
      var userList = _userTasks.GetList();
      return View(userList);
    }

    //
    // GET: /Users/Details/5

    public ActionResult Details(int id) {
      var user = _userTasks.GetUser(id);
      return View(user);
    }

    //
    // GET: /Users/Edit/5

    public ActionResult Edit(int id = 0) {
      var user = _userTasks.GetUser(id);
      return View(user);
    }

    //
    // POST: /Users/Edit/5

    [HttpPost]
    public ActionResult Edit(int id, User userFromForm) {

      if (ModelState.IsValid) {
        var user = _userTasks.UpdateUser(id, userFromForm);

        TempData["SuccessMessage"] = string.Format("{0} has been updated", user.Email);
        return RedirectToAction("Index");
      }
      return View(userFromForm);
    }

    //
    // GET: /Users/Delete/5

    public ActionResult Delete(int id = 0) {
      var user = _userTasks.GetUser(id);
      return View(user);
    }

    //
    // POST: /Users/Delete/5

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id) {
      _userTasks.DeleteUser(id);
      return RedirectToAction("Index");
    }

    [Authorize]
    public ActionResult Register() {
      return View();
    }

    [Authorize]
    [HttpPost]
    public ActionResult Register(RegisterForm registerForm) {
      if (ModelState.IsValid) {

        var registerUserResult = _accountTasks.RegisterUser(registerForm.Username, registerForm.Email);
        switch (registerUserResult.RegistrationRequestResult) {
          case AccountTasks.RegistrationRequestResult.UsernameTaken:
            ModelState.AddModelError("Username", "Username is taken. Please choose another");
            break;
          case AccountTasks.RegistrationRequestResult.EmailRegistered:
            ModelState.AddModelError("Email", "Email is already registered");
            break;
          default:
            _userMailer.Register(registerForm.Email, registerForm.Username, registerUserResult.TempPassword).Send();

            TempData["SuccessMessage"] = "Account has been registered";
            return RedirectToAction("Index");
        }

      }
      return View(registerForm);
    }

    protected void AddValidationErrors(IEnumerable<DbEntityValidationResult> errors) {
      AddValidationErrors(errors, string.Empty);
    }

    protected void AddValidationErrors(IEnumerable<DbEntityValidationResult> errors, string prefix) {
      foreach (var error in errors.SelectMany(x => x.ValidationErrors)) {
        ModelState.AddModelError(string.Format("{0}{1}", prefix, error.PropertyName), error.ErrorMessage);
      }
    }
  }

}