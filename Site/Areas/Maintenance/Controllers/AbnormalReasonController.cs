using DataAccessor;
using DataAccessor.Maintenance;
using Models.Authentication;
using Models.Maintenance.AbnormalReason;
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
    public class AbnormalReasonController : Controller
    {
        // GET: Maintenance/AbnormalReason
        public ActionResult Index()
        {
            return View(new QueryFormModel());
        }

        public ActionResult Query(QueryParameters queryParameters)
        {
            RequestResult result = AbnormalReasonDataAccessor.Query(queryParameters, Session["Account"] as Account);

            if (result.IsSuccess)
            {
                return PartialView("_List", result.Data);
            }
            else
            {
                return PartialView("_Error", result.Error);
            }
        }

        public ActionResult Detail(string abnormalReasonId)
        {
            RequestResult result = AbnormalReasonDataAccessor.GetDetailViewModel(abnormalReasonId, Session["Account"] as Account);

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
        public ActionResult Create(string organizationId, string abnormalType)
        {
            RequestResult result = AbnormalReasonDataAccessor.GetCreateFormModel(organizationId, abnormalType);

            if (result.IsSuccess)
            {
                Session["AbnormalReasonFormAction"] = Define.EnumFormAction.Create;
                Session["AbnormalReasonCreateFormModel"] = result.Data;

                return PartialView("_Create", result.Data);
            }
            else
            {
                return PartialView("_Error", result.Error);
            }
        }

        public ActionResult Copy(string abnormalReasonId)
        {
            RequestResult result = AbnormalReasonDataAccessor.GetCopyFormModel(abnormalReasonId);

            if (result.IsSuccess)
            {
                Session["AbnormalReasonFormAction"] = Define.EnumFormAction.Create;
                Session["AbnormalReasonCreateFormModel"] = result.Data;

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
            RequestResult result = new RequestResult();

            try
            {
                var model = Session["AbnormalReasonCreateFormModel"] as CreateFormModel;

                model.FormInput = createFormModel.FormInput;

                result = AbnormalReasonDataAccessor.Create(model);
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return Content(JsonConvert.SerializeObject(result));
        }

        [HttpGet]
        public ActionResult Edit(string abnormalReasonId)
        {
            RequestResult result = AbnormalReasonDataAccessor.GetEditFormModel(abnormalReasonId);

            if (result.IsSuccess)
            {
                Session["AbnormalReasonFormAction"] = Define.EnumFormAction.Edit;
                Session["AbnormalReasonEditFormModel"] = result.Data;

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
            RequestResult result = new RequestResult();

            try
            {
                var model = Session["AbnormalReasonEditFormModel"] as EditFormModel;

                model.FormInput = editFormModel.FormInput;

                result = AbnormalReasonDataAccessor.Edit(model);
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return Content(JsonConvert.SerializeObject(result));
        }

        public ActionResult Delete(string selecteds)
        {
            RequestResult result = new RequestResult();

            try
            {
                var selectedList = JsonConvert.DeserializeObject<List<string>>(selecteds);

                result = AbnormalReasonDataAccessor.Delete(selectedList);
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
                var organizations = HttpRuntime.Cache.GetOrInsert<List<Models.Shared.Organization>>("Organizations", () => OrganizationDataAccessor.GetAllOrganizations());

                var account = Session["Account"] as Account;

                RequestResult result = new RequestResult();

                if (account.RootOrganizationId ==new Guid())
                {
                    result = AbnormalReasonDataAccessor.GetTreeItems(organizations, account.RootOrganizationId, "", Session["Account"] as Account);
                }
                else
                {
                    result = AbnormalReasonDataAccessor.GetRootTreeItems(organizations, account.RootOrganizationId, Session["Account"] as Account);
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

        public ActionResult GetTreeItem(string organizationId, string type)
        {
            string jsonTree = string.Empty;

            try
            {
                var organizations = HttpRuntime.Cache.GetOrInsert<List<Models.Shared.Organization>>("Organizations", () => OrganizationDataAccessor.GetAllOrganizations());

                RequestResult result = AbnormalReasonDataAccessor.GetTreeItems(organizations, new Guid(organizationId), type, Session["Account"] as Account);

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

        public ActionResult InitSelectTree(string refOrganizationId)
        {
            try
            {
                var organizationList = HttpRuntime.Cache.GetOrInsert<List<Models.Shared.Organization>>("Organizations", () => OrganizationDataAccessor.GetAllOrganizations());

                RequestResult result = SolutionDataAccessor.GetTreeItems(organizationList, new Guid(refOrganizationId), new Guid(), "");

                if (result.IsSuccess)
                {
                    return PartialView("_SelectTree", JsonConvert.SerializeObject((List<TreeItem>)result.Data));
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

        public ActionResult GetSelectTreeItem(string refOrganizationId, string organizationId, string type)
        {
            string jsonTree = string.Empty;

            try
            {
                var organizations = HttpRuntime.Cache.GetOrInsert<List<Models.Shared.Organization>>("Organizations", () => OrganizationDataAccessor.GetAllOrganizations());

                RequestResult result = SolutionDataAccessor.GetTreeItems(organizations, new Guid(refOrganizationId), new Guid(organizationId), type);

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

        public ActionResult GetSelectedList()
        {
            try
            {
                if ((Define.EnumFormAction)Session["AbnormalReasonFormAction"] == Define.EnumFormAction.Create)
                {
                    return PartialView("_SelectedList", (Session["AbnormalReasonCreateFormModel"] as CreateFormModel).SolutionModels);
                }
                else if ((Define.EnumFormAction)Session["AbnormalReasonFormAction"] == Define.EnumFormAction.Edit)
                {
                    return PartialView("_SelectedList", (Session["AbnormalReasonEditFormModel"] as EditFormModel).SolutionModels);
                }
                else
                {
                    return PartialView("_Error", new Error(MethodBase.GetCurrentMethod(), Resources.Resource.UnKnownOperation));
                }
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                return PartialView("_Error", err);
            }
        }

        public ActionResult AddSelect(string selecteds, string refOrganizationId)
        {
            RequestResult result = new RequestResult();

            try
            {
                List<string> selectedList = JsonConvert.DeserializeObject<List<string>>(selecteds);

                if ((Define.EnumFormAction)Session["AbnormalReasonFormAction"] == Define.EnumFormAction.Create)
                {
                    var model = Session["AbnormalReasonCreateFormModel"] as CreateFormModel;

                    result = AbnormalReasonDataAccessor.AddSolution(model.SolutionModels, selectedList, new Guid(refOrganizationId));

                    if (result.IsSuccess)
                    {
                        model.SolutionModels = result.Data as List<SolutionModel>;

                        Session["AbnormalReasonCreateFormModel"] = model;
                    }
                }
                else if ((Define.EnumFormAction)Session["AbnormalReasonFormAction"] == Define.EnumFormAction.Edit)
                {
                    var model = Session["AbnormalReasonEditFormModel"] as EditFormModel;

                    result = AbnormalReasonDataAccessor.AddSolution(model.SolutionModels, selectedList, new Guid(refOrganizationId));

                    if (result.IsSuccess)
                    {
                        model.SolutionModels = result.Data as List<SolutionModel>;

                        Session["AbnormalReasonEditFormModel"] = model;
                    }
                }
                else
                {
                    result.ReturnFailedMessage(Resources.Resource.UnKnownOperation);
                }
            }
            catch (Exception ex)
            {
                Error err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return Content(JsonConvert.SerializeObject(result));
        }

        public ActionResult DeleteSelected(string solutionId)
        {
            RequestResult result = new RequestResult();

            try
            {
                if ((Define.EnumFormAction)Session["AbnormalReasonFormAction"] == Define.EnumFormAction.Create)
                {
                    var model = Session["AbnormalReasonCreateFormModel"] as CreateFormModel;

                    model.SolutionModels.Remove(model.SolutionModels.First(x => x.SolutionId == solutionId));

                    Session["AbnormalReasonCreateFormModel"] = model;

                    result.Success();
                }
                else if ((Define.EnumFormAction)Session["AbnormalReasonFormAction"] == Define.EnumFormAction.Edit)
                {
                    var model = Session["AbnormalReasonEditFormModel"] as EditFormModel;

                    model.SolutionModels.Remove(model.SolutionModels.First(x => x.SolutionId == solutionId));

                    Session["AbnormalReasonEditFormModel"] = model;

                    result.Success();
                }
                else
                {
                    result.ReturnFailedMessage(Resources.Resource.UnKnownOperation);
                }
            }
            catch (Exception ex)
            {
                Error err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return Content(JsonConvert.SerializeObject(result));
        }
    }
}