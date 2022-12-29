using System;
using targeteo.pl.Model.ViewModel;

namespace targeteo.pl.Model.ViewModel
{
    public class AnnouncementPaymentVm
    {
        public int Id { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string Link { get; set; }
        public string ProductId { get; set; }
        public ProductVM Product { get; set; }
    }
}
