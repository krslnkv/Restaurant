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
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using System.IO;

namespace Restaurant.Controllers
{
    public class WaiterController : Controller
    {
        public ActionResult Index()
        {
            return HttpNotFound();
        }
        // GET: Waiter
        [HttpGet]
        public ActionResult Index(string id)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                ViewBag.ShiftNotFound = TempData["ShiftNotFound"];
                var waiter = dbContext.Waiters.Include(w=>w.User).FirstOrDefault(w => w.UserId == id);
                ViewBag.Waiter = waiter;
            }
            return View();
        }

        public async Task<ActionResult> AddWaiterToShift(int id)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var waiter = await dbContext.Waiters.Include(w => w.User).FirstOrDefaultAsync(w => w.Id == id);
                var shift = await dbContext.Shifts.Include(s => s.Waiters).OrderByDescending(s => s.Id).FirstOrDefaultAsync(s => s.IsClosed == false);
                if (shift==null)
                {
                    TempData["ShiftNotFound"] = true;
                    return RedirectToAction("Index", "Waiter", new { id = waiter.UserId });
                }
                shift.Waiters.Add(waiter);
                waiter.IsWorkingNow = true;
                await dbContext.SaveChangesAsync();
                return RedirectToAction("Index", "Waiter", new { id = waiter.UserId });
            }
        }

        public async Task<ActionResult> RemoveWaiterFromShift(int id)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var waiter = await dbContext.Waiters.Include(w => w.User).FirstOrDefaultAsync(w => w.Id == id);
                waiter.IsWorkingNow = false;
                await dbContext.SaveChangesAsync();
                return RedirectToAction("Index", "Waiter", new { id = waiter.UserId });
            }
        }

        [HttpGet]
        public ActionResult CreateOrder()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                ViewBag.Dishes = dbContext.Dishes.ToList();
                ViewBag.Shift = dbContext.Shifts.FirstOrDefault(s => s.IsClosed == false);
                ViewBag.Tables = new SelectList(dbContext.Tables.Where(t => t.IsBooked == false).ToList(), "Id", "Name");
            }
            return View();
        }

        [HttpPost]
        public string NewOrder()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                Stream req = Request.InputStream;
                req.Seek(0, SeekOrigin.Begin);
                string orderJson = new StreamReader(req).ReadToEnd();
                NewOrderModel newOrderModel = JsonConvert.DeserializeObject<NewOrderModel>(orderJson);
                var waiterId = User.Identity.GetUserId();
                var waiter = dbContext.Waiters.FirstOrDefault(w => w.UserId == waiterId);
                var table = dbContext.Tables.Find(newOrderModel.Order.TableId);
                var shift = dbContext.Shifts.Include(s => s.Waiters).OrderByDescending(s => s.Id).FirstOrDefault(s => s.IsClosed == false);
                var order = new Order { Waiter = waiter, Shift = shift, Table = table,
                    OrderTime = DateTime.Now, FinalPrice=newOrderModel.Order.FinalPrice };
                dbContext.Orders.Add(order);
                table.IsBooked = true;
                dbContext.SaveChanges();
                for (int i=0; i<newOrderModel.OrderDetails.Count; i++)
                {
                    newOrderModel.OrderDetails[i].OrderId = order.Id;
                    dbContext.OrderDetails.Add(newOrderModel.OrderDetails[i]);
                }
                dbContext.SaveChanges();
                ViewBag.Tables = new SelectList(dbContext.Tables.Where(t=>t.IsBooked==false).ToList(), "Id", "Name");
                return waiterId;
            }
        }

        public ActionResult TodayOrders()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var waiterId = User.Identity.GetUserId();
                var od = new Order { OrderTime = DateTime.Now };
                //od.OrderTime.TimeOfDay
                ViewBag.TodayOrders = dbContext.Orders.Include(o => o.OrderDetails).Include("OrderDetails.Dish").Include(o=>o.Shift).Include(o=>o.Waiter).Include(o=>o.Table).Where(o=>o.Shift.IsClosed==false&&o.Waiter.UserId==waiterId).ToList();
                    return View();
            }
        }

        private string SetPassword()
        {
            //int[] arr = new int[16];
            Random rnd = new Random();
            string password = "";
            for (int i=0; i<16; i++)
            {
                char c = (char)rnd.Next(33, 125);
                password += c;
            }
            return password;
        }

        private Guest CreateUser(string name, string email, ApplicationUserManager userManager, ApplicationDbContext dbContext)
        {
            var user = new ApplicationUser { Name = name, Email = email, UserName=email };
            user.LastName = SetPassword();
            var result = userManager.Create(user, user.LastName);
            if (result.Succeeded)
            {
                result = userManager.AddToRole(user.Id, "guest");
                if (result.Succeeded)
                {
                    Guest guest = CreateGuest(user.Id, dbContext);
                    return guest;
                }
            }
            return null;
        }

        private Guest CreateGuest(string id, ApplicationDbContext dbContext)
        {
            Guest guest = new Guest { UserId = id };
            dbContext.Guests.Add(guest);
            dbContext.SaveChanges();
            return guest;
        }
    }
}