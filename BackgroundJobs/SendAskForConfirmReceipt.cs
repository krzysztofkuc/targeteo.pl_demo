using System;
using System.Linq;
using targeteo.pl.Common;
using targeteo.pl.Model;
using Microsoft.EntityFrameworkCore;

namespace targeteo.pl.BackgroundJobs
{
    public class SendAskForConfirmReceipt
    {
        private readonly ApplicationDbContext ctx;
        public SendAskForConfirmReceipt(ApplicationDbContext ctx)
        {
            this.ctx = ctx;
        }

        public void Init()
        {
            var x = (from order in ctx.OneUserOrder.Include(o => o.OrderSummary).AsEnumerable()
                     where Convert.ToInt32((DateTime.Now - order.OrderSummary.Date).Days) > 5 && order.Status.Contains(nameof(Enums.OrderStatus.paymentCompleted))
                     select order);

            if (x.ToList() != null && x.ToList()?.Count() > 0)
            {
                x.ToList().ForEach(a =>
                {
                    //send Email to ask for confirm receipt
                });
            }
        }
    }
}