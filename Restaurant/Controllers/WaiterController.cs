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
    public class WaiterController : Controller
    {
        // GET: Waiter
        [HttpGet]
        public ActionResult Index(string id)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                ViewBag.ManagerNotFound = TempData["ManagerNotFound"];
                var waiter = dbContext.Waiters.Include(w=>w.User).FirstOrDefault(w => w.UserId == id);
                ViewBag.Waiter = waiter;
            }
            return View();
        }

        public async Task<ActionResult> StartWaiterShift(int id)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var startTime = DateTime.UtcNow;
                var waiter = await dbContext.Waiters.Include(w => w.User).FirstOrDefaultAsync(w => w.Id == id);
                var manager = await dbContext.Managers.Include(m => m.User).FirstOrDefaultAsync(m => m.IsWorkingNow == true);
                if (manager == null)
                {
                    TempData["ManagerNotFound"] = true;
                    return RedirectToAction("Index", "Waiter", new { id = waiter.UserId });
                }
                var shift = new Shift { Manager = manager, Waiter = waiter, StartDate = startTime };
                dbContext.Shifts.Add(shift);
                waiter.IsWorkingNow = true;
                waiter.LastShift = startTime;
                await dbContext.SaveChangesAsync();
                return RedirectToAction("Index", "Waiter", new { id = waiter.UserId });
            }
        }

        public async Task<ActionResult> StopWaiterShift(int id)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var waiter = await dbContext.Waiters.Include(w => w.User).FirstOrDefaultAsync(w => w.Id == id);
                var shift = await dbContext.Shifts.Include(s => s.Waiter).Include("Waiter.User").FirstOrDefaultAsync(s => s.WaiterId == id&&s.StartDate==s.Waiter.LastShift);
                waiter.IsWorkingNow = false;
                shift.ExpDate = DateTime.UtcNow;
                shift.IsClosed = true;
                await dbContext.SaveChangesAsync();
                return RedirectToAction("Index", "Waiter", new { id = waiter.UserId });
            }
        }
    }
}