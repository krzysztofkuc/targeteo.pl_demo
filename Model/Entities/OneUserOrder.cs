// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace targeteo.pl.Model.Entities
{
    public partial class OneUserOrder
    {
        public OneUserOrder()
        {
            OrderDetail = new HashSet<OrderDetail>();
        }

        public int Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public double Total { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public string TrackingNo { get; set; }
        public string PaymentUrl { get; set; }
        public int? UserSupplyMethodId { get; set; }
        public string AspNetUserId { get; set; }
        public string OrderSummaryId { get; set; }

        public virtual Users AspNetUser { get; set; }
        public virtual OrderSummary OrderSummary { get; set; }
        public virtual UserSupplyMethod UserSupplyMethod { get; set; }
        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
    }
}