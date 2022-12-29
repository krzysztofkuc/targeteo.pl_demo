using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace targeteo.pl.Model.ViewModel
{
    public class ChatVm
    {
        public string UserFrom { get; set; }
        public string UserTo { get; set; }
        public ICollection<CommentVm> Comments { get; set; }
    }
}
