// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace targeteo.pl.Model.Entities
{
    public partial class Comments
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string ProductId { get; set; }
        public DateTime Date { get; set; }
        public string UserIdFrom { get; set; }
        public string UserIdTo { get; set; }
        public int? ConversationId { get; set; }
        public bool Viewed { get; set; }

        public virtual Conversations Conversation { get; set; }
        public virtual Product Product { get; set; }
        public virtual Users UserIdFromNavigation { get; set; }
        public virtual Users UserIdToNavigation { get; set; }
    }
}