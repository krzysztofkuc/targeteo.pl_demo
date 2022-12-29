using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace targeteo.pl.Model.ViewModel
{
    public class MsgToUserVm
    {
        public int Id { get; set; }
        public string Content { get; set; }
        //public string OrderId { get; set; }
        public string ProductId { get; set; }
        public string UserIdFrom { get; set; }
        public string UserIdTo { get; set; }
        public DateTime  Date { get; set; }
        public int? ConversationId { get; set; }
    }
}
