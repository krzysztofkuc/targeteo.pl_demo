using System.Collections.Generic;

namespace targeteo.pl.Model.ViewModel
{
    public class UserAccountSummary
    {
        public decimal SumOverall { get; set; }
        public ICollection<UserAccountVM> UserAccounts { get; set; }
    }
}
