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
using System.Net.Mail;
using System.Runtime.Caching;
using Project4Aptech.Repository;
using System.Web.Script.Serialization;

namespace Project4Aptech.Controllers
{
    public class AccountsController : Controller
    {
        private DatabaseEntities db = new DatabaseEntities();
        private MemoryCache cache = MemoryCache.Default;
        Repo r = new Repo();
        // GET: Accounts
        public async Task<ActionResult> Index()
        {
            var account = db.Account.Include(a=>a.Customers);
            return View(await account.ToListAsync());
        }
        public ActionResult ChuyenTien(int id) {
            var accounts = db.Account.Include(a => a.Customers).Where(m => m.id == id).FirstOrDefault();
            r.OTPGenerate(accounts.Customers.email);
            ViewBag.id = accounts.id;
            ViewBag.email = accounts.Customers.email;
            return View();
        }
        [HttpPost]
        public ActionResult ChuyenTien(string money,int idSend,string idReceiver,string mess, string OTP) {
            string Key = cache.Get("OTP").ToString();
            double cash = Double.Parse(money);
            Account accountSend = db.Account.Include(a => a.Customers).Where(m => m.id == idSend).FirstOrDefault();            
            if (OTP == null) {
                ViewBag.Mess = mess;
                ViewBag.statusOTP = "OTP khong dung";
               r.OTPGenerate(accountSend.Customers.email);
                return View();
            }
            if (OTP == Key)
            {
                
                if (cash + 20000 >= accountSend.Customers.balance)
                {
                    ViewBag.Mess = mess;
                    ViewBag.statusBalance = "So tien khong du";
                    return View();
                }
                string time = DateTime.Now.ToString();
                accountSend.Customers.balance -= (cash + 20000);
                db.Entry(accountSend).State = EntityState.Modified;
                db.SaveChanges();
                r.SendBalance(accountSend.Customers.email, accountSend.Customers.Id, "-" + (cash + 20000).ToString("N"), mess,time);
                Account account = db.Account.Include(a => a.Customers).Where(m => m.Num_id == idReceiver).First();
                account.Customers.balance = account.Customers.balance + cash;
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                r.SendBalance(account.Customers.email, account.Customers.Id, "+" + cash.ToString("N"), mess,time);
                r.SaveHistory(cash, mess, "CT", accountSend.Num_id, account.Num_id,20000,time);
                r.Logging(accountSend.Customers.Id, account.Customers.Id, "CT", cash.ToString());
                return RedirectToAction("Index");
            }
            else
            {
               r.OTPGenerate(accountSend.Customers.email);
                ViewBag.Mess = mess;
                ViewBag.statusOTP = "OTP khong dung";
                return View();
            }
        }
       
        public ActionResult ResendOTP(string mailAdress) {
            r.OTPGenerate(mailAdress);
            return RedirectToAction("Index");
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
       
        // GET: Accounts/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = await db.Account.FindAsync(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // GET: Accounts/Create
        public ActionResult Create()
        {
            ViewBag.Num_id = new SelectList(db.Customers, "Id", "Name");
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,Num_id,Usn,Pwd,A_Status")] Account account)
        {
            if (ModelState.IsValid)
            {
                db.Account.Add(account);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Num_id = new SelectList(db.Customers, "Id", "Name", account.Num_id);
            return View(account);
        }

        // GET: Accounts/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = await db.Account.FindAsync(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            ViewBag.Num_id = new SelectList(db.Customers, "Id", "Name", account.Num_id);
            return View(account);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,Num_id,Usn,Pwd,Balance,A_Status")] Account account)
        {
            if (ModelState.IsValid)
            {
                db.Entry(account).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.Num_id = new SelectList(db.Customers, "Id", "Name", account.Num_id);
            return View(account);
        }

        // GET: Accounts/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = await db.Account.FindAsync(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Account account = await db.Account.FindAsync(id);
            db.Account.Remove(account);
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
