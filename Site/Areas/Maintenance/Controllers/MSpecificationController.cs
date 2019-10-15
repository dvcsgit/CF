using DataAccessor;
using DataAccessor.Maintenance;
using Models.Authentication;
using Models.Maintenance.MSpecification;
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

namespace Site.Areas.Maintenance.Controllers
{
    public class MSpecificationController : Controller
    {
        // GET: Maintenance/MaterialSpecification
        public ActionResult Index()
        {
            return View(new QueryFormModel());
        }

        public ActionResult InitTree()
        {
            try
            {
                var organizationList = HttpRuntime.Cache.GetOrInsert<List<Models.Shared.Organization>>("Organizations", () => OrganizationDataAccessor.GetAllOrganizations());

                var account = Session["Account"] as Account;

                RequestResult result = new RequestResult();

                if (account.RootOrganizationId==new Guid())
                {
                    result = MSpecificationDataAccessor.GetTreeItems(organizationList, account.RootOrganizationId, "", Session["Account"] as Account);
                }
                else
                {
                    result = MSpecificationDataAccessor.GetRootTreeItem(organizationList, account.RootOrganizationId, Session["Account"] as Account);
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

        public ActionResult GetTreeItems(string organizationId, string materialType)
        {
            string jsonTree = string.Empty;

            try
            {
                var organizationList = HttpRuntime.Cache.GetOrInsert<List<Models.Shared.Organization>>("Organizations", () => OrganizationDataAccessor.GetAllOrganizations());

                RequestResult requestResult = MSpecificationDataAccessor.GetTreeItems(organizationList, new Guid(organizationId), materialType, Session["Account"] as Account);

                if (requestResult.IsSuccess)
                {
                    jsonTree = JsonConvert.SerializeObject((List<TreeItem>)requestResult.Data);
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

        public ActionResult Query(QueryParameters queryParameters)
        {
            RequestResult requestResult = MSpecificationDataAccessor.Query(queryParameters, Session["Account"] as Account);

            if (requestResult.IsSuccess)
            {
                return PartialView("_List", requestResult.Data);
            }
            else
            {
                return PartialView("_Error", requestResult.Error);
            }
        }

        public ActionResult Detail(string materialSpecificationId)
        {
            RequestResult requestResult = MSpecificationDataAccessor.GetDetailViewModel(materialSpecificationId, Session["Account"] as Account);

            if (requestResult.IsSuccess)
            {
                return PartialView("_Detail", requestResult.Data);
            }
            else
            {
                return PartialView("_Error", requestResult.Error);
            }
        }

        [HttpGet]
        public ActionResult Create(string organizationId, string materialType)
        {
            RequestResult requestResult = MSpecificationDataAccessor.GetCreateFormModel(organizationId, materialType);

            if (requestResult.IsSuccess)
            {
                return PartialView("_Create", requestResult.Data);
            }
            else
            {
                return PartialView("_Error", requestResult.Error);
            }
        }

        [HttpPost]
        public ActionResult Create(CreateFormModel createFormModel)
        {
            return Content(JsonConvert.SerializeObject(MSpecificationDataAccessor.Create(createFormModel)));
        }

        [HttpGet]
        public ActionResult Edit(string materialSpecificationId)
        {
            RequestResult requestResult = MSpecificationDataAccessor.GetEditFormModel(materialSpecificationId);

            if (requestResult.IsSuccess)
            {
                return PartialView("_Edit", requestResult.Data);
            }
            else
            {
                return PartialView("_Error", requestResult.Error);
            }
        }

        [HttpPost]
        public ActionResult Edit(EditFormModel editFormModel)
        {
            return Content(JsonConvert.SerializeObject(MSpecificationDataAccessor.Edit(editFormModel)));
        }

        public ActionResult Delete(string selecteds)
        {
            RequestResult result = new RequestResult();

            try
            {
                var selectedList = JsonConvert.DeserializeObject<List<string>>(selecteds);

                result = MSpecificationDataAccessor.Delete(selectedList);
            }
            catch (Exception e)
            {
                var error = new Error(MethodBase.GetCurrentMethod(), e);

                Logger.Log(error);

                result.ReturnError(error);
            }

            return Content(JsonConvert.SerializeObject(result));
        }

        public ActionResult Copy(string materialSpecificationId)
        {
            RequestResult requestResult = MSpecificationDataAccessor.GetCopyFormModel(materialSpecificationId);

            if (requestResult.IsSuccess)
            {
                return PartialView("_Create", requestResult.Data);
            }
            else
            {
                return PartialView("_Error", requestResult.Error);
            }
        }
    }
}