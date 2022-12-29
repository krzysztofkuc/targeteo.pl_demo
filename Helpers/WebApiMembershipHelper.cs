using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace targeteo.pl.Helpers
{
    public class WebApiMembershipHelper
    {
		public static string GetRoles(string userName)
		{

			var systemRoles = GetRolesForUser(userName);

			if (systemRoles.Count() == 0)
			{
				return null;
			}
			
			return null;
		}

		private static string[] GetRolesForUser(string userName)
		{

			return new string[0];
		}
	}
}
