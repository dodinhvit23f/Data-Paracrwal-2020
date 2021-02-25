using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Project4Aptech.Repository
{
    public class Repo
    {
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
        public void SendEmail(string mailAdress, string OTP)
        {
            var smtpClient = new SmtpClient();
            var msg = new MailMessage();
            msg.To.Add(mailAdress);
            msg.Subject = "Test";
            msg.Body = "Your OTP is: " + OTP;
            smtpClient.Send(msg);
        }
    }
}