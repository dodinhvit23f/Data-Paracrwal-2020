using Project4Aptech.Models;
using Project4Aptech.Repository;
using System;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;
using System.IO;
using Project4Aptech.Invoice;

namespace Project4Aptech.Controllers
{
    public class WithDrawingController : Controller
    {
        private MemoryCache cache = MemoryCache.Default;
        Repo r = new Repo();
        private DatabaseEntities db = new DatabaseEntities();
        // GET: WithDrawing
        public ActionResult Index()
        {
            if (Session["logged"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Index(string str_amount)
        {
            double amount =  0;
            TempData["amout"] = amount;
            var logged = (Account)Session["logged"];
            //rut toi da 5 cu?
            //moi ngay rut toi da 100 cu
            //So tien phai chia het cho 20000 hoac 50000
            var histories = db.TransactionHistory.Where(p=>p.SendAccount==logged.Customers.Id && p.Code=="W" && p.Status=="S");
            double amt = 0;
            foreach (var item in histories)
            {
                if (DateTime.Compare(DateTime.Parse(item.tran_time),DateTime.Today)==0)
                {
                    amt += (double)item.Amount;
                }
            }
            if (!r.isNum(str_amount.ToString()))
            {
                ViewBag.err = "Please enter a correct number";
                return View();
            }
            else
            {
                 amount = (double)int.Parse(str_amount);
            }
            if (amount > 5000000)
            {
                ViewBag.err = "You only can withdraw 5 million VND a time";
            }else if (amount <0)
            {
                ViewBag.err = "Please enter a positive number";
            }
            else if (amount % 20000 != 0 && amount % 50000 != 0)
            {
                ViewBag.err = "You only can withdraw an amount that is divided by 20.000 VND or 50.000VND";
            }else if (amt > 100000000)
            {
                ViewBag.err = "You only can withdraw 100 million VND a day";
            }else if ((logged.Customers.balance - amount) <= 50000 )
            {
                ViewBag.err = "Your balance must be remained more than 50.000 VND";
            }
            else
            {
                TransactionHistory t = new TransactionHistory()
                {
                    Amount = (decimal)amount,
                    Code = "W",//Withdraw 
                    fee = 0,
                    Status = "P",//Pending
                    tran_time = DateTime.Now.ToString(),
                    SendAccount = logged.Customers.Id.ToString(),
                    ReceiveAccount = logged.Customers.Id.ToString(),                    
                };
                db.TransactionHistory.Add(t);
                db.SaveChanges();
                TempData["id"] = t.Id;
                Session["time"] = 0;
                //r.SendEmail(logged.Customers.email, "heheh");
                r.OTPGenerate(logged.Customers.email);
                if (logged.A_Status == 0)
                {
                    return RedirectToAction("Signout","Home");
                }
                return RedirectToAction("CheckOtp");
            }            
            return View();
        }
        public ActionResult CheckOtp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CheckOtp(string otp_code)
        {
            var logged = (Account)Session["logged"];
            string Key = cache.Get("OTP").ToString();
            if ((int)Session["time"] >= 3)
            {
                TempData["fail"] = "WithDraw cash failed,please try again";
                return RedirectToAction("Index", "Home");
            }
            else if (otp_code==Key)
            {
                int id = (int)TempData["id"];
                var changed = db.TransactionHistory.Find(id);
                var user = db.Account.Find(logged.id);
                
                user.Customers.balance -= (double)changed.Amount;
                changed.Status = "S";
                db.SaveChanges();
                string mess = string.Format("Account - {0},balance:{1},at {2} :",changed.Amount, user.Customers.balance,changed.tran_time);
                r.SendEmail(logged.Customers.email, mess);
                return RedirectToAction("Detail/"+id);
            }
            else
            {
                Session["time"] = (int)Session["time"] +1;
                ViewBag.wrong = "Invalid Otp code";
                return View();
            }
        }
        public ActionResult Detail(int id)
        {
            var isValid = db.TransactionHistory.Find(id);
            return View(isValid);
        }
        public ActionResult Print(int id,string cus_id)
        {
            InvoicePrepare invoice = new InvoicePrepare();
            byte[] abytes = invoice.Prepare(db.TransactionHistory.Where(p=>p.Id==id).ToList(),cus_id);
            return File(abytes, "application/pdf");
        }
    }
}