﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using AppTemplate.Tasks;

namespace AppTemplate.Controllers {
  [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes",
    Justification =
      "Unsealed so that subclassed types can set properties in the default constructor or override our behavior.")]
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
  public class SimpleAuthorizeAttribute : FilterAttribute, IAuthorizationFilter {

    private string _roles;
    private string[] _rolesSplit = new string[0];
    private string _userRole;

    private readonly IAccountTasks _accountTasks;

    public SimpleAuthorizeAttribute() {
      _accountTasks = DependencyResolver.Current.GetService<IAccountTasks>();
    }

    public string Roles {
      get { return _roles ?? String.Empty; }
      set {
        _roles = value;
        _rolesSplit = SplitString(value);
      }
    }


    // This method must be thread-safe since it is called by the thread-safe OnCacheAuthorization() method.
    protected virtual bool AuthorizeCore(HttpContextBase httpContext) {
      if (httpContext == null) {
        throw new ArgumentNullException("httpContext");
      }

      IPrincipal user = httpContext.User;
      if (!user.Identity.IsAuthenticated) {
        return false;
      }

      if (_rolesSplit.Length > 0) {
        _userRole = _accountTasks.GetUserRole(user.Identity.Name);
        return _rolesSplit.Any(x => _userRole == x.ToLower());
      }
      return true;
    }

    private void CacheValidateHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus) {
      validationStatus = OnCacheAuthorization(new HttpContextWrapper(context));
    }

    public virtual void OnAuthorization(AuthorizationContext filterContext) {
      if (filterContext == null) {
        throw new ArgumentNullException("filterContext");
      }

      if (OutputCacheAttribute.IsChildActionCacheActive(filterContext)) {
        // If a child action cache block is active, we need to fail immediately, even if authorization
        // would have succeeded. The reason is that there's no way to hook a callback to rerun
        // authorization before the fragment is served from the cache, so we can't guarantee that this
        // filter will be re-run on subsequent requests.
        throw new InvalidOperationException("CannotUseWithinChildActionCache");
      }

      bool skipAuthorization = filterContext.ActionDescriptor.IsDefined(typeof (AllowAnonymousAttribute), inherit: true)
                               ||
                               filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(
                                 typeof (AllowAnonymousAttribute), inherit: true);

      if (skipAuthorization) {
        return;
      }

      if (AuthorizeCore(filterContext.HttpContext)) {
        // ** IMPORTANT **
        // Since we're performing authorization at the action level, the authorization code runs
        // after the output caching module. In the worst case this could allow an authorized user
        // to cause the page to be cached, then an unauthorized user would later be served the
        // cached page. We work around this by telling proxies not to cache the sensitive page,
        // then we hook our custom authorization code into the caching mechanism so that we have
        // the final say on whether a page should be served from the cache.
        //filterContext.Controller.ViewData["UserRole"] = _userRole;


        HttpCachePolicyBase cachePolicy = filterContext.HttpContext.Response.Cache;
        cachePolicy.SetProxyMaxAge(new TimeSpan(0));
        cachePolicy.AddValidationCallback(CacheValidateHandler, null /* data */);
      }
      else {
        HandleUnauthorizedRequest(filterContext);
      }
    }

    protected virtual void HandleUnauthorizedRequest(AuthorizationContext filterContext) {
      // Returns HTTP 401 - see comment in HttpUnauthorizedResult.cs.
      filterContext.Result = new HttpUnauthorizedResult();
    }

    // This method must be thread-safe since it is called by the caching module.
    protected virtual HttpValidationStatus OnCacheAuthorization(HttpContextBase httpContext) {
      if (httpContext == null) {
        throw new ArgumentNullException("httpContext");
      }

      bool isAuthorized = AuthorizeCore(httpContext);
      return (isAuthorized) ? HttpValidationStatus.Valid : HttpValidationStatus.IgnoreThisRequest;
    }

    internal static string[] SplitString(string original) {
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