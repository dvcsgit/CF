using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Site.Filters
{
    public class AjaxAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //若要httpcontext不为空，需要在web.config配置authentication
            if (httpContext != null)
            {
                if (!httpContext.User.Identity.IsAuthenticated)
                    return false;
                else if (httpContext.Session["Account"] == null)
                    return false;
                else
                    return true;
            }
            else
            {
                return false;
            }
        }


        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (filterContext.Result is HttpUnauthorizedResult)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {                  
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                            {
                                { "controller", "Home" },
                                { "action", "Login" }
                            });
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                            {
                                { "controller", "Home" },
                                { "action", "Login" }
                            });
                }
            }
        }
    }
}