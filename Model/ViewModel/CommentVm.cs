using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace targeteo.pl.Model.ViewModel
{
    public class CommentVm
    {
        public int Id { get; set; }
        public string Content { get; set; }

        public int OrderId { get; set; }

        public string UserId { get; set; }

        public UserVM UserFrom { get; set; }

        public UserVM UserTo { get; set; }

        public string ProductId { get; set; }

        public ProductVM Product { get; set; }

        public int ConversationId { get; set; }

        public bool? Viewed { get; set; }

        public DateTime Date { get; set; }
    }
}
