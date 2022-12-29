using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace targeteo.pl.Model.ViewModel
{
    public class DiscountVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ValuePercent { get; set; }
        public double ValuePrice { get; set; }
        public DateTime EndDateTime { get; set; }
        public ICollection<DiscountsProductsVm> DiscountsProducts { get; set; }
    }
}
