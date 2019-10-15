using System.Threading;
using System.Web.Mvc;

namespace Site.Filters
{
    public class CultureActionFilter:ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            string lang;

            if (filterContext.RouteData.Values.ContainsKey("Lang"))
            {
                lang = filterContext.RouteData.Values["Lang"].ToString().ToLower();
            }
            else
            {
                lang = "zh-cn";
            }

            filterContext.Controller.ViewBag.Lang = lang;
            
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(lang);
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lang);
        }
    }
}