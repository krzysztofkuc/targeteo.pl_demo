﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace targeteo.pl.Model.ViewModel
{
    public class AddCategoryAttributeVM
    {
        public int PKAttributeId { get; set; }

        [Display(Name = "Attribute Name")]
        public string Name { get; set; }

        public string Value { get; set; }

        public int CategoryAttributeId { get; set; }

        public string AttributeType { get; set; }

        public CategoryVM Category { get; set; }

        public List<ProductAttributeVM> ProductAttributes { get; set; }

        public List<AttributeValueListVM> ComboboxValues { get; set; }
    }
}