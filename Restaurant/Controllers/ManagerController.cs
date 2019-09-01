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

        public async Task<ActionResult> OrdersList()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var waitersViewModel = await dbContext.Waiters.Select(w => new WaiterViewModel()
                {
                    Id = w.Id,
                    //Так как Linq to entities не может распознать String.Format используется конкатинация
                    WaiterName = w.User.Name+" "+w.User.LastName
                }).ToListAsync();
                waitersViewModel.Add(new WaiterViewModel { Id = 0, WaiterName = "Не выбрано" });
                ViewBag.Waiters = new SelectList(waitersViewModel, "Id", "WaiterName");
                ViewBag.Orders = await dbContext.Orders.Include(o => o.OrderDetails).Include("OrderDetails.Dish").Include(o => o.Shift).Include(o => o.Waiter).Include("Waiter.User").Include(o => o.Table).ToListAsync();
                return View();
            }
        }

        [HttpPost]
        public async Task<ActionResult> OrdersList(OrderFiltration filtration)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var waitersViewModel = await dbContext.Waiters.Select(w => new WaiterViewModel()
                {
                    Id = w.Id,
                    //Так как Linq to entities не может распознать String.Format используется конкатинация
                    WaiterName = w.User.Name + " " + w.User.LastName
                }).ToListAsync();
                waitersViewModel.Add(new WaiterViewModel { Id = 0, WaiterName = "Не выбрано" });
                ViewBag.Waiters = new SelectList(waitersViewModel, "Id", "WaiterName");
                var orders = await dbContext.Orders.Include(o => o.OrderDetails).Include("OrderDetails.Dish").Include(o => o.Shift).Include(o => o.Waiter).Include("Waiter.User").Include(o => o.Table).ToListAsync();
                if (filtration.IsActive == 0)
                    orders = orders.Where(o => o.IsActive == false).ToList();
                if (filtration.IsActive == 1)
                    orders = orders.Where(o => o.IsActive == true).ToList();
                if (filtration.WaiterId > 0)
                    orders = orders.Where(o => o.WaiterId == filtration.WaiterId).ToList();
                if (filtration.OrderDate != null)
                    orders = orders.Where(o => o.OrderTime.Date == (DateTime)filtration.OrderDate.Value.Date).ToList();
                ViewBag.Orders = orders;
                return View();
            }
        }
    }
}