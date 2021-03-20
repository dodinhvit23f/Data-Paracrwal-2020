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
                }else if (isValid.A_Status==0)
                {
                    ViewBag.err = "You are logging for the 1st time,pls change your password for security purpose ";
                    return View();
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

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}