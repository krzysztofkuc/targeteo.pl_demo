using System.Collections.Generic;

namespace targeteo.pl.Model.ViewModel
{
    public class ShoppingCartVM
    {
        public ICollection<CartVM> CartItems { get; set; }
        public double CartTotal { get; set; }
    }
}