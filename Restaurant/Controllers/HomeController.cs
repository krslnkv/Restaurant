using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace Restaurant.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var id = User.Identity.GetUserId();
            if (User.IsInRole("waiter"))
                return RedirectToAction("Index", "Waiter", new { id });
            if (User.IsInRole("manager"))
                return RedirectToAction("Index", "Manager", new { id });
            return View();
        }
    }
}