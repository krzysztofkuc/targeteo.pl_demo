using Microsoft.EntityFrameworkCore;
using shortid;
using shortid.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using targeteo.pl.Model;
using targeteo.pl.Model.Entities;
using targeteo.pl.Model.ViewModel;
using static targeteo.pl.Common.Enums;

namespace targeteo.pl.Helpers
{
    public class IndexHelper
    {
        private ApplicationDbContext db;
        public IndexHelper(ApplicationDbContext db)
        {
            this.db = db;
        }

        public static string GenerateOrderId()
        {
            var options = new GenerationOptions
            {
                UseNumbers = true,
                UseSpecialCharacters = false,

                Length = 8
            };
            string shortId = ShortId.Generate(options).ToLower();
            var date = DateTime.Now.Date.ToShortDateString().Replace("/", "");
            date = DateTime.Now.Date.ToShortDateString().Replace("-", "");

            return date + shortId;

        }

        #region privateMethodsHelpers

        public List<List<CategoryAttributeVM>> FiltersGroupByName(List<CategoryAttributeVM> resultFilters)
        {
            var result = new List<List<CategoryAttributeVM>>();

            var alreadyProcessed = string.Empty;
            foreach (var item in resultFilters)
            {
                if (item.Name == alreadyProcessed)
                {
                    continue;
                }

                alreadyProcessed = item.Name;
                var allSameItems = resultFilters.Where(o => o.Name == item.Name).ToList();

                result.Add(allSameItems);
            }

            return result;
        }

        /// <summary>
        /// return "From", "To, ""
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetFilterType(string key)
        {
            var result = string.Empty;
            var splitedTypeDash = key.Split('_');
            var splitedTypePause = key.Split('-');
            if (splitedTypeDash.Length > 1)
            {
                result = splitedTypeDash[1];
            }
            else if (splitedTypePause.Length == 1)
            {
                result = splitedTypePause[0];
            }

            return result;
        }

        public IEnumerable<Category> GetFlattenCategoriesUp(Category currentCategory)
        {
            var result = new List<Category>();
            return FlattingUp(currentCategory, ref result);
        }

        public IEnumerable<Category> GetFlattenCategories(Category currentCategory)
        {
            var resultUp = new List<Category>();
            FlattingUp(currentCategory, ref resultUp);

            //maybe it is not neccessary because first node is added before
            var res = resultUp.Concat(Flatting(currentCategory));
            var xx = res.DistinctBy(c => c.CategoryId);
            //this dsisctinc is problematic, doesnt load products
            return res.DistinctBy(c => c.CategoryId);
            //return Flatting(currentCategory);

        }

        /// <su2qwwwwa  qqqqqqQ
        /// 
        /// 
        /// 
        /// mmary>
        /// flat with parents
        /// </summary>
        public IEnumerable<Category> FlattingUp(Category root, ref List<Category> result)
        {
            var stack = new Stack<Category>();
            db.Entry(root).Collection(d => d.CategoryAttributes).Load();
            db.Entry(root).Reference(r => r.Parent).Load();
            db.Entry(root).Collection(r => r.Product).Load();
            //db.Entry(root).Collection(d => d.Products).Load();
            //loading productscan be removed because it slow down

            if (root.Product == null)
            {
                db.Entry(root).Collection(c => c.Product).Query()
                                    .Include(p => p.Picture)
                                    .Include(p => p.City)
                                    .Include(p => p.ProductAttribute)
                                    .ThenInclude(attr => attr.CategoryAttribute).Load();
            }
            //db.Entry(root).Collection(d => d.Products).Load();
            //db.Entry(root).Reference(r => r.Parent).Load();
            result.Add(root);

            if (root.Parent != null)
            {
                FlattingUp(root.Parent, ref result);
                //db.Entry(current.Parent).Collection(d => d.Attributes).Load();
                //db.Entry(current.Parent).Collection(d => d.Products).Query().Include(p => p.Attributes).Load();

            }

            return result;
        }

