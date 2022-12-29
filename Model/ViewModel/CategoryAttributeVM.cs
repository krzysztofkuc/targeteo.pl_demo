using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace targeteo.pl.Model.ViewModel
{
    public class CategoryAttributeVM
    {
        public int Id { get; set; }

        [Display(Name = "Attribute name")]
        public string Name { get; set; }

        public string Value { get; set; }

        //public double? numberFrom { get; set; }

        //public double? numberTo { get; set; }

        //public string[] dateRange { get; set; }

        //public string dateFrom { get; set; }

        //public string dateTo { get; set; }

        public bool Bit { get; set; }

        public int CategoryAttributeId { get; set; }

        public string AttributeType { get; set; }

        public CategoryVM Category { get; set; }
        public AttributeValueListVM AttributeValueList { get; set; }

        public ICollection<ProductAttributeVM> ProductAttribute { get; set; }

        //public CategoryVM CategoryAttribute { get; set; }

        //public List<ProductAttributeVM> ProductAttributes { get; set; }

        public List<AttributeValueListVM> ComboboxValues { get; set; }
        public List<AttributeValueListVM> SelectedValues { get; set; }

        public AttributeValueListVM SelectedValue { get; set; }

        public string ViewFilterType { get; set; }

        public bool Hide { get; set; }
    }
}