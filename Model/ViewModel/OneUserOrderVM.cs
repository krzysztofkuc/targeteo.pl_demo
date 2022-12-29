using System.Collections.Generic;
using targeteo.pl.Model.ViewModel;

namespace targeteo.pl.Model.ViewModel
{
    public class OneUserOrderVM
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool Checkbox { get; set; }
        public string Status { get; set; }
        public UserVM User { get; set; }
        public string AspNetUserId { get; set; }

        public SupplyMethodVm[] SupplyMethods { get; set; }
        public UserSupplyMethodVm SelectedSupplyMethod { get; set; }
        public int? UserSupplyMethodId { get; set; }

        public UserSupplyMethodVm UserSupplyMethod { get; set; }
        public string PaczkomatDestinationAddress { get; set; }
        //cause it has Quantity and Productvm
        public ICollection<OrderDetailVM> OrderDetails { get; set; }

        public string OrderSummaryId { get; set; }
        public OrderSummaryVm OrderSummary { get; set; }
    }
}
