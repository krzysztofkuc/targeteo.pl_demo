using Newtonsoft.Json;
using System.Collections.Generic;

namespace targeteo.pl.Model.ViewModel
{
    public class ProductAttributeVM
    {
        public int ProductAttributeId { get; set; }

        public string ProductId { get; set; }

        public string Value { get; set; }

        public bool IsRequired { get; set; }

        [JsonIgnore]
        public ProductVM Product { get; set; }

        public int CategoryAttributeId { get; set; }
        public CategoryAttributeVM CategoryAttribute { get; set; }

        [JsonIgnore]
        public ICollection<AttributeValueListVM> ComboboxValues { get; set; }
    }
}