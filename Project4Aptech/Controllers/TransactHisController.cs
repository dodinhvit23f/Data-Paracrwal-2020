using Project4Aptech.Invoice;
using Project4Aptech.Models;
using Project4Aptech.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project4Aptech.Controllers
{
    public class TransactHisController : Controller
    {
        private DatabaseEntities db = new DatabaseEntities();
        Repo r = new Repo();
        public ActionResult Index(string id)
        {
            if (db.Customers.Find(id).Account.FirstOrDefault().A_Status == 0)
            {
                return RedirectToAction("Signout", "Home");
            }
            //var logged = (Account)Session["logged"];
            ViewBag.cus_id = id;
            var isValid=db.TransactionHistory.Where(p=>p.ReceiveAccount==id|| p.SendAccount==id);
            return View(isValid);
        }
        public ActionResult Bankstatement_Option()
        {
            return View();
        }
        public ActionResult ByQuaterly(int quater, int year)
        {
            List<TransactionHistory> isValid = new List<TransactionHistory>();
            var logged = (Account)Session["logged"];
            if (logged.A_Status == 0)
            {
                return RedirectToAction("Signout", "Home");
            }
            int[] quater1 = { 1, 2, 3 };
            int[] quater2 = { 4, 5, 6 };
            int[] quater3 = { 7, 8, 9 };
            int[] quater4 = { 10, 11, 12 };
            if (quater==1)
            {
                isValid = db.TransactionHistory.ToList().Where(p =>quater1.Contains(r.stringToDate(p.tran_time).Month) && r.stringToDate(p.tran_time).Year == year).ToList();
            }else if (quater == 2)
            {
                isValid = db.TransactionHistory.ToList().Where(p => quater2.Contains(r.stringToDate(p.tran_time).Month) && r.stringToDate(p.tran_time).Year == year).ToList();

            }
            else if (quater == 3)
            {
                isValid = db.TransactionHistory.ToList().Where(p => quater3.Contains(r.stringToDate(p.tran_time).Month) && r.stringToDate(p.tran_time).Year == year).ToList();

            }
            else
            {
                isValid = db.TransactionHistory.ToList().Where(p => quater1.Contains(r.stringToDate(p.tran_time).Month) && r.stringToDate(p.tran_time).Year == year).ToList();
            }
            InvoicePrepare invoice = new InvoicePrepare();
            byte[] abytes = invoice.Prepare(isValid, logged.Customers.Id);
            return File(abytes, "application/pdf");
        }
        
        public ActionResult ByMonth(int month,int year)
        {
            var logged = (Account)Session["logged"];
            if (logged.A_Status == 0)
            {
                return RedirectToAction("Signout", "Home");
            }
            var isValid = db.TransactionHistory.ToList().Where(p => r.stringToDate(p.tran_time).Month == month && r.stringToDate(p.tran_time).Year==year).ToList();
            //TransactionHistory tran = new TransactionHistory() {
            //    Message ="Print invoice fee",
            //    SendAccount = logged.Customers.Id,
            //    ReceiveAccount = "013639335",
            //    Amount = 10000,
            //    tran_time = DateTime.Now.ToString(),
            //    fee = 0,
            //    Code = "T",
            //    Status = "S",                                
            //};
            //Phi in sao ke 10000/1 thang,20k/1 quy'
            InvoicePrepare invoice = new InvoicePrepare();
            byte[] abytes = invoice.Prepare(isValid, logged.Customers.Id);
            return File(abytes, "application/pdf");
        }

    }
}