using System.Collections.Generic;
using targeteo.pl.Model.ViewModel;

namespace targeteo.pl.Model.ViewModel
{
    public class OrderOpinionVM
    {
        public int Id { get; set; }

        public string Text { get; set; }
        public int OrderDetailId { get; set; }
        public int PictureId { get; set; }
        public int Evaluation { get; set; }

        public virtual OrderDetailVM OrderDetails { get; set; }

        public virtual PictureVM Pictures { get; set; }
    }
}