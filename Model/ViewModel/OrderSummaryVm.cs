using System;
using System.Collections.Generic;
using targeteo.pl.Model.Entities;

namespace targeteo.pl.Model.ViewModel
{
    public class OrderSummaryVm
    {
        public string Id { get; set; }
        public string PaymentUrl { get; set; }
        public DateTime Date { get; set; }

        public ICollection<OneUserOrderVM> OneUserOrders { get; set; }
    }
}
