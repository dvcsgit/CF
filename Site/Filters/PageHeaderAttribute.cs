using Models.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Utility;

namespace Site.Filters
{
    public class PageHeaderAttribute : ActionFilterAttribute, IActionFilter
    {
        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            var account = HttpContext.Current.Session["Account"] as Account;

            try
            {
                string area = string.Empty;

                if (filterContext.RouteData.DataTokens["area"] != null)
                {
                    area = filterContext.RouteData.DataTokens["area"].ToString();
                }

                string controller = filterContext.RouteData.Values["controller"].ToString();

                if (controller != "Home")
                {
                    if (account != null)
                    {
                        var parentPage = account.MenuItems.FirstOrDefault(a => a.SubItemList.Any(x => x.Area == area && x.Controller == controller));

                        if (parentPage != null)
                        {
                            var page = parentPage.SubItemList.FirstOrDefault(x => x.Area == area && x.Controller == controller);

                            if (page != null)
                            {
                                //filterContext.Controller.ViewBag.Controller = page.ID;
                                filterContext.Controller.ViewBag.FunctionName = page.Name[filterContext.Controller.ViewBag.Lang];
                            }
                            else
                            {
                                //filterContext.Controller.ViewBag.Controller = null;
                                filterContext.Controller.ViewBag.FunctionName = null;
                            }
                        }
                        else
                        {
                            //filterContext.Controller.ViewBag.Controller = null;
                            filterContext.Controller.ViewBag.FunctionName = null;
                        }
                    }
                    else
                    {
                        //filterContext.Controller.ViewBag.Controller = null;
                        filterContext.Controller.ViewBag.FunctionName = null;
                    }
                }
                else
                {
                    //filterContext.Controller.ViewBag.Controller = null;
                    filterContext.Controller.ViewBag.FunctionName = null;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), ex);

                //filterContext.Controller.ViewBag.Controller = null;
                filterContext.Controller.ViewBag.FunctionName = null;
            }
        }

        void IActionFilter.OnActionExecuted(ActionExecutedContext filterContext)
        {

        }
    }
}