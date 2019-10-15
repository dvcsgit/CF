using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utility.Models;
using Models.Maintenance.Solution;
using Models.Authentication;
using System.Reflection;
using Utility;
using Newtonsoft.Json;
using Models.Shared;
using DataAccessor;
using DataAccessor.Maintenance;

namespace Site.Areas.Maintenance.Controllers
{
    public class SolutionController : Controller
    {
        // GET: Maintenance/Solution
        public ActionResult Index()
        {
            return View(new QueryFormModel());
        }

        public ActionResult Query(QueryParameters queryParameters)
        {
            RequestResult result = SolutionDataAccessor.Query(queryParameters, Session["Account"] as Account);

            if (result.IsSuccess)
            {
                return PartialView("_List", result.Data);
            }
            else
            {
                return PartialView("_Error", result.Error);
            }
        }

        public ActionResult Detail(string solutionId)
        {
            RequestResult result = SolutionDataAccessor.GetDetailViewModel(solutionId, Session["Account"] as Account);

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
        public ActionResult Create(string organizationId, string solutionType)
        {
            RequestResult result = SolutionDataAccessor.GetCreateFormModel(organizationId, solutionType);

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
            return Content(JsonConvert.SerializeObject(SolutionDataAccessor.Create(createFormModel)));
        }

        [HttpGet]
        public ActionResult Edit(string solutionId)
        {
            RequestResult result = SolutionDataAccessor.GetEditFormModel(solutionId);

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
            return Content(JsonConvert.SerializeObject(SolutionDataAccessor.Edit(editFormModel)));
        }

        public ActionResult Delete(string selecteds)
        {
            RequestResult result = new RequestResult();

            try
            {
                var selectedList = JsonConvert.DeserializeObject<List<string>>(selecteds);

                result = SolutionDataAccessor.Delete(selectedList);
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return Content(JsonConvert.SerializeObject(result));
        }

        public ActionResult InitTree()
        {
            try
            {
                var organizationList = HttpRuntime.Cache.GetOrInsert<List<Models.Shared.Organization>>("Organizations", () => OrganizationDataAccessor.GetAllOrganizations());

                var account = Session["Account"] as Account;

                RequestResult result = new RequestResult();

                if (account.RootOrganizationId == new Guid())
                {
                    result = SolutionDataAccessor.GetTreeItems(organizationList, account.RootOrganizationId, "", Session["Account"] as Account);
                }
                else
                {
                    result = SolutionDataAccessor.GetRootTreeItems(organizationList, account.RootOrganizationId, Session["Account"] as Account);
                }

                if (result.IsSuccess)
                {
                    return PartialView("_Tree", JsonConvert.SerializeObject((List<TreeItem>)result.Data));
                }
                else
                {
                    return PartialView("_Error", result.Error);
                }
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                return PartialView("_Error", err);
            }
        }

        public ActionResult GetTreeItem(string organizationId, string solutionType)
        {
            string jsonTree = string.Empty;

            try
            {
                var organizationList = HttpRuntime.Cache.GetOrInsert<List<Models.Shared.Organization>>("Organizations", () => OrganizationDataAccessor.GetAllOrganizations());

                RequestResult result = SolutionDataAccessor.GetTreeItems(organizationList, new Guid(organizationId), solutionType, Session["Account"] as Account);

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