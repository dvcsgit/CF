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
    public class PermissionAttribute : ActionFilterAttribute, IActionFilter
    {
        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {

                string area = string.Empty;

                if (filterContext.RouteData.DataTokens["area"] != null)
                {
                    area = filterContext.RouteData.DataTokens["area"].ToString();
                }

                string controller = filterContext.RouteData.Values["controller"].ToString();

                if (HttpContext.Current.Session["Account"] is Account account)
                {
                    if (area != "Customized_CHIMEI" && controller != "Home")
                    {
                        if (!string.IsNullOrEmpty(area))
                        {
                            filterContext.Controller.ViewBag.CanCreate = account.WebPermissionFunctions.Any(x => x.Area == area && x.Controller == controller && x.WebFunctionId== "Create");
                            filterContext.Controller.ViewBag.CanDelete = account.WebPermissionFunctions.Any(x => x.Area == area && x.Controller == controller && x.WebFunctionId == "Delete");
                            filterContext.Controller.ViewBag.CanEdit = account.WebPermissionFunctions.Any(x => x.Area == area && x.Controller == controller && x.WebFunctionId == "Edit");
                            filterContext.Controller.ViewBag.CanQuery = account.WebPermissionFunctions.Any(x => x.Area == area && x.Controller == controller && x.WebFunctionId == "Query");
                            filterContext.Controller.ViewBag.CanClosed = account.WebPermissionFunctions.Any(x => x.Area == area && x.Controller == controller && x.WebFunctionId == "Closed");
                            filterContext.Controller.ViewBag.CanDownload = account.WebPermissionFunctions.Any(x => x.Area == area && x.Controller == controller && x.WebFunctionId == "Download");
                            filterContext.Controller.ViewBag.CanUpload = account.WebPermissionFunctions.Any(x => x.Area == area && x.Controller == controller && x.WebFunctionId == "Upload");
                            filterContext.Controller.ViewBag.CanVerify = account.WebPermissionFunctions.Any(x => x.Area == area && x.Controller == controller && x.WebFunctionId == "Verify");
                            filterContext.Controller.ViewBag.CanTakeJob = account.WebPermissionFunctions.Any(x => x.Area == area && x.Controller == controller && x.WebFunctionId == "TakeJob");
                        }
                        else
                        {
                            filterContext.Controller.ViewBag.CanCreate = account.WebPermissionFunctions.Any(x => x.Controller == controller && x.WebFunctionId == "Create");
                            filterContext.Controller.ViewBag.CanDelete = account.WebPermissionFunctions.Any(x => x.Controller == controller && x.WebFunctionId == "Delete");
                            filterContext.Controller.ViewBag.CanEdit = account.WebPermissionFunctions.Any(x => x.Controller == controller && x.WebFunctionId == "Edit");
                            filterContext.Controller.ViewBag.CanQuery = account.WebPermissionFunctions.Any(x => x.Controller == controller && x.WebFunctionId == "Query");
                            filterContext.Controller.ViewBag.CanClosed = account.WebPermissionFunctions.Any(x => x.Controller == controller && x.WebFunctionId == "Closed");
                            filterContext.Controller.ViewBag.CanDownload = account.WebPermissionFunctions.Any(x => x.Controller == controller && x.WebFunctionId == "Download");
                            filterContext.Controller.ViewBag.CanUpload = account.WebPermissionFunctions.Any(x => x.Controller == controller && x.WebFunctionId == "Upload");
                            filterContext.Controller.ViewBag.CanVerify = account.WebPermissionFunctions.Any(x => x.Controller == controller && x.WebFunctionId == "Verify");
                            filterContext.Controller.ViewBag.CanTakeJob = account.WebPermissionFunctions.Any(x => x.Controller == controller && x.WebFunctionId == "TakeJob");
                        }
                    }
                }
                else
                {
                    filterContext.Controller.ViewBag.CanCreate = false;
                    filterContext.Controller.ViewBag.CanDelete = false;
                    filterContext.Controller.ViewBag.CanEdit = false;
                    filterContext.Controller.ViewBag.CanQuery = false;
                    filterContext.Controller.ViewBag.CanClosed = false;
                    filterContext.Controller.ViewBag.CanDownload = false;
                    filterContext.Controller.ViewBag.CanUpload = false;
                    filterContext.Controller.ViewBag.CanVerify = false;
                    filterContext.Controller.ViewBag.CanTakeJob = false;
                }
            }
            catch (Exception ex)
            {
                filterContext.Controller.ViewBag.CanCreate = false;
                filterContext.Controller.ViewBag.CanDelete = false;
                filterContext.Controller.ViewBag.CanEdit = false;
                filterContext.Controller.ViewBag.CanQuery = false;
                filterContext.Controller.ViewBag.CanClosed = false;
                filterContext.Controller.ViewBag.CanDownload = false;
                filterContext.Controller.ViewBag.CanUpload = false;
                filterContext.Controller.ViewBag.CanVerify = false;
                filterContext.Controller.ViewBag.CanTakeJob = false;

                Logger.Log(MethodBase.GetCurrentMethod(), ex);
            }
        }

        void IActionFilter.OnActionExecuted(ActionExecutedContext filterContext)
        {

        }
    }
}