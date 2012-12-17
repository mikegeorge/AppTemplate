using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using AppTemplate.Tasks;

namespace AppTemplate.Extensions {
  public static class AuthorizationExtensions {
    private static IAccountTasks _accountTasks;

    public static bool IsAuthorized(this IPrincipal user, string roles) {
      if (user.Identity.IsAuthenticated) {
        _accountTasks = DependencyResolver.Current.GetService<IAccountTasks>();
        var userRole = _accountTasks.GetUserRole(user.Identity.Name);
        return SplitString(roles).Any(x => userRole.ToLower() == x.ToLower());
      }
      return false;
    }

    static IEnumerable<string> SplitString(string original) {
      if (String.IsNullOrEmpty(original)) {
        return new string[0];
      }

      var split = from piece in original.Split(',')
                  let trimmed = piece.Trim()
                  where !String.IsNullOrEmpty(trimmed)
                  select trimmed;
      return split.ToArray();
    }
  }
}