﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace targeteo.pl.Model.Entities
{
    public partial class OrderSummary
    {
        public OrderSummary()
        {
            OneUserOrder = new HashSet<OneUserOrder>();
            UserAccount = new HashSet<UserAccount>();
        }

        public string Id { get; set; }
        public string PaymentUrl { get; set; }
        public DateTime Date { get; set; }

        public virtual ICollection<OneUserOrder> OneUserOrder { get; set; }
        public virtual ICollection<UserAccount> UserAccount { get; set; }
    }
}