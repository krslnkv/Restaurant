using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Restaurant.Dal;
using Restaurant.Models;
using System.Threading.Tasks;

namespace Restaurant.Controllers
{
    public class ManagerController : Controller
    {
        // GET: Manager
        [HttpGet]
        public ActionResult Index(string id)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var manager = dbContext.Managers.Include(m => m.User).FirstOrDefault(m => m.UserId == id);
                ViewBag.Manager = manager;
            }
            return View();
        }

        public async Task<ActionResult> StartManagerShift(int id)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var manager = await dbContext.Managers.Include(m => m.User).FirstOrDefaultAsync(m => m.Id == id);
                manager.IsWorkingNow = true;
                var shift = new Shift { Manager = manager, StartDate = DateTime.Now };
                dbContext.Shifts.Add(shift);
                await dbContext.SaveChangesAsync();
                return RedirectToAction("Index", "Manager", new { id = manager.UserId });
            }
        }
        public async Task<ActionResult> StopManagerShift(int id)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var manager = await dbContext.Managers.Include(m => m.User).FirstOrDefaultAsync(m => m.Id == id);
                manager.IsWorkingNow = false;
                var shift = await dbContext.Shifts.Include(s => s.Manager).OrderByDescending(s => s.Id).FirstOrDefaultAsync(s => s.ManagerId == id);
                shift.ExpDate = DateTime.UtcNow;
                shift.IsClosed = true;
                await dbContext.SaveChangesAsync();
                return RedirectToAction("Index", "Manager", new { id = manager.UserId });
            }
        }
    }
}