using Project4Aptech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project4Aptech.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        private DatabaseEntities db = new DatabaseEntities();
        Repository.Repo r = new Repository.Repo();
        // GET: Admin/Home
        public ActionResult Index()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string usn,string pwd)
        {
            string hashed = r.HashPwd(pwd);
            var isValid = db.Users.Where(p=>p.UserName == usn && p.Password == hashed).FirstOrDefault();
            if(isValid != null)
            {
                Session["user"] = isValid;
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
    }
}