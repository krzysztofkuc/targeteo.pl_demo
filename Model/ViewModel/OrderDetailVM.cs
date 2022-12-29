using targeteo.pl.Model.Entities;

namespace targeteo.pl.Model.ViewModel
{
    public class OrderDetailVM
    {
        public int OrderDetailId { get; set; }
        public string ForeignOrderId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public ProductVM Product { get; set; }
        public OneUserOrderVM OneUserOrder { get; set; }
    }
}