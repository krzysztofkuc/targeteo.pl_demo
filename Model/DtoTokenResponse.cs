using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using targeteo.pl.Model.Entities;

namespace targeteo.pl.Model
{
    public class DtoTokenResponse
    {
        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        public string Login { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string RefreshTokenExprarationDate { get; set; }
        public string Id { get; set; }
        public string[] Roles { get; set; }
    }
}
