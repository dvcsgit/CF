using DataAccessor;
using DataAccessor.Maintenance;
using Models.Authentication;
using Models.Maintenance.Checkpoint;
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
    public class CheckpointController : Controller
    {
        // GET: Maintenance/Checkpoint
        public ActionResult Index()
        {
            return View(new QueryFormModel());
        }

        public ActionResult Query(QueryParameters queryParameters)
        {
            RequestResult result = CheckpointDataAccessor.Query(queryParameters, Session["Account"] as Account);

            if (result.IsSuccess)
            {
                return PartialView("_List", result.Data);
            }
            else
            {
                return PartialView("_Error", result.Error);
            }
        }

        public ActionResult Detail(string checkpointId)
        {
            RequestResult result = CheckpointDataAccessor.GetDetailViewModel(checkpointId, Session["Account"] as Account);

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
        public ActionResult Create(string orgnaizationId)
        {
            Session["ControlPointFormAction"] = Define.EnumFormAction.Create;
            Session["ControlPointCreateFormModel"] = new CreateFormModel()
            {
                OrganizationId = orgnaizationId,
                ParentOrganizationFullName = OrganizationDataAccessor.GetOrganizationFullName(new Guid(orgnaizationId))
            };

            return PartialView("_Create", Session["ControlPointCreateFormModel"]);
        }

        public ActionResult Copy(string checkpointId)
        {
            RequestResult result = CheckpointDataAccessor.GetCopyFormModel(checkpointId);

            if (result.IsSuccess)
            {
                Session["ControlPointFormAction"] = Define.EnumFormAction.Create;
                Session["ControlPointCreateFormModel"] = result.Data;

                return PartialView("_Create", result.Data);
            }
            else
            {
                return PartialView("_Error", result.Error);
            }
        }

        [HttpPost]
        public ActionResult Create(CreateFormModel createFormModel, string pageStates)
        {
            RequestResult result = new RequestResult();

            try
            {
                var model = Session["ControlPointCreateFormModel"] as CreateFormModel;

                var pageStateList = JsonConvert.DeserializeObject<List<string>>(pageStates);

                result = CheckpointDataAccessor.SavePageState(model.CheckItemModels, pageStateList);

                if (result.IsSuccess)
                {
                    model.FormInput = createFormModel.FormInput;
                    model.CheckItemModels = result.Data as List<CheckItemModel>;

                    result = CheckpointDataAccessor.Create(model);

                    if (result.IsSuccess)
                    {
                        Session.Remove("ControlPointFormAction");
                        Session.Remove("ControlPointCreateFormModel");
                    }
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
        public ActionResult Edit(string checkpointId)
        {
            RequestResult result = CheckpointDataAccessor.GetEditFormModel(checkpointId);

            if (result.IsSuccess)
            {
                Session["ControlPointFormAction"] = Define.EnumFormAction.Edit;
                Session["ControlPointEditFormModel"] = result.Data;

                return PartialView("_Edit", result.Data);
            }
            else
            {
                return PartialView("_Error", result.Error);
            }
        }

        [HttpPost]
        public ActionResult Edit(EditFormModel editFormModel, string pageStates)
        {
            RequestResult result = new RequestResult();

            try
            {
                var model = Session["ControlPointEditFormModel"] as EditFormModel;

                var pageStateList = JsonConvert.DeserializeObject<List<string>>(pageStates);

                result = CheckpointDataAccessor.SavePageState(model.CheckItemModels, pageStateList);

                if (result.IsSuccess)
                {
                    model.FormInput = editFormModel.FormInput;
                    model.CheckItemModels = result.Data as List<CheckItemModel>;

                    result = CheckpointDataAccessor.Edit(model);

                    if (result.IsSuccess)
                    {
                        Session.Remove("ControlPointFormAction");
                        Session.Remove("ControlPointEditFormModel");
                    }
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

                result = CheckpointDataAccessor.Delete(selectedList);
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
                    result = CheckpointDataAccessor.GetTreeItems(organizations, account.RootOrganizationId, Session["Account"] as Account);
                }
                else
                {
                    result = CheckpointDataAccessor.GetRootTreeItems(organizations, account.RootOrganizationId, Session["Account"] as Account);
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

        public ActionResult GetTreeItem(string organizationId)
        {
            string jsonTree = string.Empty;

            try
            {
                var organizations = HttpRuntime.Cache.GetOrInsert<List<Models.Shared.Organization>>("Organizations", () => OrganizationDataAccessor.GetAllOrganizations());

                RequestResult result = CheckpointDataAccessor.GetTreeItems(organizations, new Guid(organizationId), Session["Account"] as Account);

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

                RequestResult result = CheckItemDataAccessor.GetTreeItems(organizations, new Guid(refOrganizationId), new Guid(), "");

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

                RequestResult result = CheckItemDataAccessor.GetTreeItems(organizations, new Guid(refOrganizationId), new Guid(organizationId), type);

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
                if ((Define.EnumFormAction)Session["ControlPointFormAction"] == Define.EnumFormAction.Create)
                {
                    return PartialView("_SelectedList", (Session["ControlPointCreateFormModel"] as CreateFormModel).CheckItemModels);
                }
                else if ((Define.EnumFormAction)Session["ControlPointFormAction"] == Define.EnumFormAction.Edit)
                {
                    return PartialView("_SelectedList", (Session["ControlPointEditFormModel"] as EditFormModel).CheckItemModels);
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

        public ActionResult AddSelect(string selecteds, string pageStates, string refOrganizationId)
        {
            RequestResult result = new RequestResult();

            try
            {
                List<string> selectedList = JsonConvert.DeserializeObject<List<string>>(selecteds);

                List<string> pageStateList = JsonConvert.DeserializeObject<List<string>>(pageStates);

                if ((Define.EnumFormAction)Session["ControlPointFormAction"] == Define.EnumFormAction.Create)
                {
                    var model = Session["ControlPointCreateFormModel"] as CreateFormModel;

                    result = CheckpointDataAccessor.SavePageState(model.CheckItemModels, pageStateList);

                    if (result.IsSuccess)
                    {
                        model.CheckItemModels = result.Data as List<CheckItemModel>;

                        result = CheckpointDataAccessor.AddCheckItem(model.CheckItemModels, selectedList, refOrganizationId);

                        if (result.IsSuccess)
                        {
                            model.CheckItemModels = result.Data as List<CheckItemModel>;

                            Session["ControlPointCreateFormModel"] = model;
                        }
                    }
                }
                else if ((Define.EnumFormAction)Session["ControlPointFormAction"] == Define.EnumFormAction.Edit)
                {
                    var model = Session["ControlPointEditFormModel"] as EditFormModel;

                    result = CheckpointDataAccessor.SavePageState(model.CheckItemModels, pageStateList);

                    if (result.IsSuccess)
                    {
                        model.CheckItemModels = result.Data as List<CheckItemModel>;

                        result = CheckpointDataAccessor.AddCheckItem(model.CheckItemModels, selectedList, refOrganizationId);

                        if (result.IsSuccess)
                        {
                            model.CheckItemModels = result.Data as List<CheckItemModel>;

                            Session["ControlPointEditFormModel"] = model;
                        }
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

        public ActionResult DeleteSelected(string checkItemId, string pageStates)
        {
            RequestResult result = new RequestResult();

            try
            {
                List<string> pageStateList = JsonConvert.DeserializeObject<List<string>>(pageStates);

                if ((Define.EnumFormAction)Session["ControlPointFormAction"] == Define.EnumFormAction.Create)
                {
                    var model = Session["ControlPointCreateFormModel"] as CreateFormModel;

                    result = CheckpointDataAccessor.SavePageState(model.CheckItemModels, pageStateList);

                    if (result.IsSuccess)
                    {
                        model.CheckItemModels = result.Data as List<CheckItemModel>;

                        model.CheckItemModels.Remove(model.CheckItemModels.First(x => x.CheckItemId == checkItemId));

                        Session["ControlPointCreateFormModel"] = model;

                        result.Success();
                    }
                }
                else if ((Define.EnumFormAction)Session["ControlPointFormAction"] == Define.EnumFormAction.Edit)
                {
                    var model = Session["ControlPointEditFormModel"] as EditFormModel;

                    result = CheckpointDataAccessor.SavePageState(model.CheckItemModels, pageStateList);

                    if (result.IsSuccess)
                    {
                        model.CheckItemModels = result.Data as List<CheckItemModel>;

                        model.CheckItemModels.Remove(model.CheckItemModels.First(x => x.CheckItemId == checkItemId));

                        Session["ControlPointEditFormModel"] = model;

                        result.Success();
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

#if ASE
        public ActionResult ExportQRCode(string Selecteds)
        {

            try
            {
                var selectedList = JsonConvert.DeserializeObject<List<string>>(Selecteds);

                var fileName = "QRCODE_" + Guid.NewGuid().ToString() + ".xlsx";

                var model = ControlPointDataAccessor.ExportQRCode(selectedList, Session["Account"] as Account, Define.EnumExcelVersion._2007, fileName) as RequestResult;

                if (model.IsSuccess)
                {
                    var guidFileName = model.Data as string;
                    var desFileName = "管制點資料.xlsx";//depart.Name + "_" + currentDate + ".xlsx";

                    var tempPath = Url.Action("DownloadFile", "Utils", new { area = "Customized_ASE_QA", guidFileName = guidFileName, desFileName = desFileName });
                    return Json(new { success = true, data = tempPath });
                }
                else
                {
                    ModelState.AddModelError("", model.Message);
                }


            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", ex.Message);
            }

            return Json(new { errors = GetErrorsFromModelState() });
        }
#endif
        protected IEnumerable<string> GetErrorsFromModelState()
        {
            return ModelState.SelectMany(x => x.Value.Errors.Select(error => error.ErrorMessage));
        }
    }
}