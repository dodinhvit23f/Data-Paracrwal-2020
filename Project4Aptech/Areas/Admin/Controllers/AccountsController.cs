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
using Project4Aptech.Repository;

namespace Project4Aptech.Areas.Admin.Controllers
{
    public class AccountsController : Controller
    {
        private DatabaseEntities db = new DatabaseEntities();
        Repo r = new Repo();
        // GET: Admin/Accounts
        public async Task<ActionResult> Index()
        {
            if (Session["user"] == null)
            {
                return Redirect("~/Admin/Home/Login");
            }
            var account = db.Account.Include(a => a.Customers);
            return View(await account.ToListAsync());
        }
        public ActionResult LockAccount(int id) {

            if (Session["user"] == null)
            {
                return Redirect("~/Admin/Home/Login");
            }
            Account account = db.Account.Find(id);
            account.A_Status = 3;
            db.Entry(account).State = EntityState.Modified;
             db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult UnlockAccount(int id) {

            if (Session["user"] == null)
            {
                return Redirect("~/Admin/Home/Login");
            }
            Account account = db.Account.Find(id);
            account.A_Status = 1;
            db.Entry(account).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult ResetPass() {
            if (Session["user"] == null)
            {
                return Redirect("~/Admin/Home/Login");
            }
            return View();
        }
        [HttpPost]
        public ActionResult ResetPass(string id)
        {
            if (id.Length != 9 ) {
                if (id.Length != 12)
                {
                    ViewBag.id = id;
                    ViewBag.Error = "CMTND không đúng định dạng";
                    return View();
                }
            }
            Account acc = db.Account.Where(a => a.Customers.Id == id).FirstOrDefault();
            if (acc != null)
            {
                string newpass = r.GeneratePass(acc.Customers.Name, acc.Customers.Id);
                acc.Pwd = r.HashPwd(newpass);
                db.Entry(acc).State = EntityState.Modified;
                db.SaveChanges();
                r.SendPass(acc.Customers.email, newpass);
                return RedirectToAction("Index");
            }
            ViewBag.id = id;
            ViewBag.Error = "khách hàng không tồn tại";
                return View();
            
        }
        // GET: Admin/Accounts/Details/5
        public async Task<ActionResult> Details(int? id)
        {

            if (Session["user"] == null)
            {
                return Redirect("~/Admin/Home/Login");
            }
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

        // GET: Admin/Accounts/Create
        public ActionResult Create()
        {

            if (Session["user"] == null)
            {
                return Redirect("~/Admin/Home/Login");
            }
            ViewBag.Num_id = new SelectList(db.Customers, "Id", "Name");
            return View();
        }

        // POST: Admin/Accounts/Create
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

        // GET: Admin/Accounts/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {

            if (Session["user"] == null)
            {
                return Redirect("~/Admin/Home/Login");
            }
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

        // POST: Admin/Accounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,Num_id,Usn,Pwd,A_Status")] Account account)
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
        
        // GET: Admin/Accounts/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {

            if (Session["user"] == null)
            {
                return Redirect("~/Admin/Home/Login");
            }
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

        // POST: Admin/Accounts/Delete/5
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
