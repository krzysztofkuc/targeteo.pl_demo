using System.Collections.Generic;
using targeteo.pl.Model.Entities;
using targeteo.pl.Model.ViewModel;

namespace targeteo.pl.Model.ViewModel
{
    public class AddProductVM
    {
        public ICollection<ProductVM> AllProducts { get; set; }

        public ICollection<CategoryVM> AllCategories { get; set; }

        public ProductVM CurrentProduct { get; set; }

        public ICollection<string> DeletedPictures { get; set; }

        //public CategoryVM CurrentCategory { get; set; }

        public int iteration { get; set; }

        public ICollection<TreeNodeVm> AllCategoriesTreeNodeFlaten { get; set; }
        public ICollection<TreeNodeVm> AllCategoriesTreeNode { get; set; }

        public int AvgUserEvaluation { get; set; }

        public int NumberOfUserEvaluation { get; set; }

        public ICollection<SupplyMethodVm> AllSupplyMethods { get; set; }
    }
}