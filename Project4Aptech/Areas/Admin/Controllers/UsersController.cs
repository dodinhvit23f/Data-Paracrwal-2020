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
    public class UsersController : Controller
    {
        private DatabaseEntities db = new DatabaseEntities();
        Repo r = new Repo();
        // GET: Admin/Users
        public async Task<ActionResult> Index()
        {
            var users = db.Users.Include(u => u.Roles);
            return View(await users.ToListAsync());
        }

        // GET: Admin/Users/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = await db.Users.FindAsync(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // GET: Admin/Users/Create
        public ActionResult Create()
        {
            ViewBag.Roll_id = new SelectList(db.Roles, "id", "name");
            return View();
        }

        // POST: Admin/Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,Password,UserName,Roll_id")] Users users,string repass)
        {
            if (ModelState.IsValid)
            {
                if (db.Users.Where(u => u.UserName == users.UserName).FirstOrDefault() == null)
                {
                    if (repass != users.Password)
                    {
                        ViewBag.Roll_id = new SelectList(db.Roles, "id", "name", users.Roll_id);
                        ViewBag.Err = "Repassword not correct";
                        return View(users);
                    }
                    users.Password = r.HashPwd(users.Password);
                    db.Users.Add(users);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                ViewBag.Roll_id = new SelectList(db.Roles, "id", "name", users.Roll_id);
                ViewBag.ErrU = "Username already exists";
                return View(users);
            }

            ViewBag.Roll_id = new SelectList(db.Roles, "id", "name", users.Roll_id);
            return View(users);
        }

        // GET: Admin/Users/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = await db.Users.FindAsync(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            ViewBag.Roll_id = new SelectList(db.Roles, "id", "name", users.Roll_id);
            return View(users);
        }

        // POST: Admin/Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,Password,UserName,Roll_id")] Users users)
        {
            if (ModelState.IsValid)
            {
                db.Entry(users).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.Roll_id = new SelectList(db.Roles, "id", "name", users.Roll_id);
            return View(users);
        }

        // GET: Admin/Users/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = await db.Users.FindAsync(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // POST: Admin/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Users users = await db.Users.FindAsync(id);
            db.Users.Remove(users);
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
