using DataAccessor;
using DataAccessor.Maintenance;
using Models.Authentication;
using Models.Maintenance.ESpecification;
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
    public class ESpecificationController : Controller
    {
        // GET: Maintenance/EquipmentSpecification
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

                RequestResult requestResult = new RequestResult();

                if (account.RootOrganizationId == new Guid())
                {
                    requestResult = ESpecificationDataAccessor.GetTreeItems(organizationList, account.RootOrganizationId, "", Session["Account"] as Account);
                }
                else
                {
                    requestResult = ESpecificationDataAccessor.GetRootTreeItems(organizationList, account.RootOrganizationId, Session["Account"] as Account);
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
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                return PartialView("_Error", err);
            }
        }

        public ActionResult GetTreeItem(string organizationId, string equipmentType)
        {
            string jsonTree = string.Empty;

            try
            {
                var organizationList = HttpRuntime.Cache.GetOrInsert<List<Models.Shared.Organization>>("Organizations", () => OrganizationDataAccessor.GetAllOrganizations());

                RequestResult result = ESpecificationDataAccessor.GetTreeItems(organizationList, new Guid(organizationId), equipmentType, Session["Account"] as Account);

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

        public ActionResult Query(QueryParameters queryParameters)
        {
            RequestResult result = ESpecificationDataAccessor.Query(queryParameters, Session["Account"] as Account);

            if (result.IsSuccess)
            {
                return PartialView("_List", result.Data);
            }
            else
            {
                return PartialView("_Error", result.Error);
            }
        }

        public ActionResult Detail(string equipmentSpecificationId)
        {
            RequestResult result = ESpecificationDataAccessor.GetDetailViewModel(equipmentSpecificationId, Session["Account"] as Account);

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
        public ActionResult Create(string organizationId, string equipmentType)
        {
            RequestResult result = ESpecificationDataAccessor.GetCreateFormModel(organizationId, equipmentType);

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
            return Content(JsonConvert.SerializeObject(ESpecificationDataAccessor.Create(createFormModel)));
        }

        [HttpGet]
        public ActionResult Edit(string equipmentSpecificationId)
        {
            RequestResult result = ESpecificationDataAccessor.GetEditFormModel(equipmentSpecificationId);

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
            return Content(JsonConvert.SerializeObject(ESpecificationDataAccessor.Edit(editFormModel)));
        }

        public ActionResult Copy(string equipmentSpecificationId)
        {
            RequestResult result = ESpecificationDataAccessor.GetCopyFormModel(equipmentSpecificationId);

            if (result.IsSuccess)
            {
                return PartialView("_Create", result.Data);
            }
            else
            {
                return PartialView("_Error", result.Error);
            }
        }

        public ActionResult Delete(string selecteds)
        {
            RequestResult result = new RequestResult();

            try
            {
                var selectedList = JsonConvert.DeserializeObject<List<string>>(selecteds);

                result = ESpecificationDataAccessor.Delete(selectedList);
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return Content(JsonConvert.SerializeObject(result));
        }
    }
}