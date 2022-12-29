using System.ComponentModel.DataAnnotations.Schema;
using System;
using targeteo.pl.Model.Entities;

namespace targeteo.pl.Model.ViewModel
{
    public class UserAccountVM
    {
        public int Id { get; set; }
        public DateTime OperationDate { get; set; }
        public decimal OperationAmount { get; set; }
        public string OrderSummaryId { get; set; }
        public string UserId { get; set; }

        public AccountOperationStatusVM Status { get; set; }
        public int StatusId { get; set; }
        public UserVM User { get; set; }
        public OrderSummaryVm OrderSummary{ get; set; }

        public double Sum { get; set; }

        public string AccountNo{ get; set; }
    }
}
