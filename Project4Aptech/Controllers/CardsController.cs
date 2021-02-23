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

namespace Project4Aptech.Controllers
{
    public class CardsController : Controller
    {
        private DatabaseEntities db = new DatabaseEntities();

        // GET: Cards
        public async Task<ActionResult> Index()
        {
            var cards = db.Cards.Include(c => c.Customers);
            return View(await cards.ToListAsync());
        }
        public ActionResult RutTien() {
            return View();
        }
        [HttpPost]
        public ActionResult RutTien(float money) {
            return View();
        }

        // GET: Cards/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cards cards = await db.Cards.FindAsync(id);
            if (cards == null)
            {
                return HttpNotFound();
            }
            return View(cards);
        }

        // GET: Cards/Create
        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.Customers, "Id", "Name");
            return View();
        }

        // POST: Cards/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,CardNumber,AccountNumber,Password,Amount,CreateDate,ExprirateDate,Opt,Status,CreditLimmit,CustomerID")] Cards cards)
        {
            if (ModelState.IsValid)
            {
                db.Cards.Add(cards);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "Id", "Name", cards.CustomerID);
            return View(cards);
        }

        // GET: Cards/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cards cards = await db.Cards.FindAsync(id);
            if (cards == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "Id", "Name", cards.CustomerID);
            return View(cards);
        }

        // POST: Cards/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,CardNumber,AccountNumber,Password,Amount,CreateDate,ExprirateDate,Opt,Status,CreditLimmit,CustomerID")] Cards cards)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cards).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "Id", "Name", cards.CustomerID);
            return View(cards);
        }

        // GET: Cards/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cards cards = await db.Cards.FindAsync(id);
            if (cards == null)
            {
                return HttpNotFound();
            }
            return View(cards);
        }

        // POST: Cards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Cards cards = await db.Cards.FindAsync(id);
            db.Cards.Remove(cards);
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
