using DataAccessor;
using Models.Authentication;
using Models.Person;
using Models.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Utility;
using Utility.Models;

namespace Site.Controllers
{
    public class PersonController : Controller
    {
        // GET: Person
        public ActionResult Index()
        {
            return View(new QueryFormModel());
        }       

        public ActionResult Query(QueryParameters queryParameters)
        {
            RequestResult requestResult = PersonDataAccessor.Query(queryParameters, Session["Account"] as Account);

            if (requestResult.IsSuccess)
            {
                return PartialView("_List", requestResult.Data);
            }
            else
            {
                return PartialView("_Error", requestResult.Error);
            }
        }

        public ActionResult Detail(string pId)
        {
            RequestResult result = PersonDataAccessor.GetDetailViewModel(pId, Session["Account"] as Account);

            if (result.IsSuccess)
            {
                return PartialView("_Detail", result.Data);
            }
            else
            {
                return PartialView("_Error", result.Error);
            }
        }

        [HttpGet]
        public ActionResult Create(string organizationId)
        {
            RequestResult result = PersonDataAccessor.GetCreateFormModel(new Guid(organizationId));

            if (result.IsSuccess)
            {
                return PartialView("_Create", result.Data);
            }
            else
            {
                return PartialView("_Error", result.Error);
            }
        }

        [HttpPost]
        public ActionResult Create(CreateFormModel createFormModel)
        {
            RequestResult result = PersonDataAccessor.Create(createFormModel);

            if (result.IsSuccess)
            {
                HttpRuntime.Cache.Remove("Users");
            }

            return Content(JsonConvert.SerializeObject(result));
        }

        public ActionResult Copy(string pId)
        {
            RequestResult result = PersonDataAccessor.GetCopyFormModel(pId);

            if (result.IsSuccess)
            {
                return PartialView("_Create", result.Data);
            }
            else
            {
                return PartialView("_Error", result.Error);
            }
        }        

        [HttpGet]
        public ActionResult Edit(string pId)
        {
            RequestResult result = PersonDataAccessor.GetEditFormModel(pId);

            if (result.IsSuccess)
            {
                return PartialView("_Edit", result.Data);
            }
            else
            {
                return PartialView("_Error", result.Error);
            }
        }

        [HttpPost]
        public ActionResult Edit(EditFormModel editFormModel)
        {
            RequestResult result = PersonDataAccessor.Edit(editFormModel);

            if (result.IsSuccess)
            {
                HttpRuntime.Cache.Remove("Users");
            }

            return Content(JsonConvert.SerializeObject(result));
        }

        public ActionResult Delete(string selecteds)
        {
            RequestResult result = new RequestResult();

            try
            {
                var selectedList = JsonConvert.DeserializeObject<List<string>>(selecteds);

                result = PersonDataAccessor.Delete(selectedList);

                if (result.IsSuccess)
                {
                    HttpRuntime.Cache.Remove("Users");
                }
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return Content(JsonConvert.SerializeObject(result));
        }

        public ActionResult Move(string organizationId, string selecteds)
        {
            RequestResult result = new RequestResult();

            try
            {
                var selectedList = JsonConvert.DeserializeObject<List<string>>(selecteds);

                result = PersonDataAccessor.Move(organizationId, selectedList);

                if (result.IsSuccess)
                {
                    HttpRuntime.Cache.Remove("Users");
                }
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return Content(JsonConvert.SerializeObject(result));
        }

        public ActionResult ResetPassword(string pId)
        {
            return Content(JsonConvert.SerializeObject(PersonDataAccessor.ResetPassword(pId)));
        }

        public ActionResult InitTree()
        {
            try
            {
                var account = Session["Account"] as Account;
                var organizations = HttpRuntime.Cache.GetOrInsert<List<Models.Shared.Organization>>("Organization", () => OrganizationDataAccessor.GetAllOrganizations());

                RequestResult requestResult = new RequestResult();

                if (account.RootOrganizationId == new Guid())
                {
                    requestResult = PersonDataAccessor.GetTreeItem(organizations, account.RootOrganizationId, account);
                }
                else
                {
                    requestResult = PersonDataAccessor.GetRootTreeItem(organizations, account);
                }

                if (requestResult.IsSuccess)
                {
                    return PartialView("_Tree", JsonConvert.SerializeObject((List<TreeItem>)requestResult.Data));
                }
                else
                {
                    return PartialView("_Error", requestResult.Error);
                }
            }
            catch (Exception e)
            {
                var error = new Error(MethodBase.GetCurrentMethod(), e);
                Logger.Log(error);
                return PartialView("_Error", error);
                //throw;
            }
        }

        public ActionResult GetTreeItem(string organizationId)
        {
            string jsonTree = string.Empty;

            try
            {
                var organizations = HttpRuntime.Cache.GetOrInsert<List<Models.Shared.Organization>>("Organizations", () => OrganizationDataAccessor.GetAllOrganizations());

                RequestResult result = PersonDataAccessor.GetTreeItem(organizations, new Guid(organizationId), Session["Account"] as Account);

                if (result.IsSuccess)
                {
                    jsonTree = JsonConvert.SerializeObject((List<TreeItem>)result.Data);
                }
                else
                {
                    jsonTree = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), ex);

                jsonTree = string.Empty;
            }

            return Content(jsonTree);
        }
    }
}