using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Runtime.Caching;

namespace Project4Aptech.Repository
{
    public class Repo
    {
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
        public  void OTPGenerate(string mailAdress)
        {
            var stringChars = new char[4];
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
    }
}