using System;
using System.Linq;
using targeteo.pl.Common;
using targeteo.pl.Model;
using Microsoft.EntityFrameworkCore;

namespace targeteo.pl.BackgroundJobs
{
    public class CheckOrderShouldBeCanceledJob
    {
        ApplicationDbContext ctx;
        public CheckOrderShouldBeCanceledJob(ApplicationDbContext ctx)
        {
            this.ctx = ctx;
        }
        public void Init()
        {

            var x = (from order in ctx.OneUserOrder.Include(o => o.OrderSummary).AsEnumerable()
                     where Convert.ToInt32((DateTime.Now - order.OrderSummary.Date).TotalMinutes) >= 5 && order.Status.Contains(nameof(Enums.OrderStatus.waitingForPayment))
                     select order);

            if (x.ToList() != null && x.ToList()?.Count() > 0)
            {
                x.ToList().ForEach(a =>
               {
                   ManageEntity.ChangeOrderStatus(ctx, a.OrderSummary.Id, nameof(Enums.OrderStatus.canceled), string.Empty);
                   //have to add quantity that wa ordered and canceled !!!!! logical bug !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
               });
            }
        }

    }
}
