using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Project4Aptech.Models;
using System.Security.Cryptography;
using System.Text;
using System.Net.Mail;
using Project4Aptech.Repository;

namespace Project4Aptech.Areas.Admin.Controllers
{
    public class CustomersController : Controller
    {
        private DatabaseEntities db = new DatabaseEntities();
        Repo r = new Repo();

        // GET: Admin/Customers
        public async Task<ActionResult> Index()
        {

            if (Session["user"] == null)
            {
                return Redirect("~/Admin/Home/Login");
            }
            return View(await db.Customers.ToListAsync());
        }

        // GET: Admin/Customers/Details/5
        public async Task<ActionResult> Details(string id)
        {

            if (Session["user"] == null)
            {
                return Redirect("~/Admin/Home/Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customers customers = await db.Customers.FindAsync(id);
            if (customers == null)
            {
                return HttpNotFound();
            }
            return View(customers);
        }

        // GET: Admin/Customers/Create
        public ActionResult Create()
        {

            if (Session["user"] == null)
            {
                return Redirect("~/Admin/Home/Login");
            }
            return View();
        }

        // POST: Admin/Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,PhoneNumber,Address,DOF,balance,email")] Customers customers)
        {
            if (ModelState.IsValid)
            {
                Customers c = db.Customers.Where(m => m.Id == customers.Id).FirstOrDefault();
                if (c == null)
                {
                    if (r.CheckEmailExist(customers.email))
                    {
                        customers.balance = 0;
                        db.Customers.Add(customers);
                        await db.SaveChangesAsync();
                        r.CreateAccount(customers.Id, customers.email, customers.Name);
                        return RedirectToAction("Index");
                    }
                    ViewBag.ErrorEmail = "Email already exist !";
                    return View(customers);
                }
                ViewBag.ErrorCMTND = "CMTND already exist !";
                return View(customers);
            }

            return View(customers);
        }
      
        public ActionResult AddBalance() {

            if (Session["user"] == null)
            {
                return Redirect("~/Admin/Home/Login");
            }
            return View();
        }
        [HttpPost]
        public ActionResult AddBalance(string money, string idSend, string idReceiver, string mess) {
            Customers Send = null;
            double cash = Double.Parse(money);
            Customers Reciver = db.Customers.Find(idReceiver);
            string time="";
            if (idSend != "") {
                Send = db.Customers.Find(idSend);
                if (Send != null)
                {
                    if (cash + 20000 >=Send.balance)
                    {
                        ViewBag.idSend = idSend;
                        ViewBag.idReceiver = idReceiver;
                        ViewBag.Mess = mess;
                        ViewBag.statusBalance = "So tien khong du";
                        return View();
                    }
                    Reciver.balance += cash;
                    db.Entry(Reciver).State = EntityState.Modified;
                    db.SaveChanges();
                    r.SendBalance(Reciver.email, Reciver.Id, "+" + money.ToString(), mess, time);
                    Send.balance -= (cash + 20000);
                    db.Entry(Send).State = EntityState.Modified;
                    db.SaveChanges();
                    r.SendBalance(Send.email, Send.Id, "-" + (cash + 20000).ToString("N"), mess, time);
                    r.SaveHistory(cash, mess, "CT", idSend, idReceiver, 20000,time);
                    r.Logging(Send.Id, Reciver.Id, "CT",cash.ToString());
                    return RedirectToAction("Index");
                }
                else {
                        
                    ViewBag.Error = "Send Customer not exist!";
                    return View();
                }
            }
            else {
                if (Reciver != null)
                {
                    time = DateTime.Now.ToString();
                    Reciver.balance += cash;
                    db.Entry(Reciver).State = EntityState.Modified;
                    db.SaveChanges();
                    r.SendBalance(Reciver.email, Reciver.Id, "+" + cash.ToString("N"), mess, time);
                    r.SaveHistory(cash, mess, "CT", "0", idReceiver, 20000, time);
                    r.Logging("NH", Reciver.Id, "CT", cash.ToString());
                    return RedirectToAction("Index");
                }
                ViewBag.idReceiver = idReceiver;
                ViewBag.Mess = mess;
                return View();
            }
        }
        
        public JsonResult getCustomer(string id)
        {
            string Name = "";
            try
            {
                var Cus = db.Customers.Find(id);
                if (Cus != null)
                {
                    Name += Cus.Name;
                }

            }
            catch (Exception e)
            {
            }

            return Json(Name, JsonRequestBehavior.AllowGet);
        }
        // GET: Admin/Customers/Edit/5
        public async Task<ActionResult> Edit(string id)
        {

            if (Session["user"] == null)
            {
                return Redirect("~/Admin/Home/Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customers customers = await db.Customers.FindAsync(id);
            if (customers == null)
            {
                return HttpNotFound();
            }
            return View(customers);
        }

        // POST: Admin/Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,PhoneNumber,Address,DOF,acc_num,balance,email")] Customers customers)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customers).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(customers);
        }

        // GET: Admin/Customers/Delete/5
        public async Task<ActionResult> Delete(string id)
        {

            if (Session["user"] == null)
            {
                return Redirect("~/Admin/Home/Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customers customers = await db.Customers.FindAsync(id);
            if (customers == null)
            {
                return HttpNotFound();
            }
            return View(customers);
        }

        // POST: Admin/Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            Customers customers = await db.Customers.FindAsync(id);
            db.Customers.Remove(customers);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
