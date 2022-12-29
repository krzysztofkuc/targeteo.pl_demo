using System;
using System.Linq;
using targeteo.pl.Common;
using targeteo.pl.Model;
using Microsoft.EntityFrameworkCore;

namespace targeteo.pl.BackgroundJobs
{
    public class CkeckIfOrderReceiptShouldBeConfirmed
    {
        private readonly ApplicationDbContext ctx;
        public CkeckIfOrderReceiptShouldBeConfirmed(ApplicationDbContext ctx)
        {
            this.ctx = ctx;
        }

        public void Init()
        {
            var x = (from order in ctx.OneUserOrder.Include(o => o.OrderSummary).AsEnumerable()
                     where Convert.ToInt32((DateTime.Now - order.OrderSummary.Date).Days) > 14 && order.Status.Contains(nameof(Enums.OrderStatus.paymentCompleted))
                     select order);

            if (x.ToList() != null && x.ToList()?.Count() > 0)
            {
                x.ToList().ForEach(a =>
                {
                    if(a.Status != nameof(Enums.OrderStatus.receiptConfirmed))
                    {
                        ManageEntity.ChangeOrderStatus(ctx, a.OrderSummary.Id, nameof(Enums.OrderStatus.receiptConfirmed), string.Empty);
                    }
                });
            }
        }
    }
}
