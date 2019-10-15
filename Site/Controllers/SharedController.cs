using DataAccessor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utility;
using Utility.Models;
using Webdiyer.WebControls.Mvc;

namespace Site.Controllers
{
    public class SharedController : Controller
    {
        // GET: Shared
        public ActionResult GetPersonOptions_Multi([DefaultValue(1)]int PageIndex, [DefaultValue(10)]int PageSize, string Term, [DefaultValue(false)]bool IsInit)
        {
            var userList = HttpRuntime.Cache.GetOrInsert<List<Models.Shared.PersonModel>>("Users", () => AccountDataAccessor.GetAllPeople());

            RequestResult result = PersonDataAccessor.GetPersonOptions(userList, Term, IsInit);

            if (result.IsSuccess)
            {
                var queryResult = result.Data as List<SelectListItem>;

                var data = queryResult.Select(x => new { id = x.Value, text = x.Text, name = x.Text }).AsQueryable().OrderBy(x => x.id).ToPagedList(PageIndex, PageSize);

                return Json(new { Success = true, Data = data, Total = data.TotalItemCount }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = false, Message = result.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}