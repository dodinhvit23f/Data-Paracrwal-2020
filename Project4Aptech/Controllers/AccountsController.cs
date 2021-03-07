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

namespace Project4Aptech.Controllers
{
    public class AccountsController : Controller
    {
        private DatabaseEntities db = new DatabaseEntities();
        private MemoryCache cache = MemoryCache.Default;
        // GET: Accounts
        public async Task<ActionResult> Index()
        {
            var account = db.Account.Include(a => a.Customers);
            return View(await account.ToListAsync());
        }
        public ActionResult ChuyenTien(int id) {
            var accounts = db.Account.Include(a => a.Customers).Where(m => m.id == id).FirstOrDefault();
            OTPGenerate(accounts.Customers.email);
            ViewBag.id = accounts.id;
            return View();
        }
        [HttpPost]
        public ActionResult ChuyenTien(double money,int idSend,string idReceiver,string mess, string OTP) {
            string Key = cache.Get("OTP").ToString();
            Account accountSend = db.Account.Include(a => a.Customers).Where(m => m.id == idSend).FirstOrDefault();
            
            if (OTP == null) {
                ViewBag.Mess = mess;
                ViewBag.statusOTP = "OTP khong dung";
                OTPGenerate(accountSend.Customers.email);
                return View();
            }
            if (OTP == Key)
            {
                if (money >= accountSend.Customers.balance)
                {
                    ViewBag.Mess = mess;
                    ViewBag.statusBalance = "So tien khong du";
                    return View();
                }              
                accountSend.Customers.balance -= money;
                db.Entry(accountSend).State = EntityState.Modified;
                db.SaveChanges();
                Account account = db.Account.Include(a => a.Customers).Where(m => m.Num_id == idReceiver).First();
                account.Customers.balance = account.Customers.balance + money;
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                SaveHistory(money, mess, "CT", accountSend.id, account.id);
                return RedirectToAction("Index");
            }
            else
            {
                OTPGenerate(accountSend.Customers.email);
                ViewBag.Mess = mess;
                ViewBag.statusOTP = "OTP khong dung";
                return View();
            }
        }
        public void Send(string mailAdress,string OTP) {
            var smtpClient = new SmtpClient();
            var msg = new MailMessage();
            msg.To.Add(mailAdress);
            msg.Subject = "Test";
            msg.Body = "Your OTP is: "+OTP;
            smtpClient.Send(msg);          
        }
        public void OTPGenerate(string mailAdress) {
            var stringChars = new char[6];
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            for(int i = 0; i < stringChars.Length; i++)
{
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var OTP = new String(stringChars);
            if (cache.Get("OTP") != null)
            {
                cache.Remove("OTP");
            }
            cache.Add("OTP", OTP, DateTimeOffset.Now.AddHours(1.0));
            Send(mailAdress, OTP);
            
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
        public void SaveHistory(double money,string Mess,string code,int idFrom,int idTo) {
            TransactionHistory history = new TransactionHistory()
            {
                
                Amount =(decimal)money,
                Message = Mess,
                Code = code,
                SendAccount = idFrom.ToString(),
                ReceiveAccount = idTo.ToString(),
                Bank_id = 1,
                Status = "0"              
            };
            db.TransactionHistory.Add(history);
            db.SaveChanges();
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
