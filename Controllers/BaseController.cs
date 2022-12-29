using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using targeteo.pl.Helpers;
using targeteo.pl.Model;

namespace targeteo.pl.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IConfiguration _config;
        protected readonly ApplicationDbContext _db;
        protected readonly IMapper _mapper;
        protected readonly IWebHostEnvironment _hostingEnvironment;

        public BaseController(IConfiguration config, ApplicationDbContext db, IMapper mapper, IWebHostEnvironment hostingEnvironment)
        {
            _config = config;
            _db = db;
            _mapper = mapper;
            _hostingEnvironment = hostingEnvironment;
        }

        protected string GetLoginFromRequest()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return null;
            }

            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            var secret = _config["Jwt:Key"];
            var tokenClaims = JwtHelper.DecodeToken(authHeader.Parameter, secret);
            return tokenClaims.FirstOrDefault(x => x.Type == Consts.Login)?.ToString().Split(": ")[1];
        }
    }
}