using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

using System.Data.Entity;

using Project4Aptech.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Project4Aptech
{
    [HubName("TPHub")]
    public class TPHub : Hub
    {
        private DatabaseEntities db = new DatabaseEntities();
        public void Send(string Id)
        {
     
            Customers cus = db.Customers.Where(c=>c.acc_num==Id).FirstOrDefault();
            var balance = cus.balance;
            Clients.All.Receive(balance);
        }
    }
}