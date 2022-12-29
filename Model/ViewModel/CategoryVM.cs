using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace targeteo.pl.Model.ViewModel
{
    public class CategoryVM
    {
        public int CategoryId { get; set; }

        [Display(Name = "Category name")]
        public string Name { get; set; }

        //[JsonIgnore]
        public int? ParentId { get; set; }
        //[JsonIgnore]
        public int? ChildId { get; set; }

        //[JsonIgnore]
        public string ParentName { get; set; }

        [JsonIgnore]
        public int? ProdId { get; set; }


        //[JsonIgnore]
        public CategoryVM Parent { get; set; }

        //[JsonIgnore]
        public ICollection<CategoryVM> Categories { get; set; }

        //[JsonIgnore]
        public ICollection<ProductVM> Products { get; set; }

        [JsonIgnore]
        public ICollection<CategoryAttributeVM> Attributes { get; set; }

        public string Icon { get; set; }

        public string Caption { get; set; }

        public string ThumbnailFileName { get; set; }

    }
}