        public IEnumerable<Category> Flatting(Category root)
        {   
            var stack = new Stack<Category>();
            db.Entry(root).Collection(d => d.CategoryAttributes).Load();
            //loading productscan be removed because it slow down
            db.Entry(root).Collection(c => c.Product).Query().Include(p => p.Picture).Include(p => p.ProductAttribute).ThenInclude(attr => attr.CategoryAttribute).Load();
            //db.Entry(root).Collection(d => d.Products).Load();
            db.Entry(root).Reference(r => r.Parent).Load();

            stack.Push(root);
            while (stack.Count > 0)
            {
                var current = stack.Pop();
                yield return current;

                if (current.InverseParent != null)
                {
                    foreach (var child in current.InverseParent)
                    {
                        db.Entry(child).Collection(d => d.CategoryAttributes).Load();
                        stack.Push(child);
                    }
                }
            }
        }

        public string GetFilterName(string key)
        {
            string result = string.Empty;

            var isPause = key.Split('-').Count() > 1;
            var isDash = key.Split('_').Count() > 1;

            result = key.Split('_')[0];
            //if (isDash)
            //{
            //    result = key.Split('_')[0];
            //}

            //else if (isPause)
            //{
            //    result = key.Split('-')[1];
            //}

            return result;
        }

        public List<CategoryAttributeVM> GetViewFilters(List<CategoryVM> categories)
        {
            var result = new List<CategoryAttributeVM>();

            //Here should be entities not VM
            var allProducts = new List<ProductVM>();

            foreach (var item in categories)
            {
                if (item?.Attributes != null)
                {
                    result.AddRange(item?.Attributes);
                }

                if (item.Products != null)
                {
                    allProducts.AddRange(item.Products?.Where(p => !p.Removed));
                }
            }

            //var allCurrentCategoryAttributes = result.GroupBy(p => p.Name).Select(g => g.First()).ToList();

            var allCurrentCategoryAttributes = result;

            for (int i = 0; i < allCurrentCategoryAttributes.Count; i++)
            {
                var catgoryAttribute = allCurrentCategoryAttributes[i];

                if (catgoryAttribute.AttributeType == nameof(AllAttributeTypes.number))
                {
                    allCurrentCategoryAttributes.Insert(i, new CategoryAttributeVM() { Name = catgoryAttribute.Name, AttributeType = nameof(AllAttributeTypes.numberTo), Id = catgoryAttribute.Id, ViewFilterType = catgoryAttribute.ViewFilterType });
                    allCurrentCategoryAttributes.Insert(i, new CategoryAttributeVM() { Name = catgoryAttribute.Name, AttributeType = nameof(AllAttributeTypes.numberFrom), Id = catgoryAttribute.Id, ViewFilterType = catgoryAttribute.ViewFilterType });
                    allCurrentCategoryAttributes.Remove(catgoryAttribute);
                    i++;
                }
                else if (catgoryAttribute.AttributeType == nameof(AllAttributeTypes.date))
                {
                    allCurrentCategoryAttributes.Insert(i, new CategoryAttributeVM() { Name = catgoryAttribute.Name, AttributeType = nameof(AllAttributeTypes.dateTo), Id = catgoryAttribute.Id, ViewFilterType = catgoryAttribute.ViewFilterType });
                    allCurrentCategoryAttributes.Insert(i, new CategoryAttributeVM() { Name = catgoryAttribute.Name, AttributeType = nameof(AllAttributeTypes.dateFrom), Id = catgoryAttribute.Id, ViewFilterType = catgoryAttribute.ViewFilterType });
                    allCurrentCategoryAttributes.Remove(catgoryAttribute);
                    i++;
                }
                else if (catgoryAttribute.AttributeType == nameof(AllAttributeTypes.list) || catgoryAttribute.AttributeType == nameof(AllAttributeTypes.multiSelect))
                {
                    var comboValues = new List<AttributeValueListVM>();
                    foreach (var product in allProducts)
                    {
                        var foundAttr = product.Attributes.FirstOrDefault(x => x.CategoryAttribute.Name == catgoryAttribute.Name &&
                                                    x.CategoryAttribute.AttributeType == nameof(AllAttributeTypes.text));

                        if (foundAttr != null)
                        {
                            var v = new AttributeValueListVM();
                            v.Value = foundAttr.Value;
                            v.Id = foundAttr.ProductAttributeId;

                            if (!comboValues.Any(cv => cv.Value == v.Value))
                            {
                                comboValues.Add(v);
                            }
                        }

                        //catgoryAttribute.ComboboxValues = comboValues;
                    }

                    catgoryAttribute.ComboboxValues = comboValues;
                }
            }

            //Fill values depends on ViewFilterType
            for (int i = 0; i < allCurrentCategoryAttributes.Count; i++)
            {
                var filter = allCurrentCategoryAttributes[i];
                switch (filter.ViewFilterType)
                {
                    case nameof(ViewFilterTypes.text):
                        break;
                    case nameof(ViewFilterTypes.list):
                    case nameof(ViewFilterTypes.multiSelect):
                    case nameof(ViewFilterTypes.multiSelectOpened):

                        if (filter.AttributeType == nameof(ViewFilterTypes.bit))
                        {
                            filter.Bit = Convert.ToBoolean(filter.Value);
                        }
                        else
                        {
                            var comboValues = new List<AttributeValueListVM>();
                            foreach (var product in allProducts)
                            {
                                //Here is no attributes -> have to load them earlier
                                var foundAttr = product.Attributes?.FirstOrDefault(x => x.CategoryAttribute.Name == filter.Name);

                                if (foundAttr != null && !string.IsNullOrEmpty(foundAttr.Value))
                                {
                                    var v = new AttributeValueListVM();
                                    v.Value = foundAttr.Value;
                                    v.Id = foundAttr.ProductAttributeId;
                                    if (!comboValues.Any(cv => cv.Value == v.Value))
                                    {
                                        comboValues.Add(v);
                                    }
                                }
                            }

                            filter.ComboboxValues = comboValues;
                        }

                        break;
                }
            }

            //var tmpList = allCurrentCategoryAttributes.Distinct(new CategoryAttrComparer()).ToList();

            return allCurrentCategoryAttributes.Distinct(new CategoryAttrComparer()).ToList();
        }

