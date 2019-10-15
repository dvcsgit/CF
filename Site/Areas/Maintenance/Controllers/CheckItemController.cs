using DataAccessor;
using DataAccessor.Maintenance;
using Models.Authentication;
using Models.Maintenance.CheckItem;
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
    public class CheckItemController : Controller
    {
        // GET: Maintenance/CheckItem
        public ActionResult Index()
        {
            return View(new QueryFormModel());
        }

        public ActionResult Query(QueryParameters queryParameters)
        {
            RequestResult result = CheckItemDataAccessor.Query(queryParameters, Session["Account"] as Account);

            if (result.IsSuccess)
            {
                return PartialView("_List", result.Data);
            }
            else
            {
                return PartialView("_Error", result.Error);
            }
        }

        public ActionResult Detail(string checkItemId)
        {
            RequestResult result = CheckItemDataAccessor.GetDetailViewModel(checkItemId, Session["Account"] as Account);

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
        public ActionResult Create(string organizationId, string type)
        {
            RequestResult result = CheckItemDataAccessor.GetCreateFormModel(organizationId, type);

            if (result.IsSuccess)
            {
                Session["CheckItemFormAction"] = Define.EnumFormAction.Create;
                Session["CheckItemCreateFormModel"] = result.Data;

                return PartialView("_Create", result.Data);
            }
            else
            {
                return PartialView("_Error", result.Error);
            }
        }

        public ActionResult Copy(string checkItemId)
        {
            RequestResult result = CheckItemDataAccessor.GetCopyFormModel(checkItemId);

            if (result.IsSuccess)
            {
                Session["CheckItemFormAction"] = Define.EnumFormAction.Create;
                Session["CheckItemCreateFormModel"] = result.Data;

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
                var model = Session["CheckItemCreateFormModel"] as CreateFormModel;

                model.FormInput = createFormModel.FormInput;

                result = CheckItemDataAccessor.Create(model);

                if (result.IsSuccess)
                {
                    Session.Remove("CheckItemFormAction");
                    Session.Remove("CheckItemCreateFormModel");
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

        [HttpGet]
        public ActionResult Edit(string checkItemId)
        {
            RequestResult result = CheckItemDataAccessor.GetEditFormModel(checkItemId);

            if (result.IsSuccess)
            {
                Session["CheckItemFormAction"] = Define.EnumFormAction.Edit;
                Session["CheckItemEditFormModel"] = result.Data;

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
                var model = Session["CheckItemEditFormModel"] as EditFormModel;

                model.FormInput = editFormModel.FormInput;

                result = CheckItemDataAccessor.Edit(model);

                if (result.IsSuccess)
                {
                    Session.Remove("CheckItemFormAction");
                    Session.Remove("CheckItemEditFormModel");
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

        public ActionResult Delete(string selecteds)
        {
            RequestResult result = new RequestResult();

            try
            {
                var selectedList = JsonConvert.DeserializeObject<List<string>>(selecteds);

                result = CheckItemDataAccessor.Delete(selectedList);
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

                if (account.RootOrganizationId == new Guid())
                {
                    result = CheckItemDataAccessor.GetTreeItems(organizations, account.RootOrganizationId, "", Session["Account"] as Account);
                }
                else
                {
                    result = CheckItemDataAccessor.GetRootTreeItems(organizations, account.RootOrganizationId, Session["Account"] as Account);
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

                RequestResult result = CheckItemDataAccessor.GetTreeItems(organizations, new Guid(organizationId), type, Session["Account"] as Account);

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
                var organizations = HttpRuntime.Cache.GetOrInsert<List<Models.Shared.Organization>>("Organizations", () => OrganizationDataAccessor.GetAllOrganizations());

                RequestResult result = AbnormalReasonDataAccessor.GetTreeItems(organizations, new Guid(refOrganizationId), new Guid(), "");

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

                RequestResult result = AbnormalReasonDataAccessor.GetTreeItems(organizations, new Guid(refOrganizationId), new Guid(organizationId), type);

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
                if ((Define.EnumFormAction)Session["CheckItemFormAction"] == Define.EnumFormAction.Create)
                {
                    return PartialView("_SelectedList", (Session["CheckItemCreateFormModel"] as CreateFormModel).AbnormalReasonModels);
                }
                else if ((Define.EnumFormAction)Session["CheckItemFormAction"] == Define.EnumFormAction.Edit)
                {
                    return PartialView("_SelectedList", (Session["CheckItemEditFormModel"] as EditFormModel).AbnormalReasonModels);
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

                if ((Define.EnumFormAction)Session["CheckItemFormAction"] == Define.EnumFormAction.Create)
                {
                    var model = Session["CheckItemCreateFormModel"] as CreateFormModel;

                    result = CheckItemDataAccessor.AddAbnormalReason(model.AbnormalReasonModels, selectedList, refOrganizationId);

                    if (result.IsSuccess)
                    {
                        model.AbnormalReasonModels = result.Data as List<AbnormalReasonModel>;

                        Session["CheckItemCreateFormModel"] = model;
                    }
                }
                else if ((Define.EnumFormAction)Session["CheckItemFormAction"] == Define.EnumFormAction.Edit)
                {
                    var model = Session["CheckItemEditFormModel"] as EditFormModel;

                    result = CheckItemDataAccessor.AddAbnormalReason(model.AbnormalReasonModels, selectedList, refOrganizationId);

                    if (result.IsSuccess)
                    {
                        model.AbnormalReasonModels = result.Data as List<AbnormalReasonModel>;

                        Session["CheckItemEditFormModel"] = model;
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

        public ActionResult DeleteSelected(string abnormalReasonId)
        {
            RequestResult result = new RequestResult();

            try
            {
                if ((Define.EnumFormAction)Session["CheckItemFormAction"] == Define.EnumFormAction.Create)
                {
                    var model = Session["CheckItemCreateFormModel"] as CreateFormModel;

                    model.AbnormalReasonModels.Remove(model.AbnormalReasonModels.First(x => x.AbnormalReasonId == abnormalReasonId));

                    Session["CheckItemCreateFormModel"] = model;

                    result.Success();
                }
                else if ((Define.EnumFormAction)Session["CheckItemFormAction"] == Define.EnumFormAction.Edit)
                {
                    var model = Session["CheckItemEditFormModel"] as EditFormModel;

                    model.AbnormalReasonModels.Remove(model.AbnormalReasonModels.First(x => x.AbnormalReasonId == abnormalReasonId));

                    Session["CheckItemEditFormModel"] = model;

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