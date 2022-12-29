using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using targeteo.pl.Helpers;
using targeteo.pl.Model.ViewModel;
using targeteo.pl.Model;
using targeteo.pl.Model.Entities;
using static targeteo.pl.Common.Enums;
using AllowAnonymousAttribute = Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace targeteo.pl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class HomeController : BaseController
    {
        
        private readonly string _angularAssetsPath;
        private readonly IndexHelper _helper;
        private readonly IMemoryCache _memoryCache;
        private readonly MemoryCacheEntryOptions cacheExpiryOptions;

        public HomeController(IMapper mapper, IWebHostEnvironment hostingEnvironment, IConfiguration config, IMemoryCache memoryCache, ApplicationDbContext db) 
            : base(config, db, mapper, hostingEnvironment)
        {
            _angularAssetsPath = config["AngularAssetsPath"];
            _helper = new IndexHelper(_db);
            _memoryCache = memoryCache;

            cacheExpiryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddSeconds(300),
                Priority = CacheItemPriority.High,
                SlidingExpiration = TimeSpan.FromSeconds(150)
            };
        }

        //this have to be refactore because it is done by every request !!!!!!!!!!!!!!!!!!!!!!!!!
        private List<CategoryAttributeVM> _currentAllCategoryFilters = new List<CategoryAttributeVM>();

        /// <summary>
        /// example path:
        /// https://localhost:44341/api/Home/Index/cat1/cat2
        ///http://localhost:50648/kategoria/2037/filtrowanie?Color=red
        ///can be wrong port
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="attrs"></param>
        /// <returns></returns>
        [Route("Index/{parentCategory}/{childCategory}")]
        [Route("Index/{parentCategory}")]
        [Route("Index")]
        [HttpGet]
        public async Task<IActionResult> Index(string parentCategory, string childCategory)
        {
            var filtersOn = childCategory == "filtrowanie";

            string searchString = Request.Query["searchString"];
            if(!string.IsNullOrEmpty(searchString))
            {
                var searchStringSplited = searchString.Split(',');
                searchString = searchStringSplited[0];
                if(searchStringSplited.Length > 1)
                {
                    var categoryFromParam = searchStringSplited[1];

                    if (string.IsNullOrEmpty(childCategory))
                    {
                        childCategory = categoryFromParam;
                    }
                }
            }
            var city = Request.Query["city"];
            double latitude = 0;
            double longitude = 0;
            //Double latitude = Convert.ToDouble(Request.Query["lat"], CultureInfo.InvariantCulture);
            //Double longitude = Convert.ToDouble(Request.Query["lon"], CultureInfo.InvariantCulture);
            Double? distance = Convert.ToDouble(Request.Query["distance"], CultureInfo.InvariantCulture);

            if(distance > 0)
            {
                distance += 5;
            }
            var orderBy = Request.Query["orderBy"];

            Category currentCategory = null;
            IQueryable<Category> qcurrentCategory = null;
            ICollection<Product> products = new List<Product>();
            bool hasChild = !string.IsNullOrEmpty(childCategory) && childCategory != "null" && !filtersOn;

            #region get lat lon from external servie
            //this have to be extracted to the method !!!!!!!!!!!

            HttpClient client = new HttpClient();
            //client.Headers.Add("User-Agent: Other");
            client.BaseAddress = new Uri("https://nominatim.openstreetmap.org/search");

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            var productValue = new ProductInfoHeaderValue("User-Agent", "Other");
            client.DefaultRequestHeaders.UserAgent.Add(productValue);

            //querystring
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["q"] = city;
            query["format"] = "json";
            query["countrycode"] = "pl";
            string queryString = "?" + query.ToString();

            // List data response.
            HttpResponseMessage response = await client.GetAsync(queryString);  // Blocking call! Program will wait here until a response is received or a timeout occurs.
            if (response.IsSuccessStatusCode)
            {
                var obj = response.Content.ReadAsAsync<dynamic>().Result;
                // Parse the response body.
                dynamic dataObjects = JsonConvert.DeserializeObject(obj.ToString());  //Make sure to add a reference to System.Net.Http.Formatting.dll

                if (dataObjects.Count > 0)
                {
                    latitude = dataObjects[0].lat;
                    longitude = dataObjects[0].lon;
                }
            }

            #endregion

            if (!hasChild)
            {
                //Here have to be from db because after that I want to load attributes and products only for that entity
                currentCategory = _db.Category.Include(c => c.Product).FirstOrDefault(x => x.Name == parentCategory);

                //Here have to be from db because after that I want to load attributes and products only for that entity
                qcurrentCategory = _db.Category.Where(x => x.Name == parentCategory);

                //db.Entry(product).Collection(d => d.Attributes).Load();
                //change everywhere firstOrdDefault to find by id !!!!!
            }
            else
            {
                Category parent = _db.Category.Include(c => c.InverseParent)
                                                    //.Include(c => c.Products).ThenInclude(p => p.City)
                                                    //.Include(c => c.Products).ThenInclude(p => p.Attributes).ThenInclude(a => a.CategoryAttribute)


                                                    //.Include(c => c.Parent)
                                                    //.ThenInclude(c => c.Products).ThenInclude(p => p.Attributes).ThenInclude(a => a.CategoryAttribute)
                                                    //.Include(c => c.Products).ThenInclude(p => p.City)

                                                    //.Include(p => p.Parent)
                                                    //.ThenInclude(c => c.Parent)
                                                    //.ThenInclude(c => c.Products).ThenInclude(p => p.Attributes).ThenInclude(a => a.CategoryAttribute)
                                                    //.Include(c => c.Products).ThenInclude(p => p.City)

                                                    //.Include(c => c.Categories).ThenInclude(c => c.Products).ThenInclude(p => p.City)
                                                    //.Include(c => c.Categories).ThenInclude(c => c.Attributes)
                                                    //.Include(c => c.Categories).ThenInclude(c => c.Categories).ThenInclude(c => c.Products).ThenInclude(p => p.City)
                                                    //.Include(c => c.Categories).ThenInclude(c => c.Categories).ThenInclude(c => c.Attributes)

                                        .FirstOrDefault(p => p.InverseParent.Any(c => c.Name == childCategory));
                //change everywhere firstOrdDefault to find by id !!!!!

                if (parent != null)
                {
                    var child = parent.InverseParent.FirstOrDefault(p => p.Name == childCategory);

                    if (child != null)
                    {
                        currentCategory = child;
                    }
                    else
                    {
                        currentCategory = parent;
                    }
                }
            }


            //when is use this recuirsive function ????????????????????? because it takes a lot of time
            //this if else i s to remove

            //this is use for searchtext to search all products have to be changed toiqueryable
            //and its for searching by attributes
            //have to load categories down

            //cache that
            var cacheKey = "categoriyTreeNode";
            //checks if cache entries exists
            if (!_memoryCache.TryGetValue(cacheKey, out List<TreeNodeVm> mobilePhoneMenu))
            {
                var cats1 = _db.Category.Include(c => c.InverseParent).ThenInclude(x => x.InverseParent);
                var catsMenuLocal = cats1.Where(x => x.Parent == null);

                mobilePhoneMenu = _mapper.Map<List<TreeNodeVm>>(catsMenuLocal);
                _memoryCache.Set(cacheKey, mobilePhoneMenu, cacheExpiryOptions);
            }

            if (currentCategory != null)
            {
                //and its for searching by attributes
                _helper.RecursiveLoadCategories(currentCategory);
            }
            else
            {

                //code never comes here !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                //}

                //if (currentCategory == null)
                //{
                //Handle SEARCHSTRING

                //this is to refactor !!!!!!!!!!!!!!
                ICollection<ProductMainPageVM> productsUnsorted = null;

                if (string.IsNullOrEmpty(searchString) && distance != 0)
                {
                    productsUnsorted = _mapper.Map<ICollection<ProductMainPageVM>>(_db.Product.Include(p => p.Picture)
                                                                                    .Include(p => p.City)
                                                                                    .Include(p => p.Category)
                                                                                    .Where(p => !p.Removed && !p.Hidden && p.QuantityInStock > 0 && (p.DateTo >= DateTime.Now || p.DateTo == null) &&
                                                                                        _db.udf_Haversine(p.City.Latitude, p.City.Longitude, latitude, longitude) < distance));
                }
                else if (!string.IsNullOrEmpty(searchString) && distance == 0)
                {
                    productsUnsorted = _mapper.Map<ICollection<ProductMainPageVM>>(_db.Product.Include(p => p.Picture)
                                                                                    .Include(p => p.City)
                                                                                    .Include(p => p.Category)
                                                                                    .Where(p => p.Title.Contains(searchString) && !p.Removed && !p.Hidden && p.QuantityInStock > 0 && (p.DateTo >= DateTime.Now || p.DateTo == null)));
                }
                else
                {
                    productsUnsorted = _mapper.Map<ICollection<ProductMainPageVM>>(_db.Product.Include(p => p.Picture)
                                                                                    .Include(p => p.City)
                                                                                    .Include(p => p.Category)
                                                                                    .Where(p => p.Title.Contains(searchString) && !p.Removed && !p.Hidden && p.QuantityInStock > 0 && (p.DateTo >= DateTime.Now || p.DateTo == null) &&
                                                                                    _db.udf_Haversine(p.City.Latitude, p.City.Longitude, latitude, longitude) < distance));
                }

                switch (orderBy)
                {
                    case "priceAsc":
                        productsUnsorted = productsUnsorted.OrderBy(x => x.Price).ToList();
                        break;
                    case "priceDesc":
                        productsUnsorted = productsUnsorted.OrderByDescending(x => x.Price).ToList();
                        break;
                    case "newest":
                        throw new NotImplementedException();
                }


                var emptyVM = new HomeVM()
                {
                    //this is no needed ?
                    //Categories = _mapper.Map<List<CategoryVM>>(cats1),
                    //this one should be cast on client machine
                    CategoriesTreeNode = mobilePhoneMenu,
                    //CurrentAttributes = new List<CategoryAttributeVM>(),
                    //MenuItems = _mapper.Map<ICollection<MenuItem>>(catsMenu),
                    Products = productsUnsorted.Where(p => p.QuantityInStock > 0).ToList(),
                    //CurrentCategoryBreadCrumb = _mapper.Map<MenuItem>(currentCategory)
                };

                return Ok(emptyVM);
            };

            #region SEARCH_PRODUCTS_BY_ATTRIBUTES

            //CHange to work only on entities
            List<CategoryAttributeVM> urlFilters = GetUrlFilters();
            List<List<CategoryAttributeVM>> urlFiltersGrouped = GetUrlFiltersGrouped();
            
            IEnumerable<Category> categoriesFlattenEnt = _helper.GetFlattenCategories(currentCategory);


            //pacj it and cache one object on one key
            cacheKey = currentCategory.Name;
            //checks if cache entries exists
            //if (!_memoryCache.TryGetValue(cacheKey, out List<CategoryAttributeVM > viewFiltersEmpty))
            //{
            //calling the server
            //List<CategoryVM> categoriesFlatten = _mapper.Map<List<CategoryVM>>(_helper.GetFlattenCategories(currentCategory).ToList());

            var categoriesFlatten = _mapper.Map<List<CategoryVM>>(categoriesFlattenEnt);
                List<CategoryAttributeVM> viewFiltersEmpty = _helper.GetViewFilters(categoriesFlatten);
                //_memoryCache.Set(cacheKey, viewFiltersEmpty, cacheExpiryOptions);

            //}
                
            List<CategoryAttributeVM> viewFiltersFilled = _helper.FillViewFilters(viewFiltersEmpty, urlFilters);


            #region get products from all categories
            List<Product> viewProductsEntities = new List<Product>();
            foreach (var category in categoriesFlattenEnt.Where(p => p.Product.Count > 1))
            {
                viewProductsEntities.AddRange(category.Product.Where(p => !p.Removed && !p.Hidden && (p.DateTo >= DateTime.Now || p.DateTo == null)));
            }
            //List<CategoryEntity> viewProductsEntities = categoriesFlattenEnt.Where(c => c.Products.Any(p => !p.Removed && !p.Hidden && (p.DateTo >= DateTime.Now || p.DateTo == null))).ToList();
            //List<ProductEntity> viewProductsEntities = categoriesFlattenEnt.Where(c => c.Products.Any(p => !p.Removed && !p.Hidden && (p.DateTo >= DateTime.Now || p.DateTo == null))).Select(c => c.Products).ToList();
            #endregion


            //category have to be flatten because cannot get children products when category is on higher level
            //can add condtition if(last leve dont do that)
            _helper.GetAllProductsEntities(categoriesFlattenEnt);



            if (urlFilters.Count > 0)
            {
                foreach (var product in viewProductsEntities)
                {
                    var addToProductList = false;
                    bool skipRestAttrs = false;
                    foreach (var urlFilterList in urlFiltersGrouped)
                    {

                        if (skipRestAttrs)
                        {
                            //skipRestAttrs = false;
                            continue;
                        }

                        bool atLeastOneFoundInUrlFilterList = false;

                        foreach (var urlFilter in urlFilterList)
                        {

                            #region special when url filter is searchstring or location
                            var isLocationInRange = _db.udf_Haversine(product.City.Latitude, product.City.Longitude, latitude, longitude) < Convert.ToDouble(distance);
                            if (urlFilter.Name == "searchString" && product.Title.Contains(urlFilter.Value) && distance != 0 && isLocationInRange && product.QuantityInStock > 0)
                            {
                                addToProductList = true;
                                atLeastOneFoundInUrlFilterList = true;
                                break;
                            }
                            else
                            {
                                if (urlFilter.Name == "searchString" && product.Title.Contains(urlFilter.Value) && distance == 0)
                                {
                                    addToProductList = true;
                                    atLeastOneFoundInUrlFilterList = true;
                                    break;
                                }
                            }

                            #endregion

                            if (atLeastOneFoundInUrlFilterList)
                            {
                                break;
                            }

                            DateTime date;
                            double number;
                            Boolean boolean;
                            var value = urlFilter.Value.Split("_").FirstOrDefault();
                            var isDatetime = DateTime.TryParse(value, out date);
                            var isNumber = double.TryParse(value, out number);
                            var isBool = Boolean.TryParse(value, out boolean);
                            //var isFrom = urlFilter.Value.EndsWith("_od");
                            //var isTo = urlFilter.Value.EndsWith("_do");

                            _db.Entry(product).Collection(d => d.ProductAttribute).Load();
                            var foundAttrs = product.ProductAttribute.Where(attr => attr.CategoryAttribute.Name == urlFilter.Name);


                            ////has no attribute like that
                            //if (foundAttrs.Count() == 0)
                            //{
                            //    products.Remove(product);
                            //    addToProductList = false;
                            //    skipRestAttrs = true;
                            //    break;
                            //}

                            //Go through found attributes
                            foreach (var attr in foundAttrs)
                            {
                                #region date check
                                if (isDatetime)
                                {

                                    var isRange = urlFilters.Count(urlf => urlf.Name == urlFilter.Name) > 1;
                                    if (!isRange && attr.Value == urlFilter.Value)
                                    {
                                        if (urlFilter == urlFilters.Last())
                                        {
                                            products.Add(product);
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        string smallerDateTmp = urlFilters.Where(f => f.Name == urlFilter.Name).Min(x => x.Value);
                                        string biggerDateTmp = urlFilters.Where(f => f.Name == urlFilter.Name).Max(x => x.Value);

                                        DateTime smallerDate = Convert.ToDateTime(smallerDateTmp, CultureInfo.InvariantCulture);
                                        DateTime biggerDate = Convert.ToDateTime(biggerDateTmp, CultureInfo.InvariantCulture);

                                        //This not always can be number
                                        DateTime produAttrValue = DateTime.ParseExact(attr.Value, "dd.MM.yyyy", new CultureInfo("pl-PL"));

                                        var isInRange = produAttrValue >= smallerDate && produAttrValue <= biggerDate;
                                        if (isInRange)
                                        {
                                            //this is because range has double check in from attr and to attr
                                            if (!products.Contains(product))
                                            {
                                                //this is because range has double check in from attr and to attr
                                                if (!products.Contains(product))
                                                {
                                                    addToProductList = true;
                                                    atLeastOneFoundInUrlFilterList = true;
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            addToProductList = false;
                                        }
                                    }
                                }
                                #endregion date check
                                #region number check
                                else if (isNumber)
                                {
                                    var isRange = urlFilters.Count(urlf => urlf.Name == urlFilter.Name) > 1;
                                    if (!isRange)
                                    {
                                        products.Add(product);
                                    }
                                    else
                                    {
                                        string smallerNumberTmp = urlFilters.Where(f => f.Name == urlFilter.Name).OrderBy(x => double.Parse(x.Value)).First().Value;
                                        string biggerNumberTmp = urlFilters.Where(f => f.Name == urlFilter.Name).OrderBy(x => double.Parse(x.Value)).Last().Value;

                                        double smallerNumber = Convert.ToDouble(smallerNumberTmp);
                                        double biggerNumber = Convert.ToDouble(biggerNumberTmp);

                                        //This not always can be number
                                        double produAttrValue = Convert.ToDouble(attr.Value);

                                        var isInRange = produAttrValue >= smallerNumber && produAttrValue <= biggerNumber;
                                        if (isInRange)
                                        {
                                            //this is because range has double check in from attr and to attr
                                            if (!products.Contains(product))
                                            {
                                                addToProductList = true;
                                                atLeastOneFoundInUrlFilterList = true;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            addToProductList = false;
                                        }
                                    }
                                }
                                #endregion number check
                                #region bool check
                                else if (isBool)
                                {
                                    if (attr.Value.ToLower() == urlFilter.Value.ToLower())
                                    {
                                        if (!products.Contains(product))
                                        {
                                            addToProductList = true;
                                            //atLeastOneFoundInUrlFilterList = true;
                                            break;
                                        }

                                        //products.Add(product);
                                        //break;
                                    }
                                    else
                                    {
                                        addToProductList = false;
                                    }
                                }
                                #endregion
                                #region else
                                else
                                {
                                    var hasSameValue = attr.Value.ToLower() == urlFilter.Value.ToLower();
                                    if (hasSameValue)
                                    {
                                        if (!products.Contains(product))
                                        {
                                            addToProductList = true;
                                            atLeastOneFoundInUrlFilterList = true;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        addToProductList = false;
                                    }
                                }
                                #endregion

                                //is last url filter iteration
                                if (urlFilterList.Last() == urlFilter && !atLeastOneFoundInUrlFilterList)
                                {
                                    skipRestAttrs = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (addToProductList)
                    {
                        products.Add(product);
                    }
                }
            }
            else
            {
                #region orderby
                //var productsUnsorted = GetProductsByCategory(currentCategory) ?? new List<ProductEntity>();
                var productsUnsorted = _helper.GetProductsByCategoryId(currentCategory.CategoryId) ?? new List<Product>();
                switch (orderBy)
                {
                    case "priceAsc":
                        productsUnsorted = productsUnsorted.OrderBy(x => x.Price).ToList();
                        break;
                    case "priceDesc":
                        productsUnsorted = productsUnsorted.OrderByDescending(x => x.Price).ToList();
                        break;
                    case "newest":
                        throw new NotImplementedException();
                }
                #endregion
                products = productsUnsorted;
            }

            #endregion SEARCH_PRODUCTS_BY_ATTRIBUTES;


            HomeVM homeVm = new HomeVM()
            {
                //Current category is needed only for editing catregory, have to do something with that because takes a lot of paylod
                CurrentCategory = _mapper.Map<CategoryVM>(currentCategory),
                CategoriesTreeNode = mobilePhoneMenu,
                Products = _mapper.Map<ICollection<ProductMainPageVM>>(products.Where(p => !p.Removed && !p.Hidden && p.QuantityInStock > 0 && (p.DateTo >= DateTime.Now || p.DateTo == null))),
                CurrentAttributes = viewFiltersFilled,
                CurrentCategoryBreadCrumb = _mapper.Map<MenuItem>(currentCategory)
            };

            return Ok(homeVm);
        }

        [Route("GetMenu")]
        [HttpGet]
        public async Task<IActionResult> GetMenu()
        {
            var cacheKey = "menu";
            //checks if cache entries exists
            if (!_memoryCache.TryGetValue(cacheKey, out List<MenuItem> menu))
            {
                var menuLocal = await _db.Category
                                    .Include(c => c.InverseParent)
                                    .ThenInclude(c => c.InverseParent)
                                    .Where(x => x.Parent == null).ToListAsync();

                menu = _mapper.Map<List<MenuItem>>(menuLocal);
                _memoryCache.Set(cacheKey, menu, cacheExpiryOptions);
            }
                
            return Ok(menu);
        }

        //this is redundant
        private List<CategoryVM> GetCategoriesHierarchy()
        {
            var cats = _db.Category.ToList();

            for (int i = 0; i < cats.Count(); i++)
            {
                var obj = cats[i];
                if (obj.Parent != null && (obj.InverseParent == null || obj.InverseParent?.Count == 0))
                {
                    cats.Remove(obj);
                    i--;
                }
            }

            return _mapper.Map<List<CategoryVM>>(cats);
        }

        [Route("Index/get-products-contains")]
        [HttpGet]
        public IActionResult GetProductsContainsName(string productName)
        {
            IEnumerable<Product> entities = null;
            if (productName == "")
            {
                //here have to be select because include two times category
                entities = _db.Product.Include(p => p.Category).ThenInclude(c => c.Parent).Where(p => !p.Removed && !p.Hidden && p.QuantityInStock > 0 && (p.DateTo >= DateTime.Now || p.DateTo == null)).DistinctBy(x => x.Title);
            }
            else
            {
                //here have to be select because include two times category
                entities = _db.Product.Include(p => p.Category).ThenInclude(c => c.Parent).Where(p => p.Title.Contains(productName) && !p.Removed && !p.Hidden && p.QuantityInStock > 0 && (p.DateTo >= DateTime.Now || p.DateTo == null)).DistinctBy(x => x.Title);
            }


            var res = _mapper.Map<ICollection<ProductVM>>(entities);

            return Ok(res);
        }

        [Route("Index/get-product")]
        [HttpGet]
        public IActionResult GetProduct(string id)
        {
            return Ok(GetAddProductModel(id));
        }

        [Route("Index/get-bestSellers")]
        [HttpGet]
        public IActionResult GetBestSellers(int length)
        {
            //var products = new List<ProductVM>();
            var products = _db.OrderDetail.Include(o => o.Product)
                                        .ThenInclude(p => p.Picture).OrderByDescending(o => o.Quantity).Select(o => o.Product).Where(p => !p.Removed && !p.Hidden && (p.DateTo >= DateTime.Now || p.DateTo == null)).DistinctBy(p => p.ProductId).Take(length);

            if (products.Count() == 0)
            {
                products = _db.Product.Include(p => p.Picture).Where(p => !p.Removed && !p.Hidden).Take(5);
            }

            return Ok(_mapper.Map<ICollection<ProductVM>>(products));
        }

        [Route("Index/get-lastAddedProducts")]
        [HttpGet]
        public IActionResult GetLastAddedProducts(int length)
        {
            var cacheKey = "lastAddedProducts";
            //checks if cache entries exists
            if (!_memoryCache.TryGetValue(cacheKey, out List<ProductVM> lastAddedProducts))
            {
                var products = _db.Product.Include(p => p.Picture).OrderByDescending(o => o.CreationDate).Where(p => !p.Removed && !p.Hidden && p.QuantityInStock > 0 && (p.DateTo >= DateTime.Now || p.DateTo == null)).DistinctBy(p => p.ProductId).Take(length);
                lastAddedProducts = _mapper.Map<List<ProductVM>>(products);
                _memoryCache.Set(cacheKey, lastAddedProducts, cacheExpiryOptions);
            }

            return Ok(lastAddedProducts);
        }


        /// <summary>
        ///     return alll atrrs that contains current category and all parents (not siblings and not parents siblings)
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-category-attrs")]
        public ICollection<CategoryAttributeVM> GetAllCategoryAttrs(int categoryId)
        {
            Category catEnt = _db.Category.Include(x => x.CategoryAttributes).Include(x => x.Product).First(x => x.CategoryId == categoryId);

            #region all filled attributes
            List<CategoryVM> categoriesFlatten = _mapper.Map<List<CategoryVM>>(_helper.GetFlattenCategoriesUp(catEnt).ToList());
            //IEnumerable<CategoryEntity> categoriesFlattenEnt = GetFlattenCategories(catEnt);
            //List<CategoryAttributeVM> viewFiltersEmpty = GetViewFilters(categoriesEnt);

            List<CategoryAttributeVM> viewFiltersEmpty = _helper.GetViewFilters(categoriesFlatten);

            var res = new List<CategoryAttributes>();
            //var attrsUp = GetCategoryAttrsUp(catEnt, ref res);

            //foreach (var item in attrsUp)
            //{
            //    var x = item;
            //}
            //List<CategoryAttributeVM> viewFiltersFilled = FillViewFilters(viewFiltersEmpty, urlFilters);
            #endregion

            //Gives all attrs but only to up direction
            IEnumerable<CategoryAttributeVM> categoryUpAttrs = viewFiltersEmpty.DistinctBy(x => x.Name);
            return categoryUpAttrs.ToList();
        }

        //Gives all attrs but only to up direction
        private IEnumerable<CategoryAttributeVM> GetCategoryAttrs(Category currentCategory)
        {
            var flattened = new List<CategoryAttributeVM>();
            _db.Entry(currentCategory).Collection(r => r.CategoryAttributes).Load();
            flattened.AddRange(_mapper.Map<IEnumerable<CategoryAttributeVM>>(currentCategory.CategoryAttributes));

            _db.Entry(currentCategory).Reference(r => r.Parent).Load();
            var parent = currentCategory.Parent;

            if (parent != null)
            {
                GetCategoryAttrs(parent);
            }

            return flattened;
        }

        [Route("Index/get-image")]
        [HttpGet]
        public IActionResult GetImage(string fileName)
        {
            fileName = "aaa.jpg";
            string folderName = "Upload";

            string webRootPath = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            string fullPath = Path.Combine(newPath, fileName);

            return PhysicalFile(fullPath, "image/jpeg");
        }

        private Task SendEmailToClient()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("Manage/saveViewFilter")]
        public ActionResult SaveViewFilter(int categoryAttrId, string viewFilterType, bool hide)
        {
            var ent = _db.CategoryAttributes.Find(categoryAttrId);

            ent.ViewFilterType = viewFilterType;
            ent.Hide = hide;

            _db.CategoryAttributes.Update(ent);

            _db.SaveChanges();

            return Ok(true);
        }

        [HttpGet]
        [Route("getuseropinions")]
        public ActionResult GetUserOpinions(string userId)
        {
            var opinions = _db.OrderOpinion.Include(x => x.Picture)
                                            .Include(x => x.OrderDetail).ThenInclude(d => d.Product).ThenInclude(p => p.AspNetUser)
                                            .Where(x => x.OrderDetail.Product.AspNetUserId == userId);
            return Ok(opinions);
        }

        #region private method that has acces to request

        private List<List<CategoryAttributeVM>> GetUrlFiltersGrouped()
        {
            List<CategoryAttributeVM> resultFilters = new List<CategoryAttributeVM>();

            var queryString = Request.Query;

            //Select filers from querstring
            if (Request.QueryString.HasValue)
            {
                foreach (var key in queryString.Keys)
                {
                    if (key.Equals(nameof(AllAttributeTypes.filtrowanie)) || key.Equals(nameof(AllAttributeTypes.orderBy)))
                    {
                        continue;
                    }

                    var filterType = _helper.GetFilterType(key);
                    var filterName = _helper.GetFilterName(key);

                    var filter = new CategoryAttributeVM();
                    switch (filterType)
                    {
                        case "From":
                            filter.Value = queryString[key];
                            filter.AttributeType = nameof(AllAttributeTypes.from);
                            filter.Name = filterName;
                            resultFilters.Add(filter);
                            break;
                        case "To":
                            filter.Value = queryString[key];
                            filter.AttributeType = nameof(AllAttributeTypes.to);
                            filter.Name = filterName;
                            resultFilters.Add(filter);
                            break;
                        default:
                            //filter.Value = queryString[key];
                            //filter.Name = filterName;
                            //filter.AttributeType = string.Empty;

                            foreach (var item in queryString[key].ToString().Split(','))
                            {

                                var f = new CategoryAttributeVM();
                                f.Name = filterName;
                                f.Value = item;
                                resultFilters.Add(f);
                            }

                            break;

                            //case nameof(AllAttributeTypes.list):
                            //case nameof(AllAttributeTypes.multiSelect):
                            //    {
                            //        filter.Value = queryString[key];
                            //        filter.AttributeType = key.Split('_')[0];
                            //        filter.Name = GetFilterName(key);
                            //        break;
                            //    }
                            //case nameof(AllAttributeTypes.dateFrom):
                            //    {
                            //        break;
                            //    }
                            //case nameof(AllAttributeTypes.dateTo):
                            //    {
                            //        break;
                            //    }
                            //case nameof(AllAttributeTypes.numberFrom):
                            //    {
                            //        filter.Value = queryString[key];
                            //        filter.AttributeType = nameof(AllAttributeTypes.numberFrom);
                            //        filter.Name = GetFilterName(key);
                            //        break;
                            //    }
                            //case nameof(AllAttributeTypes.numberTo):
                            //    {
                            //        filter.Value = queryString[key];
                            //        filter.AttributeType = nameof(AllAttributeTypes.numberTo);
                            //        filter.Name = filterName;
                            //        break;
                            //    }
                    }

                    //resultFilters.Add(filter);

                    //can be useful with ckecboxes
                    //var attr = new CategoryAttributeVM();
                    //var val = Request.Query[key];

                    //if(val.Count > 1)
                    //{
                    //    foreach (var v in val)
                    //    {
                    //        var attrSub = new CategoryAttributeVM();
                    //        attrSub.Name = key;
                    //        attrSub.Value = v;
                    //        resultFilters.Add(attrSub);
                    //    }
                    //}
                    //else
                    //{
                    //    attr.Name = key;
                    //    attr.Value = val;
                    //    resultFilters.Add(attr);
                    //}
                }
            }

            var grouped = _helper.FiltersGroupByName(resultFilters);

            return grouped;
        }

        /// <summary>
        /// Gets filters from URL string
        /// </summary>
        /// <param name="currentCategory"></param>
        /// <returns></returns>
        private List<CategoryAttributeVM> GetUrlFilters()
        {
            List<CategoryAttributeVM> resultFilters = new List<CategoryAttributeVM>();

            var queryString = Request.Query;

            //Select filers from querstring
            if (Request.QueryString.HasValue)
            {
                foreach (var key in queryString.Keys)
                {
                    if (key.Equals(nameof(AllAttributeTypes.filtrowanie)) || key.Equals(nameof(AllAttributeTypes.orderBy)))
                    {
                        continue;
                    }

                    var filterType = _helper.GetFilterType(key);
                    var filterName = _helper.GetFilterName(key);

                    var filter = new CategoryAttributeVM();
                    switch (filterType)
                    {
                        case "From":
                            filter.Value = queryString[key];
                            filter.AttributeType = nameof(AllAttributeTypes.from);
                            filter.Name = filterName;
                            resultFilters.Add(filter);
                            break;
                        case "To":
                            filter.Value = queryString[key];
                            filter.AttributeType = nameof(AllAttributeTypes.to);
                            filter.Name = filterName;
                            resultFilters.Add(filter);
                            break;
                        default:
                            //filter.Value = queryString[key];
                            //filter.Name = filterName;
                            //filter.AttributeType = string.Empty;

                            foreach (var item in queryString[key].ToString().Split(','))
                            {

                                var f = new CategoryAttributeVM();
                                f.Name = filterName;
                                f.Value = item;
                                resultFilters.Add(f);
                            }

                            break;

                            //case nameof(AllAttributeTypes.list):
                            //case nameof(AllAttributeTypes.multiSelect):
                            //    {
                            //        filter.Value = queryString[key];
                            //        filter.AttributeType = key.Split('_')[0];
                            //        filter.Name = GetFilterName(key);
                            //        break;
                            //    }
                            //case nameof(AllAttributeTypes.dateFrom):
                            //    {
                            //        break;
                            //    }
                            //case nameof(AllAttributeTypes.dateTo):
                            //    {
                            //        break;
                            //    }
                            //case nameof(AllAttributeTypes.numberFrom):
                            //    {
                            //        filter.Value = queryString[key];
                            //        filter.AttributeType = nameof(AllAttributeTypes.numberFrom);
                            //        filter.Name = GetFilterName(key);
                            //        break;
                            //    }
                            //case nameof(AllAttributeTypes.numberTo):
                            //    {
                            //        filter.Value = queryString[key];
                            //        filter.AttributeType = nameof(AllAttributeTypes.numberTo);
                            //        filter.Name = filterName;
                            //        break;
                            //    }
                    }
                }
            }

            return resultFilters;
        }

        //this is redundant
        private AddProductVM GetAddProductModel(string id)
        {
            var entity = _db.Product.Include(p => p.AspNetUser).
                                    ThenInclude(u => u.UserSupplyMethod.Where(sm => sm.IsActive == true)).
                                    ThenInclude(s => s.SupplyMethod).
                                    Include(x => x.Picture).
                                    Include(x => x.Category).
                                    Include(x => x.City).
                                    Include(x => x.ProductAttribute).
                                    ThenInclude(a => a.CategoryAttribute).
                                    FirstOrDefault(p => p.ProductId == id);

            var model = new AddProductVM();
            model.CurrentProduct = _mapper.Map<ProductVM>(entity);

            //this might be neccessarry in endit product
            //model.AllCategories = GetCategoriesHierarchy();
            model.NumberOfUserEvaluation = _db.OrderOpinion.Where(o => o.OrderDetail.Product.AspNetUserId == model.CurrentProduct.User.Id).Count();
            model.AvgUserEvaluation = 0;

            if (model.NumberOfUserEvaluation > 0)
            {
                model.AvgUserEvaluation = (int)_db.OrderOpinion.Where(o => o.OrderDetail.Product.AspNetUserId == model.CurrentProduct.User.Id).DefaultIfEmpty().Average(x => x.Evaluation);
            }

            model.NumberOfUserEvaluation = _db.OrderOpinion.Where(o => o.OrderDetail.Product.AspNetUserId == model.CurrentProduct.User.Id).Count();

            var category = _db.Category.Find(entity.Category.CategoryId);
            var catVm = _mapper.Map<CategoryVM>(category);

            model.CurrentProduct.Category = catVm;

            
            return model;
        }
        #endregion
    }
}