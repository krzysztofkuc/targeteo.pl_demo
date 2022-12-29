using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using targeteo.pl.Model.ViewModel;

namespace targeteo.pl.Model.ViewModel
{
    public class UserSupplyMethodVm
    {
        public int Id { get; set; }
        public int PricePerUnit { get; set; }
        public int PriceForManyUnits { get; set; }
        public int TimeInHours { get; set; }
        public int SupplyMethodId { get; set; }
        public string UserId { get; set; }

        public bool? IsActive { get; set; }
        public SupplyMethodVm SupplyMethod { get; set; }
    }
}
