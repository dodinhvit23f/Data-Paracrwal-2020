using Project4Aptech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project4Aptech.Controllers
{
    public class HomeController : Controller
    {
        private DatabaseEntities db = new DatabaseEntities();
        Repository.Repo r = new Repository.Repo();
        public ActionResult Index()
        {
            var logged = (Account)Session["logged"];
            if (logged == null)
            {
                return RedirectToAction("Login");
            }
            if (TempData["Suscces"] != null)
            {
                ViewBag.ss = TempData["Suscces"];
            }
            if (TempData["fail"] != null)
            {
                ViewBag.fl = TempData["fail"];
            }
            ViewBag.balance = db.Customers.Find(logged.Num_id).balance;
            return View();
        }
        public ActionResult Start()
        {
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }
        public ActionResult Login()
        {
            if (Session["logged"] != null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
        public ActionResult Signout()
        {
            Session["logged"] = null;
            return RedirectToAction("Index");
        }
        
        [HttpPost]
        public ActionResult Login(string usn,string pwd)
        {
            string hashed = r.HashPwd(pwd);
            var isValid = db.Account.Where(p => p.Usn == usn && p.Pwd == hashed).FirstOrDefault();          
            if (isValid != null)
            {
                if (isValid.Customers.Cs_status == "0")
                {
                    ViewBag.err = "Your account has been locked due to some reasons,please contact our staff for more information";
                    return View();
                }
                else if (isValid.A_Status==0)
                {
                    TempData["usnId"] = isValid.id;
                    TempData["err"] = "Please change your password as this is the first time ";
                    return RedirectToAction("ChangePwd");
                }
                Session["logged"] = isValid;
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.usn = usn;
                ViewBag.pwd = pwd;
                ViewBag.err = "Wrong credential";
                return View();
            }
        }
        public ActionResult ChangePwd()
        {
            ViewBag.usnid = TempData["usnId"];
            if (TempData["err"] != null)
            {
                ViewBag.err = TempData["err"];
            }
            return View();
        }
        [HttpPost]
        public ActionResult ChangePwd(int usnId,string newpass,string repass )
        {
            char[] nums = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
            bool containNm = false;
            foreach (var i in newpass)
            {
                if (nums.Contains(i))
                {
                    containNm = true;
                    break;
                }
            }
            if (TempData["err"] != null)
            {
                ViewBag.err = TempData["err"];
            }
            ViewBag.usnid = usnId;
            ViewBag.pass = newpass;
            ViewBag.re = repass;
            if (newpass != repass)
            {
                ViewBag.err = "Re-enter incorrectly";
                return View();
            }else if (string.IsNullOrEmpty(newpass) || string.IsNullOrEmpty(repass))
            {
                ViewBag.err = "Please fill all the field";
                return View();
            }
            else if (newpass.Count() <8 || newpass.Count() > 40)
            {
                ViewBag.err = "Your password must be 8-40-characters ";
                return View();
            }else if (!containNm)
            {
                ViewBag.err = "Your password must contain number";
                return View();
            }
            else
            {
                var isvalid = db.Account.Find(usnId);
                isvalid.Pwd = r.HashPwd(newpass);
                isvalid.A_Status = 1;
                db.SaveChanges();
                return RedirectToAction("Login");
            }
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}