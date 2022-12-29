using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using targeteo.pl.Common;
using targeteo.pl.Helpers;
using targeteo.pl.Model;
using targeteo.pl.Model.Entities;
using targeteo.pl.Model.ViewModel;

namespace targeteo.pl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RootController : BaseController
    {
        private readonly string _angularAssetsPath;
        private readonly IndexHelper _helper;
        private List<CategoryAttributeVM> _currentAllCategoryFilters = new List<CategoryAttributeVM>();

        public RootController(IMapper mapper, IWebHostEnvironment hostingEnvironment, IConfiguration config, ApplicationDbContext db) 
            : base(config, db, mapper, hostingEnvironment)
        {
            _angularAssetsPath = config["AngularAssetsPath"];
            _helper = new IndexHelper(_db);
        }

        [HttpPost]
        [Route("Manage/editCategory")]
        public ActionResult EditCategory()
        {
            var viewMode = Request.Form["mode"][0];
            var modelJson = Request.Form["model"][0];
            CategoryVM category = JsonConvert.DeserializeObject<CategoryVM>(modelJson);

            //add picture
            try
            {
                var files = Request.Form.Files;
                string folderName = "upload";

                string assetsAngular = _angularAssetsPath;

                string newPath = Path.Combine(assetsAngular, folderName);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }

                string fileName = "";

                foreach (var file in files)
                {
                    fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).Name.Trim('"');
                    string fullPath = Path.Combine(newPath, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                        //category..Pictures.Add(new PictureVM() { Path = fileName, ProdId = category.CurrentProduct.ProductId });
                        category.ThumbnailFileName = fileName;
                    }

                    //entity.Pictures.Add(_mapper.Map<PictureEntity>(vm.CurrentProduct.Pictures.Last()));
                }
            }
            catch
            {
                return StatusCode(500, "Nie dodano kategorii");
            }

            var ent = _db.Category.Find(category.CategoryId) ?? new Category();

            ent.Caption = category.Name;
            ent.Name = category.Name.RemoveDiacritics().Replace(" ", "-").ToLower();
            ent.ParentId = category.ParentId;
            ent.ThumbnailFileName = category.ThumbnailFileName;

            //this line is to refactor I dont know why 
            //have to be removed from db and code
            ent.ChildId = category.ParentId;
            ent.Icon = category.Icon;

            _db.Category.Update(ent);
            _db.SaveChanges();

            return Ok();
        }

        [HttpGet]
        [Route("Manage/deleteCategory")]
        public ActionResult DeleteCategory(string parent, string child)
        {
            Category ent = null;

            var hasOnlyOneCategroy = string.IsNullOrEmpty(child) && !string.IsNullOrEmpty(parent);
            var hasCategoryWithParent = !string.IsNullOrEmpty(child) && !string.IsNullOrEmpty(parent);

            if (hasOnlyOneCategroy)
            {
                var cat = _db.Category.FirstOrDefault(x => x.Name == parent && x.Parent == null);
                ent = cat;

            }

            else if (hasCategoryWithParent)
            {
                var cat = _db.Category.FirstOrDefault(x => x.Parent.Name == parent && x.Name == child);
                ent = cat;
            }

            _db.Remove(ent);
            _db.SaveChanges();

            return Ok();
        }

        [Route("Index/add-productAttribute-get")]
        [HttpGet]
        public ActionResult AddProductAttribute(int? productId = null)
        {
            var vm = new AddProductAttributeVM();

            if (productId != null)
            {
                Product parentEnt = _db.Product.Find(productId);
                _db.Entry(parentEnt).Collection(d => d.ProductAttribute).Load();

                var product = _mapper.Map<ProductVM>(parentEnt);
                vm.CurrentProduct = product;
                vm.ProductOfAttributeId = productId;
            }

            vm.CategoryAttributeId = _db.Product.Find(productId)?.CategoryId;

            //this can be refactored
            var categoriesEntity = _db.Category.ToList();
            vm.AllCategories = _mapper.Map<ICollection<CategoryVM>>(_db.Category.ToList());
            vm.AllCategoriesTreeNode = _mapper.Map<ICollection<TreeNodeVm>>(categoriesEntity.Where(x => x.Parent == null).ToList());

            return Ok(vm);
        }

        [Route("confirmWithdrawByAdmin")]
        [HttpPost]
        public async Task<ActionResult<UserAccountVM>> ConfirmWithdrawByAdmin([FromBody] UserAccountVM account)
        {
            using (var dbContextTransaction = _db.Database.BeginTransaction())
            {
                UserAccount accountEnt = await _db.UserAccount.FindAsync(account.Id);
                accountEnt.StatusId = (int)Enums.AccountOperationStatus.WithdrawCompleted;
                _db.SaveChanges();

                dbContextTransaction.Commit();

                return Ok(_mapper.Map<UserAccountVM>(accountEnt));
            }
        }
    }
}