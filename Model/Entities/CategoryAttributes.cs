// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace targeteo.pl.Model.Entities
{
    public partial class CategoryAttributes
    {
        public CategoryAttributes()
        {
            ProductAttribute = new HashSet<ProductAttribute>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public string AttributeType { get; set; }
        public int? AttributeValueListId { get; set; }
        public bool Hide { get; set; }
        public string ViewFilterType { get; set; }

        public virtual AttributeValueList AttributeValueList { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<ProductAttribute> ProductAttribute { get; set; }
    }
}