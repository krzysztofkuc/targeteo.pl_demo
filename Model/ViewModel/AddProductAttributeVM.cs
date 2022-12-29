using targeteo.pl.Model.Entities;
using System.Collections.Generic;
using static targeteo.pl.Common.Enums;
using targeteo.pl.Model.ViewModel;

namespace targeteo.pl.Model.ViewModel
{
    public class AddProductAttributeVM
    {
        public int ProductAttributeId { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public string AttributeType { get; set; }

        public int? CategoryAttributeId { get; set; }

        public int? FK_CategoryAttributes { get; set; }

        public int? ProductOfAttributeId { get; set; }

        public ICollection<CategoryVM> AllCategories { get; set; }

        public AllAttributeTypes AllAttributeTypes { get; set; }

        public List<AttributeValueListVM> ComboboxValues { get; set; }

        public ProductVM CurrentProduct { get; set; }

        public ProductAttributeVM CurrentProductAttribute { get; set; }

        public ICollection<CategoryAttributeVM> AllAttributes { get; set; }

        public string ViewFilterType { get; set; }

        public ICollection<TreeNodeVm> AllCategoriesTreeNode { get; set; }

        public int CategoryId { get; set; }

        public ICollection<SupplyMethodVm> SupplyMethods { get; set; }
    }
}