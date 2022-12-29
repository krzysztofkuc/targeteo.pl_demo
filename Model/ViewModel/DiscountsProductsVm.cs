using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using targeteo.pl.Model.ViewModel;

namespace targeteo.pl.Model.ViewModel
{
    public class DiscountsProductsVm
    {
        public string IdProduct { get; set; }
        public ProductVM Product { get; set; }
        public int IdDiscount { get; set; }

        public DiscountVm Discount { get; set; }
    }
}
