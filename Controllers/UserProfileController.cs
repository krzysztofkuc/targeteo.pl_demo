using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using targeteo.pl.Common;
using targeteo.pl.Helpers;
using targeteo.pl.Model.Entities;
using targeteo.pl.Model.ViewModel;
using targeteo.pl.Model;
using JWT.Algorithms;
using JWT.Serializers;
using JWT;
using targeteo.pl.Services;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace targeteo.pl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : BaseController
    {
        private readonly string _angularAssetsPath;

        public UserProfileController(IMapper mapper, IWebHostEnvironment hostingEnvironment, IConfiguration config, ApplicationDbContext db) 
            : base(config, db, mapper, hostingEnvironment)
        {
            _angularAssetsPath = config["AngularAssetsPath"];
        }


        [Route("get")]
        [HttpGet]
        public IActionResult Get()
        {

            var login = GetLoginFromRequest();
            var user = _db.Users.Include(u => u.UserSupplyMethod.Where(sm => sm.IsActive == true))
                .ThenInclude(s => s.SupplyMethod).FirstOrDefault(x => x.Email == login);

            UserProfileVm vm = new UserProfileVm()
            { 
                User = _mapper.Map<UserVM>(user),
                SupplyMethods = _mapper.Map<ICollection<SupplyMethodVm>>(_db.SupplyMethods)
            };
            //var products = db.Products.Include(p => p.Pictures)
            //                        .Include(p => p.Category)
            //                        .Where(x => x.AspNetUserId == user.Id && x.QuantityInStock > 0 && !x.Removed && !x.Hidden);

            return Ok(vm);
        }

        [Route("save")]
        [HttpPost]
        public IActionResult SaveProfile([FromBody] UserProfileVm profile)
        {

            var login = GetLoginFromRequest();
            var user = _db.Users.AsNoTracking().Include(u => u.UserSupplyMethod).AsNoTracking().FirstOrDefault(x => x.Email == login);

            //var methodsToDisable = user.UserSupplyMethods.Where(sm =>  profile.User.UserSupplyMethods.Any(x => x.Id == sm.Id));
            //var methodsToDisable = user.UserSupplyMethods.Where(sm => profile.User.UserSupplyMethods.Any(x => x.Id == sm.Id));

            //deactoivate all
            ////disactivate all
            ///
            foreach (var item in user.UserSupplyMethod)
            {
                item.IsActive = false;
                _db.Entry(item).State = EntityState.Modified;
            }

            foreach (var item in profile.User.UserSupplyMethods)
            {

                //if found supply method update it to Active
                var found = user.UserSupplyMethod.FirstOrDefault(sm => sm.Id == item.Id);
                if (found != null)
                {
                    found.IsActive = true;
                    _db.Entry(found).State = EntityState.Modified;
                }
                else
                {
                    var newEntity = _mapper.Map<UserSupplyMethod>(item);
                    newEntity.IsActive = true;
                    newEntity.UserId = user.Id;
                    user.UserSupplyMethod.Add(newEntity);
                    _db.Entry(newEntity).State = EntityState.Added;

                }
            }

            //db.UserSupplyMethods.RemoveRange(user.UserSupplyMethods);
            //db.SaveChanges();

            //var isOwner =  profile.User.UserSupplyMethods.All(x => { x.Id = 0; x.UserId = user.Id; return true; });
            //db.UserSupplyMethods.AddRange(_mapper.Map<List<UserSupplyMethodEntity>>(profile.User.UserSupplyMethods));

            _db.SaveChanges();

            //if (isOwner)
            //{
            //    db.SaveChanges();
            //}


            return Ok();
        }

        //Security problem 
        //allowed only for logged user !!!!
        //check if userId is logged user on backend!!!!!!!

        //tutaj ciagle nie oddziela wiadomości
        [Route("Messages/Get")]
        [HttpGet]
        public IActionResult GetMessages(string login)
        {
            var loginClaim = this.GetLoginFromRequest();
            var userLogged = _db.Users.FirstOrDefault(x => x.Email == loginClaim);
            var user = _db.Users.FirstOrDefault(u => u.Email == loginClaim);
            var comments = _db.Comments.Include(x => x.UserIdFromNavigation).Include(x => x.UserIdToNavigation).Include(c => c.Product)
                                        .ThenInclude(prod => prod.Picture)
                                        .Where(comment => comment.UserIdFrom == userLogged.Id || comment.UserIdTo == userLogged.Id).ToList()
                                        .GroupBy(x => x.ConversationId).ToList();
                                        //.ToList().GroupBy(x => x.ConversationId).ToList();
                                        //.Select(x => x.ToList()).ToList();

            return Ok(comments);
        }

        [Route("Messages/GetProductMessages")]
        [HttpGet]
        public async Task<IActionResult> GetProductMessages(string userIdFrom, string userIdTo, string prodId)
        {
            #region to refactor get claim id user
            //tutaj trzeba sprawdzić jakoś kto jest zalogowany na backendzie -> rozszyfrować token i pobrać z bazy po id ?
            //get token form request
            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            var secret = _config["Jwt:Key"];
            var tokenClaims = JwtHelper.DecodeToken(authHeader.Parameter, secret);
            var claimLogin = tokenClaims.FirstOrDefault(x => x.Type == Consts.Login)?.ToString().Split(": ")[1];
            var user = _db.Users.FirstOrDefault(x => x.Email == claimLogin);
            #endregion

            var userFrom = _db.Users.Find(userIdFrom);
            var userTo = _db.Users.Find(userIdTo);
            var commentsEnt = await _db.Comments.Include(c => c.UserIdFromNavigation).Include(c => c.UserIdToNavigation).Include(c => c.Product)
                                        .ThenInclude(prod => prod.Picture)
                                        .Where(comment => (comment.UserIdFrom == userIdFrom && comment.UserIdTo == userIdTo && comment.ProductId == prodId) ||
                                               (comment.UserIdFrom == userIdTo && comment.UserIdTo == userIdFrom && comment.ProductId == prodId)).ToListAsync();

            var commentsVm = _mapper.Map<ICollection<CommentVm>>(commentsEnt);
            var chatVm = new ChatVm() { Comments = commentsVm, UserFrom = userFrom.Email, UserTo = userTo.Email };

            //czy tutaj kiedy drugi użytkownik ozbaczy wiadomośi to nie updatuje tak, że są przeczytane i dla tego drugiego użytkownika ?
            //set messages as viewed
            var login = GetLoginFromRequest();
            var userLogged = _db.Users.FirstOrDefault(x => x.Email == login);
            //commentsEnt.Where(comment => (comment.UserIdTo == userLogged.Id)).ToList().ForEach(x => x.Viewed = true);


            var isUnviewedMessageIsNotMine = commentsEnt.Where(c => c.Viewed == false).Any(x => x.UserIdTo == userLogged.Id);
            if (isUnviewedMessageIsNotMine)
            {
                foreach (var item in commentsEnt)
                {
                    item.Viewed = true;
                };
            }
            
            await _db.SaveChangesAsync();

            return Ok(chatVm);
        }


        [Route("UserAnnouncements")]
        [HttpGet]
        public IActionResult GetUserAnnouncements(string userId)
        {

            //var login = GetLoginFromRequest();
            //var user = db.Users.FirstOrDefault(x => x.UserName == login);
            var userFrom = _db.Users.Find(userId);

            var prods = _db.Product.Include(p => p.Picture).Include(p => p.Category).Where(p => p.AspNetUserId == userId && !p.Removed && !p.Hidden);

            return new JsonResult(_mapper.Map<ICollection<ProductVM>>(prods));
        }

        [Route("soldannouncements")]
        [HttpGet]
        public IActionResult GetSoldAnnouncements(string userId)
        {
            var login = GetLoginFromRequest();
            var user = _db.Users.FirstOrDefault(x => x.Email == login);
            //var user = db.Users.Find(userId);

            var orders = _db.OrderDetail.Include(o => o.Product).
                                            ThenInclude(p => p.Picture)
                                        .Include(o => o.Product)
                                            .ThenInclude(p=> p.Category)
                                        .Include(o => o.OneUserOrder).Where(o => o.OneUserOrder.Status == Enums.OrderStatus.sent.ToString() || 
                                                                    o.OneUserOrder.Status == Enums.OrderStatus.waitingForPayment.ToString() ||
                                                                    o.OneUserOrder.Status == Enums.OrderStatus.canceled.ToString() &&
                                                                                o.Product.AspNetUserId == user.Id) ;

            return Ok(_mapper.Map<ICollection<OrderDetailVM>>(orders));
        }

        [Route("boughtannouncements")]
        [HttpGet]
        public IActionResult GetBoughtAnnouncements(string userId)
        {

            var login = GetLoginFromRequest();
            var user = _db.Users.FirstOrDefault(x => x.Email == login);
            //var user = db.Users.Find(userId);

            var orders = _db.OrderDetail.Include(o => o.Product).
                                            ThenInclude(p => p.Picture)
                                        .Include(o => o.OneUserOrder)
                                            .ThenInclude(p => p.OrderSummary)
                                        .Include(o => o.Product)
                                            .ThenInclude(p => p.Category)
                                        .Include(o => o.OneUserOrder)
                                        .ThenInclude(o => o.AspNetUser).Where(o =>  ( (o.OneUserOrder.Status == Enums.OrderStatus.sent.ToString() ||
                                                                            o.OneUserOrder.Status == Enums.OrderStatus.paymentCompleted.ToString()) &&
                                                                            o.OneUserOrder.AspNetUser.Id == user.Id));

            return Ok(_mapper.Map<ICollection<OrderDetailVM>>(orders));
        }

        [Route("activeannouncements")]
        [HttpGet]
        public IActionResult GetActiveAnnouncements(string userId)
        {

            var login = GetLoginFromRequest();
            var user = _db.Users.FirstOrDefault(x => x.Email == login);

            var products = _db.Product.Include(p => p.Picture)
                                    .Include(p => p.Category)
                                    .Where(x => x.AspNetUserId == user.Id && x.QuantityInStock > 0 && !x.Removed && !x.Hidden);

            return Ok(_mapper.Map<ICollection<ProductVM>>(products));
        }

        [Route("getuseraccount")]
        [HttpGet]
        public IActionResult GetUserAccount()
        {
            var login = GetLoginFromRequest();

            return Ok(GetUserAccountSummary(login));
        }

        //this is without 4% of commision
        private UserAccountSummary GetUserAccountSummary(string login)
        {
            //UserAccount is actually operations on Account, have to change className
            //to dobrze liczy z tym orderDetails.Any() ?
            IQueryable<UserAccount> plusesEntities = _db.UserAccount.Include(ua => ua.OrderSummary)
                                                                            .ThenInclude(os => os.OneUserOrder.Where(o => o.Status == nameof(Enums.OrderStatus.receiptConfirmed)))
                                                                            .ThenInclude(auo => auo.OrderDetail.Where(o => o.Product.AspNetUser.Email == login))
                                                                            .ThenInclude(d => d.Product)
                                                                            .ThenInclude(d => d.AspNetUser)

                                                                        .Include(ua => ua.OrderSummary)
                                                                            .ThenInclude(o => o.OneUserOrder.Where(o => o.Status == nameof(Enums.OrderStatus.receiptConfirmed)))
                                                                            .ThenInclude(auo => auo.AspNetUser)

                                                                        .Include(ua => ua.OrderSummary)
                                                                            .ThenInclude(o => o.OneUserOrder.Where(o => o.Status == nameof(Enums.OrderStatus.receiptConfirmed)))
                                                                            .ThenInclude(d => d.UserSupplyMethod)

                                                                        .Include(ua => ua.Status)

                                                        .Where(x => x.User.Email == login && 
                                                                x.StatusId == (int)Enums.AccountOperationStatus.Add && x.OrderSummary.OneUserOrder.Count() > 0);


            decimal sumOverallPluses = 0;
            foreach (var item in plusesEntities)
            {
                double sum = 0;
                foreach (var oneUserOrder in item.OrderSummary.OneUserOrder)
                {
                    if (oneUserOrder.Status != "paymentCompleted")
                    {
                        continue;
                    }

                    foreach (var oDetails in oneUserOrder.OrderDetail)
                    {
                        sum += oDetails.Product.Price * oDetails.Quantity;
                    }
                    sum += oneUserOrder.UserSupplyMethod.PricePerUnit;
                }

                if (sum > 0)
                {
                    item.OperationAmount = Convert.ToDecimal(sum);
                    sumOverallPluses += item.OperationAmount;
                }

            }

            //now get all minuses
            var minusesEntities = _db.UserAccount.Include(ua => ua.OrderSummary)
                                                    .ThenInclude(os => os.OneUserOrder.Where(o => o.Status == nameof(Enums.OrderStatus.receiptConfirmed)))
                                                    .ThenInclude(auo => auo.OrderDetail.Where(o => o.Product.AspNetUser.Email == login))
                                                    .ThenInclude(d => d.Product)
                                                    .ThenInclude(d => d.AspNetUser)

                                                .Include(ua => ua.OrderSummary)
                                                    .ThenInclude(o => o.OneUserOrder.Where(o => o.Status == nameof(Enums.OrderStatus.receiptConfirmed)))
                                                    .ThenInclude(auo => auo.AspNetUser)

                                                .Include(ua => ua.OrderSummary)
                                                    .ThenInclude(o => o.OneUserOrder.Where(o => o.Status == nameof(Enums.OrderStatus.receiptConfirmed)))
                                                    .ThenInclude(d => d.UserSupplyMethod)

                                                .Include(ua => ua.Status)

                                    .Where(x => x.User.Email == login && 
                                        x.Status.Id == (int)Enums.AccountOperationStatus.WithdrawCompleted || 
                                        x.Status.Id == (int)Enums.AccountOperationStatus.WthdrawPending);


            return new UserAccountSummary()
            {
                SumOverall = sumOverallPluses + minusesEntities.Where(m => m.StatusId ==  (int)Enums.AccountOperationStatus.WithdrawCompleted).Sum(x => x.OperationAmount),
                UserAccounts = _mapper.Map<ICollection<UserAccountVM>>(plusesEntities.Concat(minusesEntities).OrderBy(x => x.OperationDate))
            };
        }

        [Route("requestWithdraw")]
        [HttpPost]
        public async Task<IActionResult> RequestWithdraw([FromBody] WithdrawVM model)
        {
            //validation input data
            if (model.AccountNo.Length != 32)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Błędny numer konta");
            }

            var login = GetLoginFromRequest();
            var user = _db.Users.FirstOrDefault(x => x.Email == login);

            string token = GenerateConfirmWithdrawToken(login, model.AccountNo);
            var request = HttpContext.Request;
            var host = request.Host.Value;
            var scheme = request.Scheme;
            var url = scheme + "://" + host + "/api/userprofile/confirmWithdraw";

            if(model.SaveAccountNo)
            {
                await SaveAccounyNoInProfile(model.AccountNo, user);
            }

            //tutaj do msg dodać html z informacjami:
            // kwota na koncie
            // kwota która zostanie przelana czyli -4%
            // link gdzie można sprawdzić status po potwierdzeniu
            var msg = "Potwierdź wypłatę środków: " + "<a href ='" + url + "?token=" + token + "'> link </ a >";

            var companyInfo = _db.CompanyInformation.FirstOrDefault();
            await EmailService.SendAsync("Potwierdź wypłatę środków", msg, user.Email, null, companyInfo);

            return Ok();
        }

        private async Task SaveAccounyNoInProfile(string accountNo, Users user)
        {
            await _db.Entry(user).Reference(x => x.UserProfile).LoadAsync();
            user.UserProfile.AcountNo = accountNo;
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
        }


        /// <summary>
        ///     Generates new Token contains refresh token, expiration dates and login
        /// </summary>
        /// <returns>
        ///     New Access Token
        /// </returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("confirmWithdraw")]
        public async Task<IActionResult> ConfirmWithdraw(string token)
        {
            var secret = _config["Jwt:Key"];
            IEnumerable<Claim> claims = JwtHelper.DecodeToken(token, secret);
            var accountNo = claims.FirstOrDefault(x => x.Type == "accountNo").Value;
            var login = claims.FirstOrDefault(x => x.Type == "login").Value;
            var type = claims.FirstOrDefault(x => x.Type == "type").Value;


            //tutaj teraz trzeba zmienić satatus w bazie na płatność w trakcie realizacji
            //później zmienić na płatność zrrealizowano
            var user = _db.Users.FirstOrDefault(x => x.Email == login);

            if (user != null)
            {
                decimal allowedAmount = GetUserAccountSummary(login).SumOverall;
                UserAccount account = new UserAccount()
                {
                    OperationDate = DateTime.Now,
                    OperationAmount = -allowedAmount,
                    AccountNo = accountNo,
                    UserId = user.Id,
                    StatusId = (int)Enums.AccountOperationStatus.WthdrawPending

                };
                _db.UserAccount.Add(account);

                await _db.SaveChangesAsync();
            }
            

            return Ok();

        }

        //private double GetAllowedAmountForUserWithdraw(UserEntity user)
        //{
        //    var res = db.UserAccount
        //}

        /// <summary>
        ///     Generates new Token contains refresh token, expiration dates and login
        /// </summary>
        /// <returns>
        ///     New Access Token
        /// </returns>
        private string GenerateConfirmWithdrawToken(string login, string accountNo)
        {
            var payload = new Dictionary<string, object>
            {
                {"accountNo", accountNo},
                {"login", login},
                { "type", "confirmWithdraw" }
            };

            //this is redundant has to be extracted to external method
            var secret = _config["Jwt:Key"];
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            return encoder.Encode(payload, secret);
        }

    }
}