using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using targeteo.pl.Model.ViewModel;

namespace targeteo.pl.Model.ViewModel
{
    public class AddCategoryVM
    {
        public AddCategoryVM()
        {
            this.AllCategories = new List<CategoryVM>();
        }

        [UIHint("CategoryDropDOwn")]
        [DisplayName("Parent category")]
        public ICollection<CategoryVM> AllCategories { get; set; }

        public ICollection<TreeNodeVm> AllCategoriesTree { get; set; }

        public CategoryVM Category { get; set; }

        public int iteration { get; set; }
}
}