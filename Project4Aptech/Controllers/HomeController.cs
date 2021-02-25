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
            if (Session["logged"] == null)
            {
                return RedirectToAction("Login");
            }
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