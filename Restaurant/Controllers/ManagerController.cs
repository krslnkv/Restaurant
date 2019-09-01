using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Restaurant.Dal;
using Restaurant.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

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
                ViewBag.Manager = dbContext.Managers.Include(m => m.User).FirstOrDefault(m => m.UserId == id);
                ViewBag.TodayOrders = dbContext.Orders.Include(o => o.OrderDetails).Include("OrderDetails.Dish").Include(o => o.Shift).Include(o => o.Waiter).Include("Waiter.User").Include(o => o.Table).Where(o => o.Shift.IsClosed == false).ToList();
            }
            return View();
        }

        public async Task<ActionResult> StartManagerShift(int id)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var shiftIsNotClosed = await dbContext.Shifts.FirstOrDefaultAsync(s => s.IsClosed == false);
                if (shiftIsNotClosed!=null)
                    return RedirectToAction("Index", "Manager", new { id = User.Identity.GetUserId() });
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
                var shiftsToClose = await dbContext.Shifts.Where(s=>s.IsClosed==false).ToListAsync();
                if (shiftsToClose!=null)
                {
                    for (int i = 0; i < shiftsToClose.Count; i++)
                    {
                        shiftsToClose[i].ExpDate = DateTime.Now;
                        shiftsToClose[i].IsClosed = true;
                    }
                }
                var waitersIsWorkingNow = await dbContext.Waiters.Where(w=>w.IsWorkingNow==true).ToListAsync();
                if (waitersIsWorkingNow != null)
                {
                    for (int i = 0; i < waitersIsWorkingNow.Count; i++)
                    {
                        waitersIsWorkingNow[i].IsWorkingNow = false;
                    }
                }
                var tablesIsBooked = await dbContext.Tables.Where(t=>t.IsBooked==true).ToListAsync();
                if (tablesIsBooked!=null)
                {
                    for (int i = 0; i < tablesIsBooked.Count; i++)
                    {
                        tablesIsBooked[i].IsBooked = false;
                    }
                }
                await dbContext.SaveChangesAsync();
                return RedirectToAction("Index", "Manager", new { id = manager.UserId });
            }
        }

        [HttpGet]
        public ActionResult CloseOrder(int id)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var order = dbContext.Orders.Include(o => o.Table).FirstOrDefault(o => o.Id == id);
                order.IsActive = false;
                order.Table.IsBooked = false;
                dbContext.SaveChanges();
                return RedirectToAction("Index", "Manager", new { id = User.Identity.GetUserId() });
            }
        }
    }
}