using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
//using AuthenticationService.CommonHelpers;
//using WebApp.Helpers;
using targeteo.pl.Helpers;

namespace targeteo.pl.Handlers
{
    public class UserAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        IConfiguration _config;

        public UserAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IConfiguration config)
            : base(options, logger, encoder, clock)
        {
            _config = config;
        }

        /// <summary>
        ///     Gets the http context accessor.
        /// </summary>
        private IHttpContextAccessor HttpContextAccessor { get; }

        private IMemoryCache UserCacheRepository { get; }

        /// <summary>
        ///     Checks if user is authenticated
        /// </summary>
        /// <returns></returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {

            var expr = new Regex(@"\b(assets|login|([A-Za-z]+.js)|([A-Za-z0-9]+.png)|([A-Za-z0-9]+.jpg),|([A-Za-z0-9]+.woff2))\b");
            return await Task.Run(
                () =>
                {
                    if (expr.IsMatch(Request.Path))
                    {
                        return AuthenticateResult.NoResult();
                    }

                    if (!Request.Headers.ContainsKey("Authorization"))
                    {
                        return AuthenticateResult.Fail("Missing Authorization Header");
                    }

                    try
                    {
                        var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                        var secret = _config["Jwt:Key"];
                        var tokenClaims = JwtHelper.DecodeToken(authHeader.Parameter, secret);
                        var login = tokenClaims.FirstOrDefault(x => x.Type == "login").Value;
                        var rolesClaim = tokenClaims.FirstOrDefault(x => x.Type == Consts.ClaimTypeRoles)?.Value;
                        var expAccessToken =
                            Convert.ToInt64(tokenClaims.FirstOrDefault(x => x.Type == "expiration").Value);
                        var timeOffsetAccessToken =
                            DateTimeOffset.FromUnixTimeSeconds(expAccessToken).DateTime.ToLocalTime();
                        var accessTokenExpired = DateTime.Now > timeOffsetAccessToken;
                        //var refreshToken = tokenClaims.FirstOrDefault(x => x.Type == "refreshToken").Value;

                        if (login == null)
                        {
                            return AuthenticateResult.Fail("Invalid Authentication.");
                        }

                        var authClaims = new List<Claim>()
                        {
                            new Claim(ClaimTypes.Name, login)
                        };

                        if (!string.IsNullOrEmpty(rolesClaim))
                        {
                            authClaims.Add(new Claim(Consts.ClaimTypeRoles, rolesClaim));
                        }

                        if (accessTokenExpired)
                        {
                            return AuthenticateResult.Fail("' login' + '?reutnrUrl=" + Context.Request.Path);
                        }

                        var identity = new ClaimsIdentity(authClaims, Consts.DefaultAuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);
                        var ticket = new AuthenticationTicket(principal, Consts.DefaultAuthenticationScheme);

                        return AuthenticateResult.Success(ticket);
                    }
                    catch
                    {
                        return AuthenticateResult.Fail("' login' + '?reutnrUrl=" + Context.Request.Path);
                    }
                });
        }
    }
}
