using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Runtime.Caching;
using Project4Aptech.Models;
using Serilog;

namespace Project4Aptech.Repository
{
    public class Repo
    {
        private DatabaseEntities db = new DatabaseEntities();
        private MemoryCache cache = MemoryCache.Default;
        public bool isNum(string _param)
        {
            bool rsl = true;
            foreach (var i in _param)
            {
                if (!char.IsDigit(i))
                {
                    rsl = false;
                    break;
                }
            }
            return rsl;
        }
        public double toDouble(decimal _param)
        {
            return (double)_param;
        }
        public string HashPwd(string input)
        {
            System.Security.Cryptography.MD5 md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
        public DateTime stringToDate(string iDate)
        {
            DateTime oDate = Convert.ToDateTime(iDate);
            string sDate = oDate.ToString("yyyy MMMM");
            return oDate;
        }
        public void OTPGenerate(string mailAdress)
        {
            var stringChars = new char[6];
            var chars = "0123456789";
            var random = new Random();
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var OTP = new String(stringChars);
            if (cache.Get("OTP") != null)
            {
                cache.Remove("OTP");
            }
            cache.Add("OTP", OTP, DateTimeOffset.Now.AddMinutes(15));
            SendEmail(mailAdress, OTP);

        }
        public void SendEmail(string mailAdress, string OTP)
        {
            var smtpClient = new SmtpClient();
            var msg = new MailMessage();
            msg.To.Add(mailAdress);
            msg.Subject = "TP Bank 247";
            msg.Body = "Your OTP is: " + OTP;
            smtpClient.Send(msg);
        }
        public void SendBalance(string mailAdress, string idReceive, string money, string message, string time)
        {


            var smtpClient = new SmtpClient();
            var msg = new MailMessage();
            msg.IsBodyHtml = true;
            msg.To.Add(mailAdress);
            msg.Subject = "TP Bank 247";
            msg.Body = "TPBank:" + time + "</br>" + "TK:" + idReceive + "|</br>GD:" + money + "VND|</br>SDC:" + db.Customers.Find(idReceive).balance + "VND|" + "</br>ND:" + message;
            smtpClient.Send(msg);
        }
        public void SendPass(string mailAdress, string pass)
        {
            var smtpClient = new SmtpClient();
            var msg = new MailMessage();
            msg.To.Add(mailAdress);
            msg.Subject = "Test";
            msg.Body = "Your Password is: " + pass;
            smtpClient.Send(msg);
        }

        //EX: DAO NGOC HAI,id=123456789 -> pass = hai213634
        public string GeneratePass(string name, string id)
        {
            var next = id.Substring(0, 6);
            var stringChars = new char[6];
            //split to get lastname
            string pass = name.Split(null).Last().ToLower();
            //Random from id
            var random = new Random();
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = next[random.Next(next.Length)];
            }
            pass = pass + new String(stringChars);
            return pass;

        }
        public bool CheckEmailExist(string email)
        {
            Customers c = db.Customers.Where(m => m.email == email).FirstOrDefault();
            if (c == null)
            {
                return true;
            }
            return false;
        }
        public void CreateAccount(string id, string email, string Name)
        {
            string password = GeneratePass(Name, id);
            SendPass(email, password);
            Account account = new Account();
            account.Num_id = id;
            account.Usn = email;
            account.Pwd = HashPwd(password);
            account.A_Status = 0;
            db.Account.Add(account);
            db.SaveChanges();
        }
        public void SaveHistory(double money, string Mess, string code, string idFrom, string idTo, double fee, string time)
        {
            TransactionHistory history = new TransactionHistory()
            {

                Amount = (decimal)money,
                Message = Mess,
                Code = code,
                SendAccount = idFrom,
                ReceiveAccount = idTo,
                Bank_id = 1,
                Status = "S",
                fee = fee,
                tran_time = time
            };
            db.TransactionHistory.Add(history);
            db.SaveChanges();
        }
        public void Logging(string idSend,string idReciver,string type,string cashflow) {
            var log = new LoggerConfiguration()
              .MinimumLevel.Debug()
              .WriteTo.File(@"c:\log\log.txt")
              .CreateLogger();
            log.Information("Người dùng:"+idSend+"|đã thực hiện giáo dịch:"+type+"|Số tiền:"+cashflow+"|Người nhận:"+idReciver);
        }
    }
}