        internal class CategoryAttrComparer : IEqualityComparer<CategoryAttributeVM>
        {
            public bool Equals([AllowNull] CategoryAttributeVM x, [AllowNull] CategoryAttributeVM y)
            {
                return x.Name == y.Name && x.AttributeType == y.AttributeType && x.ViewFilterType == y.ViewFilterType;
            }

            public int GetHashCode([DisallowNull] CategoryAttributeVM obj)
            {
                return (obj.Name + obj.AttributeType + obj.ViewFilterType).GetHashCode();
            }
        }

        public List<CategoryAttributeVM> FillViewFilters(List<CategoryAttributeVM> emptyFilters, List<CategoryAttributeVM> urlFiltersX)
        {
            var result = new List<CategoryAttributeVM>();
            foreach (var emptyFilter in emptyFilters)
            {
                var urlFilters = urlFiltersX.Where(x => x.Name == emptyFilter.Name);

                foreach (var urlFilter in urlFilters)
                {
                    //have to change to Name of enum
                    //if (emptyFilter.ViewFilterType == "text" && emptyFilter.AttributeType == "dateTo")
                    //{
                    //    emptyFilter.dateTo = urlFilter.Value;
                    //    emptyFilter.dateRange = new[] { urlFilter.Value };
                    //}
                    //if (emptyFilter.ViewFilterType == "text" && emptyFilter.AttributeType == "dateFrom" && urlFilter.AttributeType == nameof(AllAttributeTypes.from))
                    //{
                    //    emptyFilter.dateFrom = urlFilter.Value;
                    //    emptyFilter.dateRange = new[] { urlFilter.Value };
                    //}
                    //else
                    if (emptyFilter.ViewFilterType == "text" && emptyFilter.AttributeType == "numberFrom" && urlFilter.AttributeType == nameof(AllAttributeTypes.from))
                    {
                        emptyFilter.Value = urlFilter.Value;
                    }
                    else if (emptyFilter.ViewFilterType == "text" && emptyFilter.AttributeType == "numberTo" && urlFilter.AttributeType == nameof(AllAttributeTypes.to))
                    {
                        emptyFilter.Value = urlFilter.Value;
                    }
                    else if (emptyFilter.ViewFilterType == "multiSelect")
                    {
                        foreach (var item in emptyFilter.ComboboxValues)
                        {
                            if (item.Value.ToLower() == urlFilter.Value.ToLower())
                            {
                                item.Checked = true;
                                if (emptyFilter.SelectedValues == null)
                                {
                                    emptyFilter.SelectedValues = new List<AttributeValueListVM>();
                                }

                                emptyFilter.SelectedValues.Add(item);
                            }
                        }
                    }
                    else if (emptyFilter.ViewFilterType == "list")
                    {
                        if (emptyFilter.AttributeType == nameof(AllAttributeTypes.bit))
                        {
                            emptyFilter.Bit = true;
                        }
                        else
                        {
                            //listbox
                            foreach (var item in emptyFilter.ComboboxValues)
                            {
                                if (item.Value.ToLower() == urlFilter.Value.ToLower())
                                {
                                    item.Checked = true;
                                    if (emptyFilter.SelectedValues == null)
                                    {
                                        emptyFilter.SelectedValues = new List<AttributeValueListVM>();
                                    }

                                    emptyFilter.SelectedValues.Add(item);
                                }
                            }
                        }
                    }
                    else if (emptyFilter.ViewFilterType == "date")
                    {
                        foreach (var item in emptyFilter.ComboboxValues)
                        {
                            if (item.Value == urlFilter.Value)
                            {
                                item.Checked = true;
                            }
                        }
                    }

                    //result.Add(emptyFilter);
                    //}
                    //else
                    //{
                    //    result.Add(emptyFilter);
                    //}
                }

                result.Add(emptyFilter);
            }

            return result;
        }

