using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace targeteo.pl.Model.ViewModel
{
    public class PaymentResponseVM
    {
        public string KWOTA { get; set; }
        public string ID_PLATNOSCI { get; set; }
        public string ID_ZAMOWIENIA { get; set; }
        public string STATUS { get; set; }
        public string SEKRET { get; set; }
        public string HASH { get; set; }
        public string SECURE { get; set; }
    }
}
