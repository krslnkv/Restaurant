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

        //[ChildActionOnly]
        public ActionResult CreateOrder()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                ViewBag.Tables = new SelectList(dbContext.Tables.Where(t => t.IsBooked == false).ToList(), "Id", "Name");
            }
            return View();
        }

        //[ChildActionOnly]
        [HttpPost]
        public ActionResult CreateOrder(NewOrderModel model)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var waiterId = User.Identity.GetUserId();
                var waiter = dbContext.Waiters.FirstOrDefault(w => w.UserId == waiterId);
                var table = dbContext.Tables.Find(model.TableId);
                var shift = dbContext.Shifts.Include(s => s.Waiters).OrderByDescending(s => s.Id).FirstOrDefault(s => s.IsClosed == false);
                var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(dbContext));
                var user = userManager.FindByEmail(model.GuestEmail);
                Guest guest;
                if (user==null)
                {
                    guest = CreateUser(model.GuestName, model.GuestEmail, userManager, dbContext);
                }
                else
                {
                    guest = dbContext.Guests.Include(g => g.User).FirstOrDefault(g => g.UserId == user.Id);
                    if (guest == null)
                        guest = CreateGuest(user.Id, dbContext);
                }
                var order = new Order { Guest = guest, Waiter = waiter, Shift = shift, Table = table, OrderTime = DateTime.Now };
                dbContext.Orders.Add(order);
                table.IsBooked = true;
                dbContext.SaveChanges();
                ViewBag.Tables = new SelectList(dbContext.Tables.Where(t=>t.IsBooked==false).ToList(), "Id", "Name");
                return RedirectToAction("Index", "Waiter", new { id = waiter.UserId });
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