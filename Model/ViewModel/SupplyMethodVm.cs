using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace targeteo.pl.Model.ViewModel
{
    public class SupplyMethodVm
    {
        public int Id { get; set; }
        public string Company { get; set; }
        public string SpecificDeliveryMethod { get; set; }
        public int SupplyMethodTypeId { get; set; }
        public string LogoPath { get; set; }
    }
}
