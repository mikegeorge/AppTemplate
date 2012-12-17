using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppTemplate.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

      [SimpleAuthorize]
        public ActionResult Members() {
          return Content("Members Only!");
        }
      [SimpleAuthorize(Roles="editor,administrator")]
        public ActionResult Editors() {
          return Content("Editors Only!");
        }
      [SimpleAuthorize(Roles = "administrator")]
        public ActionResult Admin() {
          return Content("Admin Only!");
        }
    }
}
