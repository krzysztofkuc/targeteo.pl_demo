using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using targeteo.pl.Helpers;

namespace targeteo.pl.Handlers
{
    public class UserAuthorizationHandler : AuthorizationHandler<UserAuthorizationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserAuthorizationRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == Consts.ClaimTypeRoles))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var userRoles = context.User.Claims.FirstOrDefault(x => x.Type == Consts.ClaimTypeRoles)?.ToString();
            var userRolesArray = userRoles.Split($"{Consts.ClaimTypeRoles}: ", ',').Skip(1).First().Split(',');

            //here we can get data which role to which view from xml file

            var endpoint = context.Resource as RouteEndpoint;


            //this have to be fixe !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //quick fix 
            if (!(userRolesArray.Contains("User") || userRolesArray.Contains("Admin")))
            {
                context.Fail();
                return Task.CompletedTask;
            }
            //-----------------------------------------

            //if (endpoint.DisplayName.StartsWith("targeteo.pl.Controllers.LoggedUserController"))
            //{
            //    if (!(userRolesArray.Contains("User") || userRolesArray.Contains("Admin")))
            //    {
            //        context.Fail();
            //        return Task.CompletedTask;
            //    }
            //}
            //else if(endpoint.DisplayName.StartsWith("targeteo.pl.Controllers.RootController"))
            //{
            //    if (!userRolesArray.Contains("Admin"))
            //    {
            //        context.Fail();
            //        return Task.CompletedTask;
            //    }
            //}

            #region Here we can configure which role can has acces to which view

            //var descriptor = endpoint?.Metadata?
            //    .SingleOrDefault(md => md is ControllerActionDescriptor) as ControllerActionDescriptor;

            //if (descriptor == null)
            //    throw new InvalidOperationException("Unable to retrieve current action descriptor.");

            //var controllerName = descriptor.ControllerName;
            //var actionName = descriptor.ActionName;

            ////Should be cached or taken from DB
            //var xml = new XmlDocument();
            //xml.Load("Resources/SiteMap.xml");
            //XmlNodeList nodeList = xml.SelectNodes($"/node/viewAccesNode[@url='{controllerName}/{actionName}']");

            //if (nodeList.Count > 0)
            //{
            //    string systemRoles = nodeList.Item(0).Attributes["roles"].Value;
            //    string[] systemRolesArray = systemRoles.Split(',');

            //    foreach (var userRole in userRolesArray)
            //    {
            //        if (systemRolesArray.Contains(userRole))
            //        {
            //            context.Succeed(requirement);
            //            break;
            //        }
            //    }
            //}

            #endregion
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }

    public class UserAuthorizationRequirement : IAuthorizationRequirement
    {
        public UserAuthorizationRequirement() { }
    }
}
