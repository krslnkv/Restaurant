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
    [Authorize (Roles = "manager")]
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
                if (manager != null)
                {
                    manager.IsWorkingNow = true;
                    var shift = new Shift { Manager = manager, StartDate = DateTime.Now };
                    dbContext.Shifts.Add(shift);
                    await dbContext.SaveChangesAsync();
                    return RedirectToAction("Index", "Manager", new { id = manager.UserId });
                }
                return RedirectToAction("Index", "Home");
            }
        }
        public async Task<ActionResult> StopManagerShift(int id)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var manager = await dbContext.Managers.Include(m => m.User).FirstOrDefaultAsync(m => m.Id == id);
                if (manager!=null) 
                    manager.IsWorkingNow = false;
                var shiftsToClose = await dbContext.Shifts.Where(s=>s.IsClosed==false).ToListAsync();
                if (shiftsToClose.Count!=0)
                {
                    foreach (var s in shiftsToClose)
                    {
                        s.ExpDate = DateTime.Now;
                        s.IsClosed = true;
                    }
                }
                var waitersIsWorkingNow = await dbContext.Waiters.Where(w=>w.IsWorkingNow==true).ToListAsync();
                if (waitersIsWorkingNow.Count != 0)
                {
                    foreach (var w in waitersIsWorkingNow)
                    {
                        w.IsWorkingNow = false;
                    }
                }
                var tablesIsBooked = await dbContext.Tables.Where(t=>t.IsBooked==true).ToListAsync();
                if (tablesIsBooked.Count != 0)
                {
                    foreach (var t in tablesIsBooked)
                    {
                        t.IsBooked = false;
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
                if (order == null) return RedirectToAction("Index", "Manager", new {id = User.Identity.GetUserId()});
                order.IsActive = false;
                order.Table.IsBooked = false;
                dbContext.SaveChanges();
                return RedirectToAction("Index", "Manager", new {id = User.Identity.GetUserId()});
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

        public async Task<ActionResult> WaitersList()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                //По умолчанию возвращает официантов в текущей смене
                ViewBag.Waiters = await dbContext.Waiters.Include(w=>w.User).Where(w => w.IsWork == true && w.IsWorkingNow == true).ToListAsync();
                return View();
            }
        }

        [HttpPost]
        public async Task<ActionResult> WaitersList(WaiterFiltration filtration)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var waiters = await dbContext.Waiters.Include(w => w.User).ToListAsync();
                if (filtration.IsWork == 1)
                    waiters = waiters.Where(w => w.IsWork == true).ToList();
                if (filtration.IsWork == 0)
                    waiters = waiters.Where(w => w.IsWork == false).ToList();
                if (filtration.IsWorkingNow == 1)
                    waiters = waiters.Where(w => w.IsWorkingNow == true).ToList();
                if (filtration.IsWorkingNow == 0)
                    waiters = waiters.Where(w => w.IsWorkingNow == false).ToList();
                if (String.IsNullOrEmpty(filtration.WaiterNameAndLastName) == false ||
                    String.IsNullOrWhiteSpace(filtration.WaiterNameAndLastName) == false)
                    waiters = waiters.Where(w => w.User.Name + " " + w.User.LastName == filtration.WaiterNameAndLastName).ToList();
                ViewBag.Waiters = waiters;
                return View();
            }
        }

        public async Task<ActionResult> RemoveWaiterFromShift(int id)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var waiter = await dbContext.Waiters.Include(w => w.User).FirstOrDefaultAsync(w => w.Id == id);
                if (waiter != null)
                {
                    waiter.IsWorkingNow = false;
                    await dbContext.SaveChangesAsync();
                    return RedirectToAction("WaitersList", "Manager", new {id = waiter.UserId});
                }
                return RedirectToAction("WaitersList", "Manager", new {id = User.Identity.GetUserId()});
            }
        }

        public async Task<ActionResult> TablesList()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                //По умолчанию возвращает только не скрытые в меню
                ViewBag.Tables = await dbContext.Tables.Where(t => t.IsShow == true).ToListAsync();
                return View();
            }
        }

        [HttpPost]
        public async Task<ActionResult> TablesList(TableFiltertioncs filtertion)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var tables = await dbContext.Tables.ToListAsync();
                switch (filtertion.IsShow)
                {
                    case 1:
                    {
                        tables = tables.Where(t => t.IsShow == true).ToList();
                        switch (filtertion.IsBooked)
                        {
                            case 1:
                                tables = tables.Where(t => t.IsBooked == true).ToList();
                                break;
                            case 0:
                                tables = tables.Where(t => t.IsBooked == false).ToList();
                                break;
                        }

                        break;
                    }
                    case 0:
                        tables = tables.Where(t => t.IsShow == false).ToList();
                        break;
                }

                ViewBag.Tables = tables;
                return View();
            }
        }

        public ActionResult AddTable()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AddTable(Table table)
        {
            if (!ModelState.IsValid)
            {
                return View(table);
            }
            table.IsShow = true;
            using (var dbContext = new ApplicationDbContext())
            {
                dbContext.Tables.Add(table);
                await dbContext.SaveChangesAsync();
                return RedirectToAction("TablesList", "Manager");
            }
        }

        public async Task<ActionResult> HideFromOrders (int id)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var table = await dbContext.Tables.FindAsync(id);
                if (table != null) table.IsShow = false;
                await dbContext.SaveChangesAsync();
                return RedirectToAction("TablesList", "Manager");
            }
        }

        [HttpGet]
        public async Task<ActionResult> EditTable (int id)
        {

            using (var dbContext = new ApplicationDbContext())
            {
                var table = await dbContext.Tables.FindAsync(id);
                return View(table);
            }
        }

        [HttpPost]
        public async Task<ActionResult> EditTable(Table model)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var table = await dbContext.Tables.FindAsync(model.Id);
                if (table != null && model.Name != table.Name)
                    table.Name = model.Name;
                if (table != null && model.MaxGuests != table.MaxGuests)
                    table.MaxGuests = model.MaxGuests;
                if (table != null && model.IsShow != table.IsShow)
                    table.IsShow = model.IsShow;
                await dbContext.SaveChangesAsync();
                return RedirectToAction("TablesList", "Manager");
            }
        }

        public async Task<ActionResult> DishsList()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                //По умолчанию возвращает только не скрытые в меню
                ViewBag.Dishs = await dbContext.Dishes.Where(t => t.IsShow == true).ToListAsync();
                return View();
            }
        }

        [HttpPost]
        public async Task<ActionResult> DishsList(DishFiltration filtration)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var dishes = await dbContext.Dishes.ToListAsync();
                switch (filtration.IsShow)
                {
                    case 1:
                        ViewBag.Dishs = dishes.Where(d => d.IsShow == true).ToList();
                        break;
                    case 0:
                        ViewBag.Dishs = dishes.Where(d => d.IsShow == false).ToList();
                        break;
                }

                return View();
            }
        }

        public async Task<ActionResult> HideFromOrdersDish (int id)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var dish = await dbContext.Dishes.FindAsync(id);
                if (dish != null) dish.IsShow = false;
                await dbContext.SaveChangesAsync();
                return RedirectToAction("DishsList", "Manager");
            }
        }

        public ActionResult AddDish()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AddDish(Dish dish)
        {
            if (!ModelState.IsValid)
            {
                return View(dish);
            }
            dish.IsShow = true;
            using (var dbContext = new ApplicationDbContext())
            {
                dbContext.Dishes.Add(dish);
                await dbContext.SaveChangesAsync();
                return RedirectToAction("DishsList", "Manager");
            }
        }

        [HttpGet]
        public async Task<ActionResult> EditDish(int id)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var dish = await dbContext.Dishes.FindAsync(id);
                return View(dish);
            }
        }

        [HttpPost]
        public async Task<ActionResult> EditDish(Dish model)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var dish = await dbContext.Dishes.FindAsync(model.Id);
                if (dish != null && model.Name != dish.Name)
                    dish.Name = model.Name;
                if (dish != null && model.Price != dish.Price)
                    dish.Price = model.Price;
                if (dish != null && model.IsShow != dish.IsShow)
                    dish.IsShow = model.IsShow;
                if (dish != null && model.Description != dish.Description)
                    dish.Description = model.Description;
                await dbContext.SaveChangesAsync();
                return RedirectToAction("DishsList", "Manager");
            }
        }
    }
}