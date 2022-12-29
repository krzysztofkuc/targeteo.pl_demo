using AutoMapper;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using targeteo.pl.Helpers;
using targeteo.pl.Model;
using targeteo.pl.Model.Entities;
using targeteo.pl.Model.ViewModel;
using targeteo.pl.Services;

namespace targeteo.pl.Controllers
{
    [Route("api/[controller]")]
    //[Authorize(AuthenticationSchemes = Consts.DefaultAuthenticationScheme, Policy = Consts.CustomRolePolicy)]
    [ApiController]
    public class AuthController : BaseController
    {
        public AuthController(IConfiguration config, IMapper mapper, ApplicationDbContext db)
            : base(config, db, mapper, null)
        {
            //_loginProvider = loginProvider;
            //_tokenProvider = tokenProvider;
            //_cache = cache;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(UserVM user)
        {
            try
            {
                //var result =  _loginProvider.ValidateCred`entials(user.Email, user.Password);
                //RegisterUser(user);
                    //here we shou;ld hash password
                    var dbUser = await _db.Users
                                    .Include(u => u.UserRoles).ThenInclude( r => r.Role).SingleOrDefaultAsync(x => x.Email == user.Email);

                if (dbUser == null)
                {
                    return Unauthorized("Invalid login or password");
                }
                else if (dbUser.PasswordHash == Unhash(user.Password, dbUser.SecurityStamp))
                {
                    var tokenResponse = await GetTokenAuthorized(dbUser);

                    return Ok(tokenResponse);
                }
                else
                {
                    return Unauthorized("Błędny login lub hasło");
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("registerUser")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser(UserVM user)
        {
            try
            {
                var usrInDb = _db.Users.FirstOrDefault(x => x.Email == user.Email);
                if(usrInDb != null && !usrInDb.EmailConfirmed)
                {
                    var task = await SendEmailToConfirmAdress(user.Email);
                    return Json(new JsonHelperDto().Value = "resendConfirmEmail");
                }
                else if(usrInDb != null && usrInDb.EmailConfirmed)
                {
                    return Json(new JsonHelperDto().Value = "userIsAlreadyRegitered");
                }
                else
                {
                    using (var dbContextTransaction = _db.Database.BeginTransaction())
                    {
                        Users ue = new Users();
                        ue.Id = user.Id = Guid.NewGuid().ToString();
                        ue.Email = user.Email;
                        ue.UserName = user.Email;
                        var passAndSalt = HashPassword(user.Password);
                        ue.PasswordHash = passAndSalt.Item1;
                        ue.SecurityStamp = passAndSalt.Item2;
                        ue.PhoneNumberConfirmed = false;
                        ue.Banned = false;
                        ue.RegisterDate = DateTime.Now;
                        ue.TwoFactorEnabled = false;

                        ue.UserProfileId = CreateUserProfile(ue).Result.Entity.UserProfileId;

                        //roes hardcoded!!!!!
                        var role = _db.Roles.FirstOrDefault(role => role.Name == "User");
                        var userRoles = new UserRoles() { User = ue, Role = role };

                        _db.UserRoles.Add(userRoles);
                        _db.Users.Add(ue);
                        _db.SaveChanges();

                        var task = await SendEmailToConfirmAdress(user.Email);
                        return Json(new JsonHelperDto().Value = "resendConfirmEmail");
                    }
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private async Task<EntityEntry<UserProfile>> CreateUserProfile(Users ue)
        {
            var userProfile = new UserProfile()
            {
                AcountNo = String.Empty
            };

            return await _db.UserProfile.AddAsync(userProfile);
        }

        private Tuple<string, string> HashPassword(string password)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return Tuple.Create(hashed, Convert.ToBase64String(salt));
        }

        private string Unhash(string password, string securityStamp)
        {
            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
             return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: password,
                    salt: Convert.FromBase64String(securityStamp),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8));
        }

        [HttpPost("Refresh")]
        public async Task<IActionResult> GetByRefreshToken(JsonHelperDto token)
        {
            try
            {
                var result = await GetTokenWithRefreshToken(token.Value);

                if (result == null)
                {
                    return BadRequest("Refresh token invalid or expired");
                }

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("logout")]
        public IActionResult Logout()
        {
            //MyLogger.GetInstance().Info("test");
            if (!string.IsNullOrEmpty(Request.Headers["Authorization"]))
            {
                var login = GetLoginFromRequest();
                //await _loginProvider.DeleteDeviceId(login);
            }

            return Ok();
        }

        [HttpGet]
        [Route("sendEmailToResetPassword")]
        public async Task<IActionResult> SendEmailToResetPassword(string email)
        {
            string token = GenerateResetPasswordToken(email);

            var request = HttpContext.Request;

            var host = request.Host.Value;
            var scheme = request.Scheme;
            var url = scheme + "://" + host;
            url += "/resetPassword";

            var msg = "Click link to reset password" + "<a href ='" + url + "?token=" + token + "'> Link </ a >";

            var companyInfo = _db.CompanyInformation.FirstOrDefault();
            await EmailService.SendAsync("Reset password", msg, email, null, companyInfo);

            return Ok();
        }

        [HttpGet]
        [Route("sendEmailToConfirmAdress")]
        public async Task<IActionResult> SendEmailToConfirmAdress(string email)
        {
            string token = GenerateConfirmEmailToken(email);
            var request = HttpContext.Request;
            var host = request.Host.Value;
            var scheme = request.Scheme;
            var url = scheme + "://" + host + "/confirmEmail";

            var msg = "Potwierdź adres: " + "<a href ='" + url + "?token=" + token + "'> link </ a >";

            var companyInfo = _db.CompanyInformation.FirstOrDefault();
            await EmailService.SendAsync("Confirm email", msg, email, null, companyInfo);

            return Ok();
        }

        [HttpGet]
        [Route("resetPassword")]
        public async Task<IActionResult> ResetPassword(string password, string token)
        {
            if(CanResetPassword(token))
            {
                var secret = _config["Jwt:Key"];
                IEnumerable<Claim> claims = JwtHelper.DecodeToken(token, secret);
                var login = claims.FirstOrDefault(x => x.Type == "login").Value;

                var user = _db.Users.FirstOrDefault( x => x.Email == login);

                if(user != null)
                {
                    var passAndSalt = HashPassword(password);
                    user.PasswordHash = passAndSalt.Item1;
                    user.SecurityStamp = passAndSalt.Item2;

                    _db.Update(user);
                    await _db.SaveChangesAsync();

                    return Ok();
                }
            }

            return BadRequest();
        }

        [HttpGet]
        [Route("canResetPassword")]
        public bool CanResetPassword(string token)
        {
            var secret = _config["Jwt:Key"];
            IEnumerable<Claim> claims = JwtHelper.DecodeToken(token, secret);

            var type = claims.FirstOrDefault(x => x.Type == "type").Value;
            var expiraqtion = claims.FirstOrDefault(x => x.Type == "expiration").Value;

            if (type == "resetPass")
            {
                var expTime = UnixTimeStampToDateTime(Convert.ToInt64(expiraqtion));

                if(expTime > DateTime.Now)
                {
                    return true;
                }
            }

            return false;
        }

        [HttpGet]
        [Route("canConfirmEmail")]
        public bool CanConfirmEmail(string token)
        {
            var secret = _config["Jwt:Key"];
            IEnumerable<Claim> claims = JwtHelper.DecodeToken(token, secret);

            var type = claims.FirstOrDefault(x => x.Type == "type").Value;
            var expiraqtion = claims.FirstOrDefault(x => x.Type == "expiration").Value;

            if (type == "confirmEmail")
            {
                var expTime = UnixTimeStampToDateTime(Convert.ToInt64(expiraqtion));

                if (expTime > DateTime.Now)
                {
                    return true;
                }
            }

            return false;
        }

        [HttpGet]
        [Route("confirmEmail")]
        public async Task<bool> ConfirmEmail(string token)
        {
            var secret = _config["Jwt:Key"];
            IEnumerable<Claim> claims = JwtHelper.DecodeToken(token, secret);

            var type = claims.FirstOrDefault(x => x.Type == "type").Value;
            var expiraqtion = claims.FirstOrDefault(x => x.Type == "expiration").Value;
            var email = claims.FirstOrDefault(x => x.Type == "login").Value;

            var user = _db.Users.FirstOrDefault(x => x.Email == email);

            if(user != null)
            {
                user.EmailConfirmed = true;

                //db.Attach(user);
                //db.Entry(user).Property("Name").IsModified = true;

                _db.Update(user);

                await _db.SaveChangesAsync();
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        private async Task<DtoTokenResponse> GetTokenAuthorized(Users user)
        {
            var result = await GenerateAndSaveNewToken(user);
            var token = result[0];
            var refTokExpDate = result[1];
            var refreshToken = result[2];
            return new DtoTokenResponse
            {
                Token = token,
                RefreshTokenExprarationDate = refTokExpDate,
                RefreshToken = refreshToken,
                Login = user.Email,
                Id = user.Id,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToArray()
            };
        }

        private async Task<DtoTokenResponse> GetTokenWithRefreshToken(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                return null;
            }

            var dbUser = await _db.Users.SingleOrDefaultAsync(x => x.RefreshToken.Equals(refreshToken));

            if (dbUser == null)
            {
                return null;
            }

            if (CheckIfTokenExpired(dbUser.TokenExpirationDateTime))
            {
                return null;
            }

            var result = await GenerateAndSaveNewToken(dbUser);
            var token = result[0];
            var refTokExpDate = result[1];
            var newRefreshToken = result[2];
            return new DtoTokenResponse { Token = token, RefreshTokenExprarationDate = refTokExpDate, RefreshToken = newRefreshToken };
        }
        private bool CheckIfTokenExpired(DateTime? expiresUTC)
        {
            return expiresUTC < DateTime.UtcNow;
        }

        private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        /// <summary>
        ///     Generates new Token contains refresh token, expiration dates and login
        /// </summary>
        /// <returns>
        ///     New Access Token
        /// </returns>
        private string GenerateResetPasswordToken(string login)
        {
            var payload = new Dictionary<string, object>
            {
                {"expiration", DateTimeOffset.UtcNow.ToLocalTime().AddMinutes(10).ToUnixTimeSeconds()},
                {"login", login},
                { "type", "resetPass" }
            };

            var secret = _config["Jwt:Key"];
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            return encoder.Encode(payload, secret);
        }

        /// <summary>
        ///     Generates new Token contains refresh token, expiration dates and login
        /// </summary>
        /// <returns>
        ///     New Access Token
        /// </returns>
        private string GenerateConfirmEmailToken(string login)
        {
            var payload = new Dictionary<string, object>
            {
                {"expiration", DateTimeOffset.UtcNow.ToLocalTime().AddMinutes(10).ToUnixTimeSeconds()},
                {"login", login},
                { "type", "confirmEmail" }
            };

            var secret = _config["Jwt:Key"];
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            return encoder.Encode(payload, secret);
        }

        /// <summary>
        ///     Generates new Token contains refresh token, expiration dates and login
        /// </summary>
        /// <returns>
        ///     New Access Token
        /// </returns>
        private async Task<string[]> GenerateAndSaveNewToken(Users user)
        {

            

            var refreshToken = Guid.NewGuid().ToString("n");
            var refreshTokenEncoded = this.EncodeRefreshToken(refreshToken);
            var accTokExpTime = Convert.ToInt16(_config["Jwt:AccessTokenExpirationTimeInMinutes"]);
            var refTokExpTime = Convert.ToInt16(_config["Jwt:RefreshTokenExpirationTimeInHours"]);
            var refTokExpDate = DateTime.UtcNow.ToLocalTime().AddHours(refTokExpTime);
            var roles = user.UserRoles.Select(r => r.Role.Name).ToArray();
            //var roles = WebApiMembershipHelper.GetRoles(user.Email);

            await UpdateOrInsertUserRefreshToken(user.Email, refreshTokenEncoded, refTokExpDate);

            var payload = new Dictionary<string, object>
            {
                {"expiration", DateTimeOffset.UtcNow.ToLocalTime().AddMinutes(accTokExpTime).ToUnixTimeSeconds()},
                {"login", user.Email},
                {
                    "refreshTokenExpirationDate",
                    DateTimeOffset.UtcNow.ToLocalTime().AddHours(refTokExpTime).ToUnixTimeSeconds()
                },
                { Consts.ClaimTypeRoles, roles }
            };

            var secret = _config["Jwt:Key"];
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            //serializer.Converters.Add(new JavaScriptDateTimeConverter());
            //serializer.NullValueHandling = NullValueHandling.Ignore;

            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            var body = encoder.Encode(payload, secret);
            return new string[] { body , refTokExpDate.ToString(), refreshTokenEncoded };
        }

        private async Task UpdateOrInsertUserRefreshToken(string login, string refreshToken, DateTime expiresDate)
        {
            var dbUser = await this._db.Users.SingleOrDefaultAsync(x => x.Email.Equals(login));

            if (dbUser == null)
            {
                dbUser = new Model.Entities.Users { Email = login, };
                _db.Users.Add(dbUser);
            }

            dbUser.RefreshToken = refreshToken;
            //dbUser.IssuedUTC = DateTime.UtcNow.ToLocalTime();
            dbUser.TokenExpirationDateTime = expiresDate.ToLocalTime();
            //dbUser.DeviceId = deviceId;
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// The encode refresh token.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string EncodeRefreshToken(string token)
        {
            byte[] textAsBytes = Encoding.UTF8.GetBytes(token);
            return Convert.ToBase64String(textAsBytes);
        }
    }
}