using System.Collections.Generic;
using targeteo.pl.Model.ViewModel;

namespace targeteo.pl.Model.ViewModel
{
    public class HomeVM
    {
        public CategoryVM CurrentCategory { get; set; }
        public ICollection<ProductMainPageVM> Products { get; set; }
        public ICollection<CategoryVM> Categories { get; set; }

        public ICollection<TreeNodeVm> CategoriesTreeNode { get; set; }
        public List<CategoryAttributeVM> CurrentAttributes { get; set; }

        public ICollection<MenuItem> MenuItems { get; set; }

        public MenuItem CurrentCategoryBreadCrumb { get; set; }
    }
}