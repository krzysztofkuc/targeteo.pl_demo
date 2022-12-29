using targeteo.pl.Model.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using targeteo.pl.Model.ViewModel;

namespace targeteo.pl.Model.ViewModel
{
    public class ProductMainPageVM
    {
        public ProductMainPageVM()
        {
            Pictures = new List<PictureVM>();

            for(int i = 0; i< 4; i++)
            {
                Pictures.Add(new PictureVM());
            }
            
        }

        public string ProductId { get; set; }

        public string Title { get; set; }

        //[Display(Name = "Product description")]
        //public string Description { get; set; }

        public double Price { get; set; }

        public string ThumbPath { get; set; }

        public List<PictureVM> Pictures { get; set; }

        //here is a problem with srialization on view Add new Product
        //public CategoryVM Category { get; set; }

        //public List<ProductAttributeVM> Attributes { get; set; }

        public bool Removed { get; set; }

        public bool Hidden { get; set; }

        public int? QuantityInStock { get; set; }

        public float? DiscountFromPrice { get; set; }

        //public DateTime? Date { get; set; }
        //public DateTime? DateTo { get; set; }

        //public UserVM User { get; set; }

        //public CityVm City { get; set; }
    }
}