        public List<Product> GetAllProductsEntities(IEnumerable<Category> categories)
        {
            var result = new List<Product>();

            foreach (var category in categories)
            {
                db.Entry(category).Collection(c => c.Product).Query().Include(p => p.Picture).Include(p => p.ProductAttribute).ThenInclude(attr => attr.CategoryAttribute).Load();
                result.AddRange(category.Product.Where(p => !p.Removed && !p.Hidden && (p.DateTo >= DateTime.Now || p.DateTo == null)));
            }

            return result;
        }

        public ICollection<Product> GetProductsByCategory(Category category, List<Product> result = null)
        {
            if (category == null) return null;

            if (result == null)
                result = new List<Product>();

            if (category.Product?.Count > 0)
            {
                result.AddRange(category.Product?.Where(p => !p.Removed && !p.Hidden && (p.DateTo >= DateTime.Now || p.DateTo == null)));
            }

            if (category.InverseParent != null)
            {
                foreach (var catItem in category.InverseParent)
                {
                    GetProductsByCategory(catItem, result);
                }
            }

            return result;
        }

        public ICollection<Product> GetProductsByCategoryId(int categoryId, List<Product> result = null)
        {
            var category = db.Category.Include(c => c.Product).FirstOrDefault(c => c.CategoryId == categoryId);
            if (category == null) return null;

            if (result == null)
                result = new List<Product>();

            if (category.Product?.Count > 0)
            {
                result.AddRange(category.Product?.Where(p => !p.Removed && !p.Hidden && (p.DateTo >= DateTime.Now || p.DateTo == null)));
            }

            if (category.InverseParent != null)
            {
                foreach (var catItem in category.InverseParent)
                {
                    GetProductsByCategoryId(catItem.CategoryId, result);
                }
            }

            return result;
        }

        public void RecursiveLoadCategories(Category parent)
        {
            db.Entry(parent).Collection(d => d.InverseParent).Load();
            db.Entry(parent).Collection(d => d.Product).Query()
                            .Include(p => p.ProductAttribute)
                            .Include(p => p.City)
                            .Where(p => !p.Removed && !p.Hidden && (p.DateTo >= DateTime.Now || p.DateTo == null)).Load();

            foreach (var child in parent.InverseParent)
            {
                RecursiveLoadCategories(child);
            }
        }

        #endregion
    }
}
