using System.Collections.Generic;

namespace targeteo.pl.Model.ViewModel
{
    public class UserProfileVm
    {
        public UserVM User { get; set; }

        public string AccountNo { get; set; }
        public ICollection<SupplyMethodVm> SupplyMethods { get; set; }
    }
}
