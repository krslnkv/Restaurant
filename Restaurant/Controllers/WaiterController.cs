﻿using System;
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
    [Authorize (Roles ="manager, waiter")]
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
                if (waiter == null) return RedirectToAction("Index", "Home");
                waiter.IsWorkingNow = true;
                await dbContext.SaveChangesAsync();
                return RedirectToAction("Index", "Waiter", new {id = waiter.UserId});
            }
        }

        public async Task<ActionResult> RemoveWaiterFromShift(int id)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var waiter = await dbContext.Waiters.Include(w => w.User).FirstOrDefaultAsync(w => w.Id == id);
                if (waiter == null) return RedirectToAction("Index", "Home");
                waiter.IsWorkingNow = false;
                await dbContext.SaveChangesAsync();
                return RedirectToAction("Index", "Waiter", new {id = waiter.UserId});
            }
        }

        [HttpGet]
        public ActionResult CreateOrder()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                ViewBag.Dishes = dbContext.Dishes.Where(d=>d.IsShow==true).ToList();
                ViewBag.Shift = dbContext.Shifts.FirstOrDefault(s => s.IsClosed == false);
                ViewBag.Tables = new SelectList(dbContext.Tables.Where(t => t.IsBooked == false&&t.IsShow==true).ToList(), "Id", "Name");
            }
            return View();
        }

        [HttpPost]
        public string NewOrder()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var req = Request.InputStream;
                req.Seek(0, SeekOrigin.Begin);
                var orderJson = new StreamReader(req).ReadToEnd();
                var newOrderModel = JsonConvert.DeserializeObject<NewOrderModel>(orderJson);
                var waiterId = User.Identity.GetUserId();
                var waiter = dbContext.Waiters.FirstOrDefault(w => w.UserId == waiterId);
                var table = dbContext.Tables.Find(newOrderModel.Order.TableId);
                var shift = dbContext.Shifts.Include(s => s.Waiters).OrderByDescending(s => s.Id).FirstOrDefault(s => s.IsClosed == false);
                var order = new Order { Waiter = waiter, Shift = shift, Table = table,
                    OrderTime = DateTime.Now, FinalPrice=newOrderModel.Order.FinalPrice, IsActive=true };
                dbContext.Orders.Add(order);
                if (table != null) table.IsBooked = true;
                dbContext.SaveChanges();
                foreach (var od in newOrderModel.OrderDetails)
                {
                    od.OrderId = order.Id;
                    dbContext.OrderDetails.Add(od);
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
                ViewBag.TodayOrders = dbContext.Orders.Include(o => o.OrderDetails).Include("OrderDetails.Dish").Include(o=>o.Shift).Include(o=>o.Waiter).Include(o=>o.Table).Where(o=>o.Shift.IsClosed==false&&o.Waiter.UserId==waiterId).ToList();
                    return View();
            }
        }

        [HttpGet]
        public ActionResult CloseOrder(int id)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var order = dbContext.Orders.Include(o=>o.Table).FirstOrDefault(o=>o.Id==id);
                if (order != null)
                {
                    order.IsActive = false;
                    order.Table.IsBooked = false;
                }

                dbContext.SaveChanges();
                return RedirectToAction("Index", "Waiter", new { id = User.Identity.GetUserId() });
            }
        }
    }
}