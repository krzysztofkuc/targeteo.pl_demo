using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace targeteo.pl.Helpers
{
    public static class Consts
    {
        #region Auth
        public const string Login = "login";
        public const string ClaimTypeRoles = "customRoles";
        public const string CustomRolePolicy = "RoleHasAccess";
        public const string LoggedUserPolicy = "LoggedUserPolicy";
        public const string DefaultAuthenticationScheme = "Basic";
        public const string DomainLoginRole = "DomainLogin";
        #endregion Auth
    }
}
