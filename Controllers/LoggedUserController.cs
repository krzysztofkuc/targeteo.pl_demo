using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using targeteo.pl.Common;
using targeteo.pl.Helpers;
using targeteo.pl.Model;
using targeteo.pl.Model.Entities;
using targeteo.pl.Model.ViewModel;
using targeteo.pl.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static targeteo.pl.Common.Enums;
using Enums = targeteo.pl.Common.Enums;

namespace targeteo.pl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //thios is for roles
    [Authorize(AuthenticationSchemes = Consts.DefaultAuthenticationScheme, Policy = Consts.ClaimTypeRoles)]
    public class LoggedUserController : BaseController
    {
        private readonly string _angularAssetsPath;
        private readonly IndexHelper _helper;
        private List<CategoryAttributeVM> _currentAllCategoryFilters = new List<CategoryAttributeVM>();

        public LoggedUserController(IMapper mapper, IWebHostEnvironment hostingEnvironment, IConfiguration config, ApplicationDbContext db)
            : base(config, db, mapper, hostingEnvironment)
        {
            _angularAssetsPath = config["AngularAssetsPath"];
            _helper = new IndexHelper(_db);
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("addorderopinion")]
        public ActionResult AddorderOpinion()
        {
            var modelJson = Request.Form["model"][0];
            OrderOpinionVM vm = JsonConvert.DeserializeObject<OrderOpinionVM>(modelJson);
            vm.OrderDetails = null;

            //var opinionEntity = _mapper.Map<OrderOpinionEntity>(vm);
            OrderOpinion opent = _mapper.Map<OrderOpinion>(vm);
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

                foreach (var file in files)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).Name.Trim('"');
                    string fullPath = Path.Combine(newPath, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                        var picVm = new PictureVM() { Path = fileName };
                        //vm.Pictures.Add(picVm);
                        var pic = _mapper.Map<Picture>(picVm);
                        _db.Picture.Add(pic);
                        _db.Entry(pic).State = EntityState.Added;
                        _db.SaveChanges();
                        //db.Entry(pic).State = EntityState.Detached;
                        //Entry<PictureEntity>(pic).State = EntityState.Detached;
                        //db.SaveChanges();
                        var find = _db.Picture.AsNoTracking().First(x => x.PictureId == pic.PictureId);
                        opent.Picture = find;
                        _db.Entry(opent).State = EntityState.Modified;
                        //db.Entry(pic).State = EntityState.Added;
                        //vm.Pictures.Add(_mapper.Map<PictureVM>(db.Pictures.AsNoTracking().First(x => x.PictureId == pic.PictureId)));

                        //vm.Pictures.Last().PictureId = pic.PictureId;
                    }

                    //entity.Pictures.Add(_mapper.Map<PictureEntity>(vm.CurrentProduct.Pictures.Last()));
                }

            }
            catch
            {
                throw;
            }

            //have to do that because here is eerorr cannnot add duplicate key to order
            //opinionEntity.OrderDetail = null;
            //opinionEntity.OrderDetail.Order = null;
            //opinionEntity.OrderDetail.Product = null;

            //have to attach because automapper creating new instance of entity
            //db.Entry(opinionEntity).State = EntityState.Added;
            _db.OrderOpinion.Add(opent);

            _db.SaveChanges();
            //var orderDetail = db.OrderDetails.Include(d => d.Order).
            //                                    Include(o => o.Product)
            //                                        .ThenInclude(p => p.Pictures).First(o => o.OrderDetailId == orderDetailId);

            //if (orderDetail == null)
            //{
            //    throw new Exception("Coś poszło nie tak");
            //}

            //var result = new OrderOpinionVM();
            //result.OrderDetailId = orderDetail.OrderDetailId;
            //result.OrderDetail = _mapper.Map<OrderDetailVM>(orderDetail);

            return Ok();
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("getorderopinions")]
        public ActionResult GetOrderOpinions(string prodId)
        {
            var opinions = _db.OrderOpinion.Include(x => x.Picture)
                                            .Include(x => x.OrderDetail).ThenInclude(d => d.Product)
                                            .Where(x => x.OrderDetail.ProductId == prodId);
            return Ok(opinions);
        }

        [Route("add-product-post")]
        [HttpPost, DisableRequestSizeLimit]
        public async Task<ActionResult> AddProductPost()
        {
            var viewMode = Request.Form["mode"][0];
            var modelJson = Request.Form["model"][0];
            AddProductVM vm = JsonConvert.DeserializeObject<AddProductVM>(modelJson);
            vm.CurrentProduct.Pictures.Clear();

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

                var product = _mapper.Map<Product>(vm.CurrentProduct);
                var entity = _db.Product.Include(x => x.Picture)
                                .Include(p => p.ProductAttribute)
                                .ThenInclude(a => a.CategoryAttribute)
                                .FirstOrDefault(x => x.ProductId == vm.CurrentProduct.ProductId);

                //add new product
                if (entity == null)
                {
                    entity = new Product();
                    entity.Picture = new List<Picture>();
                    entity.ProductAttribute = new List<ProductAttribute>();

                    //entity.Attributes = product.Attributes;
                    #region update attrs
                    foreach (var attrVm in product.ProductAttribute)
                    {
                        //attrEnt.Value = product.Attributes.First(a => a.CategoryAttribute.Name == attrEnt.CategoryAttribute.Name)?.Value?.ToString();
                        _db.Entry(entity).State = EntityState.Added;
                        _db.Entry(attrVm).State = EntityState.Added;
                        entity.ProductAttribute.Add(attrVm);
                    }
                    #endregion
                }
                //edit product
                else
                {
                    #region check if user is produt owner
                    var loginUser = GetLoginFromRequest();
                    var userLogged = _db.Users.Include(x => x.UserRoles).ThenInclude(r => r.Role).FirstOrDefault(x => x.Email == loginUser);

                    if ((userLogged == null || entity.AspNetUserId != userLogged.Id) && userLogged.UserRoles.FirstOrDefault(x => x.Role.Name == "Admin") == null)
                    {
                        return StatusCode(StatusCodes.Status401Unauthorized, "Błąd autoryzacji.");
                    }
                    #endregion


                    foreach (var attrVm in product.ProductAttribute)
                    {
                        var attrExists = entity.ProductAttribute.FirstOrDefault(a => a.CategoryAttribute.Name == attrVm.CategoryAttribute.Name);
                        //if entity contains attribute -> update it
                        if (attrExists != null)
                        {
                            attrExists.Value = attrVm.Value.ToString();
                            _db.Entry(attrExists).State = EntityState.Modified;
                        }
                        //else create new attribute in product
                        else
                        {
                            _db.Entry(entity).State = EntityState.Added;
                            _db.Entry(attrVm).State = EntityState.Added;
                            entity.ProductAttribute.Add(attrVm);
                        }
                    }
                }

                entity.CreationDate = DateTime.Now;

                #region to refactor get claim id user
                var login = GetLoginFromRequest();
                var user = _db.Users.FirstOrDefault(x => x.Email == login);
                #endregion

                entity.AspNetUserId = user.Id;
                entity.Description = product.Description;


                foreach (var file in files)
                {
                    fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).Name.Trim('"');
                    string fullPath = Path.Combine(newPath, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                        vm.CurrentProduct.Pictures.Add(new PictureVM() { Path = fileName, ProdId = vm.CurrentProduct.ProductId });
                    }

                    entity.Picture.Add(_mapper.Map<Picture>(vm.CurrentProduct.Pictures.Last()));
                }

                #region add City
                var existingCity = _db.City.FirstOrDefault(x => x.City1 == vm.CurrentProduct.City.City);
                if (existingCity == null)
                {
                    var newCity = new City()
                    {
                        City1 = vm.CurrentProduct.City.City,
                        Latitude = vm.CurrentProduct.City.Latitude,
                        Longitude = vm.CurrentProduct.City.Longitude
                    };
                    _db.City.Add(newCity);
                    _db.SaveChanges();
                    entity.CityId = newCity.Id;
                }
                else
                {
                    entity.CityId = existingCity.Id;
                }

                #endregion

                //this should be in automapper!!!!!!!!!!!!!!!!!
                entity.CategoryId = product.Category.CategoryId;
                entity.Category = null;
                //entity.Pictures = product.Pictures;

                entity.Description = product.Description;
                entity.Price = (double)Convert.ToDouble(product.ProductAttribute.First(a => a.CategoryAttribute.Name == "Cena").Value);
                entity.DiscountFromPrice = product.DiscountFromPrice;
                entity.Title = product.Title;
                entity.QuantityInStock = product.QuantityInStock;


                if (viewMode == "Add")
                {
                    #region create productId
                    var productId = entity.Title.ToLower().Replace(' ', '-');

                    int sameProductCount = 0;
                    while (true)
                    {
                        sameProductCount = _db.Product.Count(x => x.ProductId == productId);
                        if (sameProductCount > 0)
                        {
                            productId += "-" + sameProductCount;
                        }
                        else
                        {
                            break;
                        }
                    }


                    entity.ProductId = productId;
                    #endregion create productId

                    _db.Product.Add(entity);
                    _db.Entry(entity).State = EntityState.Added;
                }
                else
                {
                    #region deleteSelectedFiles
                    if (vm.DeletedPictures != null && vm.DeletedPictures.Count > 0)
                    {
                        foreach (var deletePath in vm.DeletedPictures)
                        {
                            var fn = Path.GetFileName(deletePath);
                            var deleteFilePath = newPath + "\\" + fn;
                            System.IO.File.Delete(fn);

                            var picEnt = entity.Picture.FirstOrDefault(x => x.Path == fn);
                            entity.Picture.Remove(picEnt);
                        }

                        //entity.Pictures = entity.Pictures.Where(pic => !vm.DeletedPictures.Contains(pic.Path)).ToList();
                    }
                    #endregion

                    _db.Product.Update(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                }
                vm.CurrentProduct.ProductId = entity.ProductId;
                try
                {
                    _db.SaveChanges();
                }
                catch (Exception)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Błąd zapisu.");
                }

                //Product already not has supply method
                ////to działa przy edycji nie działa przy nowym produkcie
                //#region add SupplyMethod
                //var existingSupplyMethods = db.ProductSupplyMethods.Where(x => x.UserId == vm.CurrentProduct.ProductId);
                //if (existingSupplyMethods != null)
                //{
                //    db.ProductSupplyMethods.RemoveRange(existingSupplyMethods);
                //}

                ////add prduct ID to supply methods
                //vm.CurrentProduct.SupplyMethods.ToList<ProductSupplyMethodVm>().ForEach(x => x.ProductId = vm.CurrentProduct.ProductId);
                //var newMethods = _mapper.Map<List<UserSupplyMethodEntity>>(vm.CurrentProduct.SupplyMethods);
                //db.ProductSupplyMethods.AddRange(newMethods);
                //db.SaveChanges();

                //#endregion

                ////add price attribute
                var addedEnt = _db.Product.Include(p => p.ProductAttribute).ThenInclude(attr => attr.CategoryAttribute).First(p => p.ProductId == entity.ProductId);

                //add attributes to product
                foreach (var productAttribute in vm.CurrentProduct.Attributes)
                {
                    var attrAlreadyInProduct = addedEnt.ProductAttribute.Any(attr => attr.CategoryAttribute.Name == productAttribute.CategoryAttribute.Name);
                    if (!attrAlreadyInProduct)
                    {
                        var addAttr = new AddProductAttributeVM()
                        {
                            CurrentProduct = vm.CurrentProduct,
                            Name = productAttribute.CategoryAttribute.Name,
                            Value = productAttribute.Value,
                            AttributeType = productAttribute.CategoryAttribute.AttributeType,
                            ViewFilterType = productAttribute.CategoryAttribute.ViewFilterType
                        };
                        await CreateProductAttribute(addAttr);
                    }
                }

                return new JsonResult(fileName);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        //Check is the announcement is the logged user announcment !!!!
        [HttpGet]
        [Route("manage/endAnnouncement")]
        public async Task<ActionResult> EndAnnouncement(string productId)
        {
            #region to refactor get claim id user
            var login = GetLoginFromRequest();
            var user = _db.Users.FirstOrDefault(x => x.Email == login);
            #endregion

            var ent = _db.Product.Find(productId);
            ent.DateTo = DateTime.Now.AddSeconds(-1);


            if (user.Id != ent.AspNetUser.Id)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, "Błąd autoryzacji.");
            }

            await _db.SaveChangesAsync();

            return Ok(ent);
        }

        //Check is the announcement is the logged user announcment !!!!
        [HttpGet]
        [Route("manage/activateAnnouncement")]
        public async Task<ActionResult> ActivateAnnouncement(string productId)
        {
            //validate user
            var login = GetLoginFromRequest();
            var user = _db.Users.FirstOrDefault(x => x.Email == login);

            var ent = _db.Product.Find(productId);
            ent.CreationDate = DateTime.Now;
            ent.DateTo = DateTime.Now.AddDays(30);

            if (user.Id != ent.AspNetUser.Id)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, "Błąd autoryzacji.");
            }

            await _db.SaveChangesAsync();

            return Ok(ent);
        }

        //Check is the announcement is the logged user announcment !!!!
        [HttpGet]
        [Route("manage/addAnnouncementActivityDays")]
        public async Task<ActionResult> AddAnnouncementActivity30Days(string productId)
        {
            var login = GetLoginFromRequest();
            var user = _db.Users.FirstOrDefault(x => x.Email == login);
            var ent = _db.Product.Find(productId);
            if (user.Id != ent.AspNetUser.Id)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, "Błąd autoryzacji.");
            }

            return Ok(await this.AddAnnouncementActivityDays(productId, 30));
        }

        //Check is the announcement is the logged user announcment !!!!
        [HttpGet]
        [Route("manage/getCountedUnreadMessages")]
        public ActionResult GetCountedUnreadMessages()
        {
            var login = GetLoginFromRequest();

            if (login == null)
            {
                return Ok(0);
            }

            var user = _db.Users.FirstOrDefault(x => x.Email == login);
            var commentsCount = _db.Comments.Where(x => x.UserIdTo == user.Id).Count(x => x.Viewed == false);

            return Ok(commentsCount);
        }

        [HttpGet]
        [Route("Manage/editProductAttribute")]
        public ActionResult EditProductAttributeGet(string prodId, int attrId)
        {
            var vm = new AddProductAttributeVM();

            var product = _mapper.Map<ProductVM>(_db.Product
                .Include(prod => prod.ProductAttribute).
                ThenInclude(attr => attr.CategoryAttribute).
                Include(prod => prod.Category).
                FirstOrDefault(x => x.ProductId == prodId));

            if (product != null)
            {
                vm.CurrentProduct = product;
                vm.ProductOfAttributeId = attrId;
                var allAttributes = _db.CategoryAttributes;
                vm.AllAttributes = _mapper.Map<ICollection<CategoryAttributeVM>>(allAttributes);
                vm.CategoryAttributeId = product.Category.CategoryId;

                var attr = product.Attributes.FirstOrDefault(attr => attr.ProductAttributeId == attrId);
                vm.AttributeType = attr.CategoryAttribute.AttributeType;
                vm.Name = attr.CategoryAttribute.Name;
                vm.Value = attr.Value;

                return new JsonResult(vm);
            }

            return StatusCode(StatusCodes.Status500InternalServerError, "Nie ma takiego produktu");
        }

        [HttpGet]
        [Route("Manage/deleteProductAttribute")]
        public ActionResult DeleteProductAttributeGet(int attrId)
        {
            var attr = _db.ProductAttribute.Find(attrId);
            _db.ProductAttribute.Remove(attr);

            _db.SaveChanges();

            return Ok(true);
        }

        [HttpPost]
        [Route("Manage/addAttribute")]
        public async Task<ActionResult> CreateProductAttribute(AddProductAttributeVM data)
        {
            try
            {
                var savedEnt = await CreateProductAttributeFunc(data);
                return Ok(_mapper.Map<ProductAttributeVM>(savedEnt.Entity));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Product posiada już taki atrybut. Edytuj obecny atrybut.");
            }
        }

        [HttpGet]
        [Route("addOrderOpinion")]
        public ActionResult AddorderOpinion(int orderDetailId)
        {
            var orderDetail = _db.OrderDetail.Include(d => d.OneUserOrder).
                                                Include(o => o.Product)
                                                    .ThenInclude(p => p.Picture).First(o => o.OrderDetailId == orderDetailId);

            if (orderDetail == null)
            {
                throw new Exception("Coś poszło nie tak");
            }

            var result = new OrderOpinionVM();
            result.OrderDetailId = orderDetail.OrderDetailId;
            result.OrderDetails = _mapper.Map<OrderDetailVM>(orderDetail);

            return Ok(result);
        }

        [HttpGet]
        [Route("Order/getOrders")]
        public ActionResult GetOrders(string orderSummaryId)
        {
            var login = GetLoginFromRequest();
            var user = _db.Users.FirstOrDefault(x => x.Email == login);




            if (string.IsNullOrEmpty(orderSummaryId))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "OrderId nie może być puste.");
            }

            OrderSummary orderSummary = _db.OrderSummary.Include(s => s.OneUserOrder)
                                                                .ThenInclude(order => order.UserSupplyMethod)
                                                            .Include(s => s.OneUserOrder.Where(ouo => ouo.AspNetUser == user))
                                                                .ThenInclude(order => order.OrderDetail)
                                                                .ThenInclude(det => det.Product)
                                                                .ThenInclude(prod => prod.Picture)

                                                            .Include(s => s.OneUserOrder.Where(ouo => ouo.AspNetUser == user))
                                                                .ThenInclude(order => order.OrderDetail)
                                                                .ThenInclude(det => det.Product)
                                                                .ThenInclude(prod => prod.AspNetUser)

                                                            //check if user is owner or seller of that product
                                                            .FirstOrDefault(order => order.Id == orderSummaryId && ( order.OneUserOrder.Any(o => o.AspNetUser == user) || order.OneUserOrder.Any(o => o.OrderDetail.All(d => d.Product.AspNetUser == user) )) );

            var orderSummaryVm = _mapper.Map<OrderSummaryVm>(orderSummary);

            //orderVm.Address = AnonymizeAddress(orderVm.Address);

            return new JsonResult(orderSummaryVm);
        }

        [Route("Order/getAllOrders")]
        [HttpGet]
        public IActionResult GetAllOrders()
        {
            var vm = _mapper.Map<ICollection<OneUserOrder>>(_db.OneUserOrder.Include(o => o.UserSupplyMethod)).OrderByDescending(o => o.OrderDate);

            return Ok(vm);
        }

        [Route("order/generateOrderId")]
        [HttpGet]
        public IActionResult GenerateOrderId()
        {
            string orderId = Helpers.IndexHelper.GenerateOrderId();

            return new JsonResult(orderId);
        }

        //very important ---------- Add transaction here !!!!!!!!!!!!!!!
        //on cancel order + quantity of items !!!!!!
        [Route("Order/completeOrder")]
        [HttpPost]
        public async Task<IActionResult> CompleteOrder([FromBody] OneUserOrderVM[] orders)
        {
            var login = GetLoginFromRequest();
            var user = _db.Users.FirstOrDefault(x => x.Email == login);

            OrderSummary orderSummary = new OrderSummary();

            //have to create it here because we need to add linkedResourceWith Id
            BodyBuilder bodyBuilder = new BodyBuilder();

            using (var dbContextTransaction = _db.Database.BeginTransaction())
            {
                #region Add orderSummaryFirst
                
                orderSummary.Id = Helpers.IndexHelper.GenerateOrderId();
                orderSummary.Date = DateTime.Now;
                orderSummary.PaymentUrl = CreatePayementUrl(orders, orderSummary.Id);
                _db.OrderSummary.Add(orderSummary);

                #endregion Add orderSummaryFirst

                foreach (var order in orders)
                {
                    OneUserOrderVM o = _mapper.Map<OneUserOrderVM>(order);
                    //o.PaymentUrl = order.PaymentUrl;
                    o.SelectedSupplyMethod = order.SelectedSupplyMethod;
                    o.UserSupplyMethodId = order.SelectedSupplyMethod.Id;
                    o.OrderSummaryId = orderSummary.Id;

                    //#region to refactor get claim id user
                    //var login = GetLoginFromRequest();
                    //var user = db.Users.FirstOrDefault(x => x.UserName == login);
                    //#endregion

                    //o.AspNetUserId = user.Id;

                    o.Status = Common.Enums.OrderStatus.waitingForPayment.ToString();
                    o.AspNetUserId = user.Id;

                    //here we have to find products by Id, not taking it from front end !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    var serialized = JsonConvert.SerializeObject(o.OrderDetails);
                    var cloned = JsonConvert.DeserializeObject<List<OrderDetailVM>>(serialized);

                    var tmpCollection = o.OrderDetails.ToList();

                    #region remove mapped product entities because of EF tracking exception
                    for (int i = 0; i < tmpCollection.Count(); i++)
                    {
                        var obj = tmpCollection[i];
                        obj.ProductId = obj.Product.ProductId;
                        obj.UnitPrice = obj.Product.Price;
                        obj.Product = null;
                    }
                    o.OrderDetails = tmpCollection;
                    #endregion

                    var orderEntity = _mapper.Map<OneUserOrder>(o);

                    #region check there is enough products in store

                    var error = string.Empty;
                    foreach (var product in order.OrderDetails)
                    {
                        var prodEnt = _db.Product.Find(product.ProductId);
                        //var orderDetail = orderEntity.OrderDetails.First(d => d.Product.ProductId == product.ProductId);
                        //db.Entry(orderDetail).Reference(c => c.Product).Load();

                        if (prodEnt.QuantityInStock < product.Quantity)
                        {
                            var errorInfo = "Niestety pozostało tylko " + prodEnt.QuantityInStock + " przedmiotów: " + prodEnt.Title + "\n";
                            error += errorInfo;

                            return StatusCode(StatusCodes.Status500InternalServerError, error);
                        }

                        prodEnt.QuantityInStock -= product.Quantity;
                    }

                    #endregion check product are in store

                    _db.OneUserOrder.Add(orderEntity);
                    await _db.SaveChangesAsync();


                    #region send email to client - shoulkd be extracted to method
                    var companyInfo = _db.CompanyInformation.FirstOrDefault();
                    var msg = "Dziękujemy za zamówienie. </br> Nr zamówienia to " + orderSummary.Id + "</br>";

                    var path = Path.Combine(Directory.GetCurrentDirectory(), @"Assets\\EmailTemplates");


                    using (StreamReader SourceReader = System.IO.File.OpenText("Assets\\EmailTemplates\\OrderSummaryEmailTemplate.html"))
                    {
                        msg = SourceReader.ReadToEnd();
                    }

                    #region add Logo picture
                    var pathLogo = Path.Combine(Directory.GetCurrentDirectory(), @"Assets\\Images\\targeteoLogo.png");
                    var image = bodyBuilder.LinkedResources.Add(pathLogo);
                    image.ContentId = MimeUtils.GenerateMessageId();
                    string htmlBody = @"<img src='cid:" + image.ContentId + @"' style='width:100%; max-width: 300px; max-height: 300px;'/>";

                    msg = msg.Replace("<<logoPath>>", htmlBody);
                    #endregion

                    double pricePerUnit = order.SelectedSupplyMethod.PricePerUnit;
                    double sum = pricePerUnit;

                    string orderLoop = @"<table style='border: 1px solid black'>
                                      <thead>
                                        <tr>
                                          <th> Przedmiot </th>
                                          <th> Cena </ h>
                                          <th> Ilość </ h>
                                        </tr>
                                      </thead>
                                      <tbody>";
                    foreach (var item in cloned)
                    {
                        orderLoop += "<tr>";
                        orderLoop += "<td>" + item.Product.Title + "</td>";
                        orderLoop += "<td>" + item.Product.Price+ " zł </td>";
                        orderLoop += "<td>" + item.Quantity + "</td>";
                        orderLoop += "</tr>";

                        sum += item.Product.Price * item.Quantity;
                    }

                    orderLoop += @"</tbody>
                                  <tfoot>
                                    <tr>
                                      <td> Cena dostawy: </td>
                                        <td></td>
                                      <td>" + pricePerUnit +  " zł</td>" +
                                      @"</tr>
                                       <tr>
                                      <td> Suma zamówienia </td>
                                        <td></td>
                                      <td>" + sum + " zł</td>" +
                                      @"</tr>
                                    </tfoot>
                                  </table> ";

                    msg = msg.Replace("<<orderLoop>>", orderLoop);

                    msg = msg.Replace("<<firstName>>", order.FirstName);
                    msg = msg.Replace("<<lastName>>", order.LastName);
                    msg = msg.Replace("<<address>>", order.Address);
                    msg = msg.Replace("<<city>>", order.City);
                    msg = msg.Replace("<<postalCode>>", order.PostalCode);

                    msg += "<h4>Adres do wysyłki: </h4>";

                    
                    if (!string.IsNullOrWhiteSpace(order.PaczkomatDestinationAddress))
                    {

                        msg += "Paczkomat " + order.PaczkomatDestinationAddress + "</br>";
                    }
                    else
                    {
                        msg += order.FirstName + " " + order.LastName + "</br>";
                        msg += order.Address + "</br>";
                        msg += order.City + "</br>";
                        msg += order.PostalCode + "</br></br>";
                    }

                    msg += $"Możesz sprawdzić aktualny status zamówienia pod adresem: </br> <a href='https://targeteo.pl/ordersummary?orderId=" + orderSummary.Id + "'>" +
                        "https://targeteo.pl/ordersummary?orderId=" + orderSummary.Id + "</a> </br>";
                    
                    msg += "Po wysłaniu zamówienia otrzymają Państwo email z numerem przesyłki. </br></br>.";

                    msg += "<div style='background-color: lightgrey;'>";
                    msg += "Dziękujemy za zakupy, </br> " + companyInfo.Name;
                    msg += "</div>";

                    //close document
                    msg += @"</div>
                                </div>
                            </body>
                            </html>";

                    await EmailService.SendAsync(companyInfo.Name + " - nowe zamówienie nr. " + orderSummary.Id, msg, o.Email, bodyBuilder,  companyInfo);
                    await EmailService.SendAsync(companyInfo.Name + " - nowe zamówienie nr. " + orderSummary.Id,  msg, companyInfo.Email, bodyBuilder, companyInfo);
                    #endregion send email to client - shoulkd be extracted to method

                    
                }
                dbContextTransaction.Commit();
            }

            #region payment

            //payment
            //var values = new Dictionary<string, string>
            //{
            //    { "SEKRET", "L0trMkVBNUNqelpqM1Qycm9Oc2lZNnJpUUVWNlBycXFKZWIzaC8yN1NaVT0" },
            //    { "KWOTA", "13" },
            //    { "NAZWA_USLUGI", "zakup" },
            //    { "ADRES_WWW", "https://targeteo.pl/order/paymentended" },
            //    { "ID_ZAMOWIENIA", "zzzzzzz" },
            //    { "EMAIL", "karolina.krykant@gmail.com" },
            //    { "DANE_OSOBOWE", "Jan Kowalski ul. Marszakowska 54" }
            //};

            //var content = new FormUrlEncodedContent(values);

            //var response = await client.PostAsync("https://platnosc.hotpay.pl/", content);

            //var responseString = await response.Content.ReadAsStringAsync();

            #endregion

            var dto = new JsonHelperDto() { Value = orderSummary.PaymentUrl };

            return Ok(dto);
        }

        private string CreatePayementUrl(OneUserOrderVM[] orders, string orderSummaryId)
        {
            double sum = 0;
            var itemsNames = string.Empty;
            var firOr = orders[0];
            var address = $"{firOr.FirstName} {firOr.LastName}, {firOr.Address} {firOr.PostalCode} {firOr.State} {firOr.City} {firOr.PaczkomatDestinationAddress}";
            var ordersId = string.Empty;

            foreach (var order in orders)
            {
                foreach (var item in order.OrderDetails)
                {
                    sum += item.Product.Price * item.Quantity;
                    itemsNames += item.Product.ProductId + " x" + item.Quantity + ";";
                }

                ordersId += $"{orderSummaryId};";
            }

            var parameters = new Dictionary<string, string>
                       {
                           { "SEKRET", _config["PaymentSecret"] },
                           { "KWOTA", sum.ToString() },
                           { "NAZWA_USLUGI", orderSummaryId },
                           { "PRZEKIEROWANIE_SUKCESS", _config["PaymentSuccessRedirectAddress"] },
                           { "EMAIL", orders[0].Email },
                           { "DANE_OSOBOWE", address },
                           { "ID_ZAMOWIENIA", orderSummaryId },
                           { "ADRES_WWW", "https://targeteo.pl/ordersummary?orderId=" + orderSummaryId }
                        };

            var url = QueryHelpers.AddQueryString("https://platnosc.hotpay.pl", parameters);

           return url;
        }

        [AllowAnonymous]
        [Route("order/paymentended")]
        [HttpPost]
        public IActionResult PaymentEnded([FromForm] PaymentResponseVM formData)
        {
            if (string.IsNullOrEmpty(formData.STATUS) ||
                string.IsNullOrEmpty(formData.SEKRET) ||
                string.IsNullOrEmpty(formData.ID_ZAMOWIENIA) ||
                string.IsNullOrEmpty(formData.ID_PLATNOSCI) ||
                string.IsNullOrEmpty(formData.HASH) ||
                string.IsNullOrEmpty(formData.KWOTA))
            {
                return Ok("fail");
            }

            var hashApi = _config["HashApi"];
            var sekret = _config["PaymentSecret"];
            string response = $"{hashApi};{formData.KWOTA};{formData.ID_PLATNOSCI};{formData.ID_ZAMOWIENIA};{formData.STATUS};{formData.SEKRET}";

            //that hash doesnt work, try with postman, every hash works and this is a bug
            var responseHashed = ComputeSHA256Hash(response);

            if (responseHashed != formData.HASH)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable, "Nie zgadza się hash wiadomości.");
            }

            if (formData.STATUS == "SUCCESS")
            {
                //add payyment to user account


                //add here transaction with change status when upgrade to .net 6 !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                var accountAmount = new UserAccount()
                {
                    OperationAmount = Convert.ToDecimal(formData.KWOTA),
                    OperationDate = DateTime.Now,
                    OrderSummaryId = formData.ID_ZAMOWIENIA,
                    StatusId = (int)Enums.AccountOperationStatus.Add
                };

                var query = _db.UserAccount.Add(accountAmount);
                _db.SaveChanges();
                ChangeStatus(formData.ID_ZAMOWIENIA, nameof(Enums.OrderStatus.paymentCompleted), string.Empty);
            }

            return Ok("sukces");
        }

        [Route("Order/changeStatus")]
        [HttpGet]
        public IActionResult ChangeStatus(string orderId, string changeToStatus, string trackingNo)
        {
            ManageEntity.ChangeOrderStatus(_db, orderId, changeToStatus, trackingNo);
            //var order = db.Orders.Find(orderId);
            //order.Status = changeToStatus;

            ////get caption from string as enum
            //var statusCaption = string.Empty;
            //switch (changeToStatus)
            //{
            //    case nameof(Enums.OrderStatus.sent):
            //        statusCaption = "wysłano";
            //        break;
            //    case nameof(Enums.OrderStatus.preparing):
            //        statusCaption = "w przygotowaniu";
            //        break;
            //    case nameof(Enums.OrderStatus.canceled):
            //        statusCaption = "anulowano";
            //        break;
            //    case nameof(Enums.OrderStatus.returnShipment):
            //        statusCaption = "zwrócono";
            //        break;
            //    case nameof(Enums.OrderStatus.waitingForPayment):
            //        statusCaption = "oczekiwanie na płatność";
            //        break;
            //    case nameof(Enums.OrderStatus.paymentCompleted):
            //        statusCaption = "opłacono";
            //        break;
            //}

            //if (changeToStatus == Enums.OrderStatus.sent.ToString())
            //{
            //    order.TrackingNo = trackingNo;
            //}

            //db.Orders.Update(order);
            //await db.SaveChangesAsync();


            //#region send change satus email
            //if (changeToStatus == Enums.OrderStatus.sent.ToString())
            //{

            //    var companyInfo = db.CompanyInformationEntities.FirstOrDefault();

            //    await EmailService.SendAsync(companyInfo.Name + " - zamówienie: " + orderId + " - zmieniono status na: '" + statusCaption + "'", "Nr. listu przewozowego: " + trackingNo, order.Email);
            //    await EmailService.SendAsync(companyInfo.Name + " - zamówienie: " + orderId + " - zmieniono status na: '" + statusCaption + "'", "Nr. listu przewozowego: " + trackingNo, companyInfo.Email);
            //}
            //else
            //{
            //    var companyInfo = db.CompanyInformationEntities.FirstOrDefault();
            //    await EmailService.SendAsync(companyInfo.Name + " - zamówienie: " + orderId + " - zmieniono status na: '" + statusCaption + "'", string.Empty, order.Email);
            //    await EmailService.SendAsync(companyInfo.Name + " - zamówienie: " + orderId + " - zmieniono status na: '" + statusCaption + "'", string.Empty, companyInfo.Email);
            //}
            //#endregion send change satus email

            return Ok("order");
        }

        private string ComputeSHA256Hash(string text)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                var hash = string.Empty;
                var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(text));
                foreach (byte theByte in bytes)
                {
                    hash += theByte.ToString("x2");
                }
                return hash;
            }
        }

        [Route("Order/getSupplyMeythods")]
        [HttpGet]
        public IActionResult GetSupplyMeythods()
        {
            return Ok(_mapper.Map<ICollection<SupplyMethodVm>>(_db.SupplyMethods));
        }

        [HttpGet]
        [Route("Manage/addAttribute")]
        public ActionResult AddProductAttributeGet(int? productId = null)
        {
            var vm = new AddProductAttributeVM();

            if (productId != null)
            {
                var product = _mapper.Map<ProductVM>(_db.Product.Find(productId));
                vm.CurrentProduct = product;
                vm.ProductOfAttributeId = productId;
            }
            else
            {
                var allAttributes = _db.CategoryAttributes;
                vm.AllAttributes = _mapper.Map<ICollection<CategoryAttributeVM>>(allAttributes);
            }

            vm.CategoryAttributeId = _db.Product.Find(productId)?.CategoryId;

            var categoriesEnt = _db.Category.ToList();
            vm.AllCategories = _mapper.Map<ICollection<CategoryVM>>(categoriesEnt);
            vm.AllCategoriesTreeNode = _mapper.Map<ICollection<TreeNodeVm>>(categoriesEnt.Where(x => x.Parent == null).ToList());

            return new JsonResult(vm);
        }

        [HttpPost]
        [Route("Manage/ask-question-for-product")]
        public async Task<ActionResult> AskQuestionForProduct(MsgToUserVm msg)
        {
            var userFrom = _db.Users.Find(msg.UserIdFrom);
            var userTo = _db.Users.Find(msg.UserIdTo);

            var alreadyExistConversationId = _db.Comments.Include(c => c.Conversation).FirstOrDefault(x => x.UserIdFrom == userFrom.Id &&
                                                                        x.UserIdTo == msg.UserIdTo &&
                                                                        x.ProductId == msg.ProductId)?.Conversation?.Id;
            var conv = new Conversations();
            var ent = _mapper.Map<Comments>(msg);
            ent.ConversationId = msg.Id;

            //this have to be refactore d is too complecated
            if (msg.ConversationId == null)
            {
                //to może znaczyć że nie odpowiada z chatu tylko pod produktem
                //poszukaj czy takie conversation już nie istnieje ?

                conv.ProductId = msg.ProductId;
                _db.Conversations.Add(conv);
                _db.SaveChanges();

                if (alreadyExistConversationId != null)
                {
                    ent.ConversationId = alreadyExistConversationId;
                }
                else
                {
                    ent.ConversationId = conv.Id;
                }
            }

            ent.Date = DateTime.Now;
            ent.UserIdFrom = userFrom.Id;
            ent.UserIdTo = msg.UserIdTo;
            ent.Viewed = false;

            _db.Comments.Add(ent);
            _db.SaveChanges();

            #region send email to client - shoulkd be extracted to method
            var product = _db.Product.Find(msg.ProductId);
            var companyInfo = _db.CompanyInformation.FirstOrDefault();
            var emailContent = "Nowa wiadomość do ogłoszenia <a href=https://" + companyInfo.Name + "/produkt/" + product.ProductId + ">" + product.Title + "</a> od " + userFrom.Email + ":</br> ";

            emailContent += msg.Content + "</br>";

            string href = "<a href=https://" + companyInfo.Name + "/user-profile/user-product-messages?userIdFrom=" + msg.UserIdFrom + "&userIdTo=" + msg.UserIdTo + "&prodId=" + msg.ProductId + "&conversationId=" + msg.ConversationId + ">";
            emailContent += href;
            emailContent += "Link do wiadomości w serwisie " + companyInfo.Name + "</a>";

            await EmailService.SendAsync("Nowa wiadomość do " + product.Title, emailContent, userTo.Email, null, companyInfo);
            #endregion send email to client - shoulkd be extracted to method


            return Ok(ent);
        }

        [HttpGet]
        [Route("Manage/addCategory")]
        public ActionResult AddCategory(string parent, string child)
        {
            AddCategoryVM acVM = new AddCategoryVM();

            var hasOnlyOneCategroy = string.IsNullOrEmpty(child) && !string.IsNullOrEmpty(parent);
            var hasCategoryWithParent = !string.IsNullOrEmpty(child) && !string.IsNullOrEmpty(parent);

            if (hasOnlyOneCategroy)
            {
                var cat = _db.Category.FirstOrDefault(x => x.Name == parent && x.Parent == null);
                acVM.Category = _mapper.Map<CategoryVM>(cat);
            }
            else if (hasCategoryWithParent)
            {
                var cat = _db.Category.FirstOrDefault(x => x.Parent.Name == parent && x.Name == child);
                acVM.Category = _mapper.Map<CategoryVM>(cat);
            }

            //var entities = db.Categories.ToList();

            //var cats = ;
            //acVM.AllCategories = _mapper.Map<List<CategoryVM>>(cats);
            acVM.AllCategories = GetCategoriesHierarchy();
            acVM.AllCategoriesTree = _mapper.Map<ICollection<TreeNodeVm>>(_db.Category.Where(x => x.Parent == null).ToList());

            return Ok(acVM);
        }

        [HttpPost]
        [Route("Manage/addCategory")]
        public async Task<ActionResult> AddCategory([FromBody] AddCategoryVM cat)
        {
            //create id without polish diacritics
            cat.Category.Caption = cat.Category.Name;
            cat.Category.Name = cat.Category.Name.RemoveDiacritics().Replace(" ", "-").ToLower();

            cat.Category.ChildId = cat.Category.ParentId;
            if (ModelState.IsValid)
            {
                var category = _mapper.Map<Category>(cat.Category);

                var parent = _db.Category.Find(category.ParentId);

                if (parent != null)
                {
                    _db.Entry(parent).Collection(d => d.InverseParent).Load();//Children are loaded, we can loop them now
                    var exist = parent.InverseParent.Any(c => c.Name == cat.Category.Name);

                    if (exist)
                    {
                        ModelState.AddModelError("error_msg", "Current category exist in the parent category");
                        var cats = _db.Category.ToList();
                        cat.AllCategories = _mapper.Map<List<CategoryVM>>(cats);
                        return new JsonResult(cat);
                    }
                }
                _db.Category.Add(category);
                _db.SaveChanges();

                #region add dummy attribute "Cena"
                AddProductAttributeVM attr = new AddProductAttributeVM();
                attr.Name = "Cena";
                attr.AttributeType = nameof(AllAttributeTypes.number);
                attr.CurrentProduct = new ProductVM();
                attr.CurrentProduct.Category = _mapper.Map<CategoryVM>(category);
                attr.CategoryId = category.CategoryId;
                attr.ViewFilterType = nameof(AllAttributeTypes.text);
                await CreateProductAttributeFunc(attr);
                #endregion
            }
            AddCategoryVM acVM = new AddCategoryVM();

            return new JsonResult(acVM);
        }

        #region private methods

        public async Task<EntityEntry<ProductAttribute>> CreateProductAttributeFunc(AddProductAttributeVM data)
        {
            var login = GetLoginFromRequest();
            var user = await _db.Users.FirstOrDefaultAsync();

            var newCatAttr = new CategoryAttributes();
            var category = _db.Category.Find(data.CurrentProduct.Category.CategoryId);

            _db.Entry(category).Collection(d => d.CategoryAttributes).Load();

            var newAttr = new ProductAttributeVM();
            //newAttr.Product = data.CurrentProduct;
            newAttr.Value = data.Value;

            #region add dummy product if not exist only to work attribute
            if (data.CurrentProduct.ProductId == null)
            {
                var dummyProduct = new Product();
                dummyProduct.ProductId = Guid.NewGuid().ToString();
                dummyProduct.Title = "dummy";
                dummyProduct.Description = "dummy description";
                dummyProduct.Category = _db.Category.Find(category.CategoryId);
                _db.Product.Add(dummyProduct);
                newAttr.ProductId = dummyProduct.ProductId;
                data.CurrentProduct.ProductId = dummyProduct.ProductId;
                dummyProduct.Hidden = true;
                dummyProduct.CreationDate= DateTime.Now;
                dummyProduct.DateTo = DateTime.Now.AddYears(1000);
                dummyProduct.AspNetUser = user;
                dummyProduct.ThumbPath = "";
                dummyProduct.CityId = 1;
                await _db.SaveChangesAsync();
            }
            #endregion

            var existingCategoryAttr = category.CategoryAttributes.FirstOrDefault(x => x.Name == data.Name && x.AttributeType == data.AttributeType);

            if (existingCategoryAttr != null)
            {
                var product = _db.Product.AsNoTracking().Include(prod => prod.ProductAttribute).ThenInclude(attr => attr.CategoryAttribute).FirstOrDefault(x => x.ProductId == data.CurrentProduct.ProductId);

                if (product != null)
                {
                    bool productAlreadyContainsAttr = product.ProductAttribute.Any(x => x.CategoryAttribute.Name == data.Value);

                    if (!productAlreadyContainsAttr)
                    {
                        newAttr.CategoryAttributeId = existingCategoryAttr.Id;
                    }
                }
                else
                {
                    throw new Exception("Product posiada już taki atrybut. Edytuj obecny atrybut.");
                    //return StatusCode(StatusCodes.Status500InternalServerError, "Product posiada już taki atrybut. Edytuj obecny atrybut.");
                }
            }
            else
            {
                newCatAttr.Name = data.Name;
                newCatAttr.AttributeType = data.AttributeType;
                newCatAttr.ViewFilterType = data.ViewFilterType;
                newCatAttr.CategoryId = data.CurrentProduct.Category.CategoryId;

                var catAdded = _db.CategoryAttributes.Add(newCatAttr);
                await _db.SaveChangesAsync();

                newAttr.CategoryAttributeId = catAdded.Entity.Id;
            }

            var savedEnt = _db.ProductAttribute.Add(_mapper.Map<ProductAttribute>(newAttr));
            await _db.SaveChangesAsync();

            return savedEnt;
        }

        [Route("Manage/{parentCategory}/{childCategory}/add-product-get")]
        [Route("Manage/{parentCategory}/add-product-get")]
        [Route("Manage/add-product-get")]
        [HttpGet]
        public ActionResult AddProductGet(string parentCategory, string childCategory, string id)
        {
            AddProductVM vmAddProduct = new AddProductVM();
            if (id != "null")
            {
                vmAddProduct = GetAddProductModel(id);
            }
            else
            {
                vmAddProduct.AllSupplyMethods = _mapper.Map<ICollection<SupplyMethodVm>>(_db.SupplyMethods.Where(m => m.SupplyMethodType.Id == (int)Enums.SupplyMethodType.defined).ToList());
            }

            //var dbCats = db.Categories.ToList();
            //vmAddProduct.AllCategories = GetCategoriesHierarchy();

            vmAddProduct.AllCategoriesTreeNodeFlaten = _mapper.Map<ICollection<TreeNodeVm>>(_db.Category);
            vmAddProduct.AllCategoriesTreeNode = _mapper.Map<ICollection<TreeNodeVm>>(_db.Category.Where(x => x.Parent == null).ToList());

            return Ok(vmAddProduct);
        }


        //this one is when we have few auth providers
        //[Authorize(AuthenticationSchemes = Consts.DefaultAuthenticationScheme)]

        //thios is for roles
        //[Authorize(AuthenticationSchemes = Consts.DefaultAuthenticationScheme, Policy = Consts.CustomRolePolicy)]
        [Route("Manage/delete-product")]
        [HttpGet]
        public IActionResult DeleteProduct(string id)
        {
            var entity = _db.Product.Include(p => p.Picture).FirstOrDefault(x => x.ProductId == id);

            if (entity != null)
            {
                entity.Removed = true;
                _db.Product.Update(entity);
                _db.SaveChanges();
            }

            return Ok(true);
        }

        private async Task<Product> AddAnnouncementActivityDays(string productId, int days)
        {
            var ent = _db.Product.Find(productId);
            ent.DateTo = ent.DateTo?.AddDays(days);

            await _db.SaveChangesAsync();

            return ent;
        }

        private string AnonymizeAddress(string address)
        {
            return new string(address.Take(5).ToArray()) + "...";
        }

        //this is redundant
        private AddProductVM GetAddProductModel(string id)
        {
            var entity = _db.Product.Include(p => p.AspNetUser).
                                    Include(x => x.Picture).
                                    Include(x => x.Category).
                                    Include(x => x.City).
                                    Include(x => x.ProductAttribute).
                                    ThenInclude(a => a.CategoryAttribute).
                                    FirstOrDefault(p => p.ProductId == id && !p.Removed && !p.Hidden);

            var model = new AddProductVM();
            model.CurrentProduct = _mapper.Map<ProductVM>(entity);
            //model.AllCategories = GetCategoriesHierarchy();
            model.NumberOfUserEvaluation = _db.OrderOpinion.Where(o => o.OrderDetail.Product.AspNetUserId == model.CurrentProduct.User.Id).Count();
            model.AvgUserEvaluation = 0;
            if (model.NumberOfUserEvaluation > 0)
            {
                model.AvgUserEvaluation = (int)_db.OrderOpinion.Where(o => o.OrderDetail.Product.AspNetUserId == model.CurrentProduct.User.Id).Average(x => x.Evaluation);
            }
            
            model.NumberOfUserEvaluation = (int)_db.OrderOpinion.Where(o => o.OrderDetail.Product.AspNetUserId == model.CurrentProduct.User.Id).Count();
            model.AllSupplyMethods = _mapper.Map<ICollection<SupplyMethodVm>>(_db.SupplyMethods.Where(m => m.SupplyMethodType.Id == (int)Enums.SupplyMethodType.defined).ToList());

            var category = _db.Category.Find(entity.Category.CategoryId);
            var catVm = _mapper.Map<CategoryVM>(category);

            model.CurrentProduct.Category = catVm;

            return model;
        }

        //this is redundant
        private List<CategoryVM> GetCategoriesHierarchy()
        {
            var cats = _db.Category.ToList();

            for (int i = 0; i < cats.Count; i++)
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
        #endregion
    }
}
