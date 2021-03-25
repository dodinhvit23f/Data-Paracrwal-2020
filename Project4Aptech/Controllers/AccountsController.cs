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
using System.Globalization;

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
        public ActionResult ChuyenTien(string id) {
            if (Session["logged"] == null)
            {
                return Redirect("~/Home/Login");
            }
            var accounts = db.Customers.Find(id);
            r.OTPGenerate(accounts.email);
            ViewBag.id = accounts.Id;
            ViewBag.email = accounts.email;
            return View();
        }
        [HttpPost]
        public ActionResult ChuyenTien(string money,string idSend,string idReceiver,string mess, string OTP) {
            CultureInfo culture = new CultureInfo("vi-VN");
            string Key = cache.Get("OTP").ToString();
            double cash = Double.Parse(money,culture);
            Customers accountSend = db.Customers.Find(idSend);
            Customers account = db.Customers.Where(a=>a.acc_num==idReceiver).FirstOrDefault();
            if (account == null) {
                ViewBag.Re = "Người nhận không tồn tại";
                ViewBag.Mess = mess;
                return View();
            }
            if (idReceiver == accountSend.acc_num) {
                ViewBag.Re = "Tài khoản nhận trùng với tài khoản gửi!!";
                ViewBag.Mess = mess;
                return View();
            }
            if (OTP == null) {
                ViewBag.Mess = mess;
                ViewBag.statusOTP = "OTP khong dung";
               r.OTPGenerate(accountSend.email);
                return View();
            }
            if (OTP == Key)
            {
                
                if (cash + 20000 >= accountSend.balance)
                {
                    ViewBag.Mess = mess;
                    ViewBag.statusBalance = "So tien khong du";
                    return View();
                }
                string time = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
               
                accountSend.balance -= (cash + 20000);
                db.Entry(accountSend).State = EntityState.Modified;
                db.SaveChanges();
                r.SendBalance(accountSend.email, accountSend.acc_num, "-" + (cash + 20000).ToString("N"), mess,time);           
                account.balance = account.balance + cash;
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                r.SendBalance(account.email, account.acc_num, "+" + cash.ToString("N"), mess,time);
                r.SaveHistory(cash, mess, "T", accountSend.acc_num, account.acc_num,20000,time);
                r.Logging(accountSend.acc_num, account.acc_num, "Chuyển tiền", cash.ToString());
                return Redirect("~/Home/Index");
            }
            else
            {
               r.OTPGenerate(accountSend.email);
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
                var Cus = db.Customers.Where(cus=>cus.acc_num==id).FirstOrDefault();
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
