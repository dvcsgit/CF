using DataAccessor;
using DataAccessor.Maintenance;
using Models.Authentication;
using Models.Maintenance.Equipment;
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
    public class EquipmentController : Controller
    {
        // GET: Maintenance/Equipment
        public ActionResult Index()
        {
            return View(new QueryFormModel());
        }

        public ActionResult Query(QueryParameters queryParameters)
        {
            RequestResult result = EquipmentDataAccessor.Query(queryParameters, Session["Account"] as Account);

            if (result.IsSuccess)
            {
                Session["QueryResults"] = result.Data;
                return PartialView("_List", result.Data);
            }
            else
            {
                return PartialView("_Error", result.Error);
            }
        }

        public ActionResult Detail(string equipmentId)
        {
            RequestResult result = EquipmentDataAccessor.GetDetailViewModel(equipmentId, Session["Account"] as Account);
            ViewBag.EquipmentUniqueID = equipmentId;  //保存设备的UniqueID，当做下载时的参数使用
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
            try
            {
                var organization = OrganizationDataAccessor.GetOrganization(new Guid(organizationId));

                Session["EquipmentFormAction"] = Define.EnumFormAction.Create;
                Session["EquipmentCreateFormModel"] = new CreateFormModel()
                {
                    EquipmentId = Guid.NewGuid().ToString(),
                    AncestorOrganizationId = organization.AncestorOrganizationId.ToString(),
                    OrganizationId = organizationId.ToString(),
                    ParentOrganizationFullName = organization.FullName
                };

                return PartialView("_Create", Session["EquipmentCreateFormModel"]);
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                return PartialView("_Error", err);
            }
        }

        [HttpPost]
        public ActionResult Create(CreateFormModel createFormModel, string specPageStates, string materialPageStates, string partPageStates)
        {
            RequestResult result = new RequestResult();

            try
            {
                var model = Session["EquipmentCreateFormModel"] as CreateFormModel;

                var specPageStateList = JsonConvert.DeserializeObject<List<string>>(specPageStates);

                result = EquipmentDataAccessor.SavePageState(model.ESpecificationModels, specPageStateList);

                if (result.IsSuccess)
                {
                    model.ESpecificationModels = result.Data as List<ESpecificationModel>;

                    var materialPageStateList = JsonConvert.DeserializeObject<List<string>>(materialPageStates);

                    result = EquipmentDataAccessor.SavePageState(model.MaterialModels, materialPageStateList);

                    if (result.IsSuccess)
                    {
                        model.MaterialModels = result.Data as List<MaterialModel>;

                        var partPageStateList = JsonConvert.DeserializeObject<List<string>>(partPageStates);

                        result = EquipmentDataAccessor.SavePageState(model.EPartModels, partPageStateList);

                        if (result.IsSuccess)
                        {
                            model.EPartModels = result.Data as List<EPartModel>;

                            model.FormInput = createFormModel.FormInput;

                            result = EquipmentDataAccessor.Create(model);

                            if (result.IsSuccess)
                            {
                                Session.Remove("EquipmentFormAction");
                                Session.Remove("EquipmentCreateFormModel");
                            }
                        }
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

        public ActionResult Copy(string equipmentId)
        {
            RequestResult result = EquipmentDataAccessor.GetCopyFormModel(new Guid(equipmentId));

            if (result.IsSuccess)
            {
                Session["EquipmentFormAction"] = Define.EnumFormAction.Create;
                Session["EquipmentCreateFormModel"] = result.Data;

                return PartialView("_Create", result.Data);
            }
            else
            {
                return PartialView("_Error", result.Error);
            }
        }

        [HttpGet]
        public ActionResult Edit(string equipmentId)
        {
            RequestResult result = EquipmentDataAccessor.GetEditFormModel(equipmentId);

            if (result.IsSuccess)
            {
                Session["EquipmentFormAction"] = Define.EnumFormAction.Edit;
                Session["EquipmentEditFormModel"] = result.Data;

                return PartialView("_Edit", result.Data);
            }
            else
            {
                return PartialView("_Error", result.Error);
            }
        }

        [HttpPost]
        public ActionResult Edit(EditFormModel editFormModel, string specPageStates, string materialPageStates, string partPageStates)
        {
            RequestResult result = new RequestResult();

            try
            {
                var model = Session["EquipmentEditFormModel"] as EditFormModel;

                var specPageStateList = JsonConvert.DeserializeObject<List<string>>(specPageStates);

                result = EquipmentDataAccessor.SavePageState(model.ESpecificationModels, specPageStateList);

                if (result.IsSuccess)
                {
                    model.ESpecificationModels = result.Data as List<ESpecificationModel>;

                    var materialPageStateList = JsonConvert.DeserializeObject<List<string>>(materialPageStates);

                    result = EquipmentDataAccessor.SavePageState(model.MaterialModels, materialPageStateList);

                    if (result.IsSuccess)
                    {
                        model.MaterialModels = result.Data as List<MaterialModel>;

                        var partPageStateList = JsonConvert.DeserializeObject<List<string>>(partPageStates);

                        result = EquipmentDataAccessor.SavePageState(model.EPartModels, partPageStateList);

                        if (result.IsSuccess)
                        {
                            model.EPartModels = result.Data as List<EPartModel>;

                            model.FormInput = editFormModel.FormInput;

                            result = EquipmentDataAccessor.Edit(model);

                            if (result.IsSuccess)
                            {
                                Session.Remove("EquipmentFormAction");
                                Session.Remove("EquipmentEditFormModel");
                            }
                        }
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

                result = EquipmentDataAccessor.Delete(selectedList);
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
                    result = EquipmentDataAccessor.GetTreeItems(organizationList, account.RootOrganizationId, Session["Account"] as Account);
                }
                else
                {
                    result = EquipmentDataAccessor.GetRootTreeItems(organizationList, account.RootOrganizationId, Session["Account"] as Account);
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

        public ActionResult GetTreeItems(string organizationId)
        {
            string jsonTree = string.Empty;

            try
            {
                var organizationList = HttpRuntime.Cache.GetOrInsert<List<Models.Shared.Organization>>("Organizations", () => OrganizationDataAccessor.GetAllOrganizations());

                RequestResult result = EquipmentDataAccessor.GetTreeItems(organizationList, new Guid(organizationId), Session["Account"] as Account);

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

        public ActionResult InitMaintenanceOrganizationTree(string organizationId)
        {
            try
            {
                var organizationList = HttpRuntime.Cache.GetOrInsert<List<Models.Shared.Organization>>("Organizations", () => OrganizationDataAccessor.GetAllOrganizations());

                RequestResult result = OrganizationDataAccessor.GetRootTreeItem(organizationList, new Guid(organizationId), Session["Account"] as Account);

                if (result.IsSuccess)
                {
                    return PartialView("_MaintenanceOrganizationTree", JsonConvert.SerializeObject((List<TreeItem>)result.Data));
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

        public ActionResult GetMaintenanceOrganizationTreeItem(string organizationId)
        {
            string jsonTree = string.Empty;

            try
            {
                var organizationList = HttpRuntime.Cache.GetOrInsert<List<Models.Shared.Organization>>("Organizations", () => OrganizationDataAccessor.GetAllOrganizations());

                RequestResult result = OrganizationDataAccessor.GetTreeItem(organizationList, new Guid(organizationId), Session["Account"] as Account);

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
                jsonTree = string.Empty;

                Logger.Log(MethodBase.GetCurrentMethod(), ex);
            }

            return Content(jsonTree);
        }

        public ActionResult InitEquipmentSpecificationSelectTree(string refOrganizationId)
        {
            try
            {
                var organizationList = HttpRuntime.Cache.GetOrInsert<List<Models.Shared.Organization>>("Organizations", () => OrganizationDataAccessor.GetAllOrganizations());

                RequestResult result = ESpecificationDataAccessor.GetTreeItems(organizationList, new Guid(refOrganizationId), new Guid(), "");

                if (result.IsSuccess)
                {
                    return PartialView("_SpecSelectTree", JsonConvert.SerializeObject((List<TreeItem>)result.Data));
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

        public ActionResult GetEquipmentSpecificationSelectTreeItem(string refOrganizationId, string organizationId, string equipmentType)
        {
            string jsonTree = string.Empty;

            try
            {
                var organizationList = HttpRuntime.Cache.GetOrInsert<List<Models.Shared.Organization>>("Organizations", () => OrganizationDataAccessor.GetAllOrganizations());

                RequestResult result = ESpecificationDataAccessor.GetTreeItems(organizationList, new Guid(refOrganizationId), new Guid(organizationId), equipmentType);

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

        public ActionResult InitMaterialSelectTree(string refOrganizationId)
        {
            try
            {
                var organizationList = HttpRuntime.Cache.GetOrInsert<List<Models.Shared.Organization>>("Organizations", () => OrganizationDataAccessor.GetAllOrganizations());

                RequestResult result = MaterialDataAccessor.GetTreeItems(organizationList, new Guid(refOrganizationId), new Guid(), "");

                if (result.IsSuccess)
                {
                    return PartialView("_MaterialSelectTree", JsonConvert.SerializeObject((List<TreeItem>)result.Data));
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

        public ActionResult GetMaterialSelectTreeItem(string refOrganizationId, string organizationId, string equipmentType)
        {
            string jsonTree = string.Empty;

            try
            {
                var organizationList = HttpRuntime.Cache.GetOrInsert<List<Models.Shared.Organization>>("Organizations", () => OrganizationDataAccessor.GetAllOrganizations());

                RequestResult result = MaterialDataAccessor.GetTreeItems(organizationList, new Guid(refOrganizationId), new Guid(organizationId), equipmentType);

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

        public ActionResult InitPartMaterialSelectTree(string refOrganizationId)
        {
            try
            {
                var organizationList = HttpRuntime.Cache.GetOrInsert<List<Models.Shared.Organization>>("Organizations", () => OrganizationDataAccessor.GetAllOrganizations());

                RequestResult result = MaterialDataAccessor.GetTreeItems(organizationList, new Guid(refOrganizationId), new Guid(), "");

                if (result.IsSuccess)
                {
                    return PartialView("_PartMaterialSelectTree", JsonConvert.SerializeObject((List<TreeItem>)result.Data));
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

        public ActionResult GetSpecSelectedList()
        {
            try
            {
                if ((Define.EnumFormAction)Session["EquipmentFormAction"] == Define.EnumFormAction.Create)
                {
                    return PartialView("_SpecSelectedList", (Session["EquipmentCreateFormModel"] as CreateFormModel).ESpecificationModels);
                }
                else if ((Define.EnumFormAction)Session["EquipmentFormAction"] == Define.EnumFormAction.Edit)
                {
                    return PartialView("_SpecSelectedList", (Session["EquipmentEditFormModel"] as EditFormModel).ESpecificationModels);
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

        public ActionResult GetMaterialSelectedList()
        {
            try
            {
                if ((Define.EnumFormAction)Session["EquipmentFormAction"] == Define.EnumFormAction.Create)
                {
                    return PartialView("_MaterialSelectedList", (Session["EquipmentCreateFormModel"] as CreateFormModel).MaterialModels);
                }
                else if ((Define.EnumFormAction)Session["EquipmentFormAction"] == Define.EnumFormAction.Edit)
                {
                    return PartialView("_MaterialSelectedList", (Session["EquipmentEditFormModel"] as EditFormModel).MaterialModels);
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

        public ActionResult GetPartList()
        {
            try
            {
                if ((Define.EnumFormAction)Session["EquipmentFormAction"] == Define.EnumFormAction.Create)
                {
                    return PartialView("_PartList", (Session["EquipmentCreateFormModel"] as CreateFormModel).EPartModels);
                }
                else if ((Define.EnumFormAction)Session["EquipmentFormAction"] == Define.EnumFormAction.Edit)
                {
                    return PartialView("_PartList", (Session["EquipmentEditFormModel"] as EditFormModel).EPartModels);
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

        public ActionResult InitSpecSelectTree(string refOrganizationId)
        {
            try
            {
                var organizationList = HttpRuntime.Cache.GetOrInsert<List<Models.Shared.Organization>>("Organizations", () => OrganizationDataAccessor.GetAllOrganizations());

                RequestResult result = ESpecificationDataAccessor.GetTreeItems(organizationList, new Guid(refOrganizationId), new Guid(), "");

                if (result.IsSuccess)
                {
                    return PartialView("_SpecSelectTree", JsonConvert.SerializeObject((List<TreeItem>)result.Data));
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

        public ActionResult GetSpecSelectTreeItem(string refOrganizationId, string organizationId, string equipmentType)
        {
            string jsonTree = string.Empty;

            try
            {
                var organizationList = HttpRuntime.Cache.GetOrInsert<List<Models.Shared.Organization>>("Organizations", () => OrganizationDataAccessor.GetAllOrganizations());

                RequestResult result = ESpecificationDataAccessor.GetTreeItems(organizationList, new Guid(refOrganizationId), new Guid(organizationId), equipmentType);

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

        [HttpGet]
        public ActionResult CreatePart()
        {
            return PartialView("_CreatePart", new CreatePartFormModel());
        }

        [HttpPost]
        public ActionResult CreatePart(CreatePartFormModel createPartFormModel, string pageStates)
        {
            RequestResult result = new RequestResult();

            try
            {
                List<string> pageStateList = JsonConvert.DeserializeObject<List<string>>(pageStates);

                var ePartId = Guid.NewGuid();

                if ((Define.EnumFormAction)Session["EquipmentFormAction"] == Define.EnumFormAction.Create)
                {
                    var model = Session["EquipmentCreateFormModel"] as CreateFormModel;

                    result = EquipmentDataAccessor.SavePageState(model.EPartModels, pageStateList);

                    if (result.IsSuccess)
                    {
                        model.EPartModels = result.Data as List<EPartModel>;

                        var exists = model.EPartModels.FirstOrDefault(x => x.Name == createPartFormModel.FormInput.Name);

                        if (exists == null)
                        {
                            model.EPartModels.Add(new EPartModel()
                            {
                                EPartId = ePartId.ToString(),
                                Name = createPartFormModel.FormInput.Name
                            });

                            Session["EquipmentCreateFormModel"] = model;

                            result.ReturnData(ePartId);
                        }
                        else
                        {
                            result.ReturnFailedMessage(string.Format("{0} {1}", Resources.Resource.EquipmentPart, Resources.Resource.Exists));
                        }
                    }
                }
                else if ((Define.EnumFormAction)Session["EquipmentFormAction"] == Define.EnumFormAction.Edit)
                {
                    var model = Session["EquipmentEditFormModel"] as EditFormModel;

                    result = EquipmentDataAccessor.SavePageState(model.EPartModels, pageStateList);

                    if (result.IsSuccess)
                    {
                        model.EPartModels = result.Data as List<EPartModel>;

                        var exists = model.EPartModels.FirstOrDefault(x => x.Name == createPartFormModel.FormInput.Name);

                        if (exists == null)
                        {
                            model.EPartModels.Add(new EPartModel()
                            {
                                EPartId = ePartId.ToString(),
                                Name = createPartFormModel.FormInput.Name
                            });

                            Session["EquipmentEditFormModel"] = model;

                            result.ReturnData(ePartId);
                        }
                        else
                        {
                            result.ReturnFailedMessage(string.Format("{0} {1}", Resources.Resource.EquipmentPart, Resources.Resource.Exists));
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
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return Content(JsonConvert.SerializeObject(result));
        }

        [HttpGet]
        public ActionResult EditPart(string ePartId)
        {
            try
            {
                if ((Define.EnumFormAction)Session["EquipmentFormAction"] == Define.EnumFormAction.Create)
                {
                    var part = (Session["EquipmentCreateFormModel"] as CreateFormModel).EPartModels.First(x => x.EPartId == ePartId);

                    return PartialView("_EditPart", new EditPartFormModel()
                    {
                        EPartId = part.EPartId,
                        PartFormInput = new PartFormInput()
                        {
                            Name = part.Name
                        }
                    });
                }
                else if ((Define.EnumFormAction)Session["EquipmentFormAction"] == Define.EnumFormAction.Edit)
                {
                    var part = (Session["EquipmentEditFormModel"] as EditFormModel).EPartModels.First(x => x.EPartId == ePartId);

                    return PartialView("_EditPart", new EditPartFormModel()
                    {
                        EPartId = part.EPartId,
                        PartFormInput = new PartFormInput()
                        {
                            Name = part.Name
                        }
                    });
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

        [HttpPost]
        public ActionResult EditPart(EditPartFormModel editPartFormModel, string pageStates)
        {
            RequestResult result = new RequestResult();

            try
            {
                List<string> pageStateList = JsonConvert.DeserializeObject<List<string>>(pageStates);

                if ((Define.EnumFormAction)Session["EquipmentFormAction"] == Define.EnumFormAction.Create)
                {
                    var model = Session["EquipmentCreateFormModel"] as CreateFormModel;

                    result = EquipmentDataAccessor.SavePageState(model.EPartModels, pageStateList);

                    if (result.IsSuccess)
                    {
                        model.EPartModels = result.Data as List<EPartModel>;

                        var exists = model.EPartModels.FirstOrDefault(x => x.EPartId != editPartFormModel.EPartId && x.Name == editPartFormModel.PartFormInput.Name);

                        if (exists == null)
                        {
                            model.EPartModels.First(x => x.EPartId == editPartFormModel.EPartId).Name = editPartFormModel.PartFormInput.Name;

                            Session["EquipmentCreateFormModel"] = model;

                            result.Success();
                        }
                        else
                        {
                            result.ReturnFailedMessage(string.Format("{0} {1}", Resources.Resource.EquipmentPart, Resources.Resource.Exists));
                        }
                    }
                }
                else if ((Define.EnumFormAction)Session["EquipmentFormAction"] == Define.EnumFormAction.Edit)
                {
                    var model = Session["EquipmentEditFormModel"] as EditFormModel;

                    result = EquipmentDataAccessor.SavePageState(model.EPartModels, pageStateList);

                    if (result.IsSuccess)
                    {
                        model.EPartModels = result.Data as List<EPartModel>;

                        var exists = model.EPartModels.FirstOrDefault(x => x.EPartId != editPartFormModel.EPartId && x.Name == editPartFormModel.PartFormInput.Name);

                        if (exists == null)
                        {
                            model.EPartModels.First(x => x.EPartId == editPartFormModel.EPartId).Name = editPartFormModel.PartFormInput.Name;

                            Session["EquipmentEditFormModel"] = model;

                            result.Success();
                        }
                        else
                        {
                            result.ReturnFailedMessage(string.Format("{0} {1}", Resources.Resource.EquipmentPart, Resources.Resource.Exists));
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
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return Content(JsonConvert.SerializeObject(result));
        }

        public ActionResult DeletePart(string ePartId, string pageStates)
        {
            RequestResult result = new RequestResult();

            try
            {
                List<string> pageStateList = JsonConvert.DeserializeObject<List<string>>(pageStates);

                if ((Define.EnumFormAction)Session["EquipmentFormAction"] == Define.EnumFormAction.Create)
                {
                    var model = Session["EquipmentCreateFormModel"] as CreateFormModel;

                    result = EquipmentDataAccessor.SavePageState(model.EPartModels, pageStateList);

                    if (result.IsSuccess)
                    {
                        model.EPartModels = result.Data as List<EPartModel>;

                        model.EPartModels.Remove(model.EPartModels.First(x => x.EPartId == ePartId));

                        Session["EquipmentCreateFormModel"] = model;

                        result.Success();
                    }
                }
                else if ((Define.EnumFormAction)Session["EquipmentFormAction"] == Define.EnumFormAction.Edit)
                {
                    var model = Session["EquipmentEditFormModel"] as EditFormModel;

                    result = EquipmentDataAccessor.SavePageState(model.EPartModels, pageStateList);

                    if (result.IsSuccess)
                    {
                        model.EPartModels = result.Data as List<EPartModel>;

                        model.EPartModels.Remove(model.EPartModels.First(x => x.EPartId == ePartId));

                        Session["EquipmentEditFormModel"] = model;

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
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return Content(JsonConvert.SerializeObject(result));
        }

        public ActionResult AddSpec(string selecteds, string pageStates, string refOrganizationId)
        {
            RequestResult result = new RequestResult();

            try
            {
                List<string> selectedList = JsonConvert.DeserializeObject<List<string>>(selecteds);

                List<string> pageStateList = JsonConvert.DeserializeObject<List<string>>(pageStates);

                if ((Define.EnumFormAction)Session["EquipmentFormAction"] == Define.EnumFormAction.Create)
                {
                    var model = Session["EquipmentCreateFormModel"] as CreateFormModel;

                    result = EquipmentDataAccessor.SavePageState(model.ESpecificationModels, pageStateList);

                    if (result.IsSuccess)
                    {
                        model.ESpecificationModels = result.Data as List<ESpecificationModel>;

                        result = EquipmentDataAccessor.AddSpecification(model.ESpecificationModels, selectedList, refOrganizationId);

                        if (result.IsSuccess)
                        {
                            model.ESpecificationModels = result.Data as List<ESpecificationModel>;

                            Session["EquipmentCreateFormModel"] = model;
                        }
                    }
                }
                else if ((Define.EnumFormAction)Session["EquipmentFormAction"] == Define.EnumFormAction.Edit)
                {
                    var model = Session["EquipmentEditFormModel"] as EditFormModel;

                    result = EquipmentDataAccessor.SavePageState(model.ESpecificationModels, pageStateList);

                    if (result.IsSuccess)
                    {
                        model.ESpecificationModels = result.Data as List<ESpecificationModel>;

                        result = EquipmentDataAccessor.AddSpecification(model.ESpecificationModels, selectedList, refOrganizationId);

                        if (result.IsSuccess)
                        {
                            model.ESpecificationModels = result.Data as List<ESpecificationModel>;

                            Session["EquipmentEditFormModel"] = model;
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

        public ActionResult DeleteSpec(string eSpecificationId, string pageStates)
        {
            RequestResult result = new RequestResult();

            try
            {
                List<string> pageStateList = JsonConvert.DeserializeObject<List<string>>(pageStates);

                if ((Define.EnumFormAction)Session["EquipmentFormAction"] == Define.EnumFormAction.Create)
                {
                    var model = Session["EquipmentCreateFormModel"] as CreateFormModel;

                    result = EquipmentDataAccessor.SavePageState(model.ESpecificationModels, pageStateList);

                    if (result.IsSuccess)
                    {
                        model.ESpecificationModels = result.Data as List<ESpecificationModel>;

                        model.ESpecificationModels.Remove(model.ESpecificationModels.First(x => x.ESpecificationId == eSpecificationId));

                        model.ESpecificationModels = model.ESpecificationModels.OrderBy(x => x.Seq).ToList();

                        int seq = 1;

                        foreach (var spec in model.ESpecificationModels)
                        {
                            spec.Seq = seq;

                            seq++;
                        }

                        Session["EquipmentCreateFormModel"] = model;

                        result.Success();
                    }
                }
                else if ((Define.EnumFormAction)Session["EquipmentFormAction"] == Define.EnumFormAction.Edit)
                {
                    var model = Session["EquipmentEditFormModel"] as EditFormModel;

                    result = EquipmentDataAccessor.SavePageState(model.ESpecificationModels, pageStateList);

                    if (result.IsSuccess)
                    {
                        model.ESpecificationModels = result.Data as List<ESpecificationModel>;

                        model.ESpecificationModels.Remove(model.ESpecificationModels.First(x => x.ESpecificationId == eSpecificationId));

                        model.ESpecificationModels = model.ESpecificationModels.OrderBy(x => x.Seq).ToList();

                        int seq = 1;

                        foreach (var spec in model.ESpecificationModels)
                        {
                            spec.Seq = seq;

                            seq++;
                        }

                        Session["EquipmentEditFormModel"] = model;

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

        public ActionResult AddMaterial(string selecteds, string pageStates, string refOrganizationId)
        {
            RequestResult result = new RequestResult();

            try
            {
                List<string> selectedList = JsonConvert.DeserializeObject<List<string>>(selecteds);

                List<string> pageStateList = JsonConvert.DeserializeObject<List<string>>(pageStates);

                if ((Define.EnumFormAction)Session["EquipmentFormAction"] == Define.EnumFormAction.Create)
                {
                    var model = Session["EquipmentCreateFormModel"] as CreateFormModel;

                    result = EquipmentDataAccessor.SavePageState(model.MaterialModels, pageStateList);

                    if (result.IsSuccess)
                    {
                        model.MaterialModels = result.Data as List<MaterialModel>;

                        result = EquipmentDataAccessor.AddMaterial(model.MaterialModels, selectedList, refOrganizationId);

                        if (result.IsSuccess)
                        {
                            model.MaterialModels = result.Data as List<MaterialModel>;

                            Session["EquipmentCreateFormModel"] = model;
                        }
                    }
                }
                else if ((Define.EnumFormAction)Session["EquipmentFormAction"] == Define.EnumFormAction.Edit)
                {
                    var model = Session["EquipmentEditFormModel"] as EditFormModel;

                    result = EquipmentDataAccessor.SavePageState(model.MaterialModels, pageStateList);

                    if (result.IsSuccess)
                    {
                        model.MaterialModels = result.Data as List<MaterialModel>;

                        result = EquipmentDataAccessor.AddMaterial(model.MaterialModels, selectedList, refOrganizationId);

                        if (result.IsSuccess)
                        {
                            model.MaterialModels = result.Data as List<MaterialModel>;

                            Session["EquipmentEditFormModel"] = model;
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

        public ActionResult DeleteMaterial(string materialId, string pageStates)
        {
            RequestResult result = new RequestResult();

            try
            {
                List<string> pageStateList = JsonConvert.DeserializeObject<List<string>>(pageStates);

                if ((Define.EnumFormAction)Session["EquipmentFormAction"] == Define.EnumFormAction.Create)
                {
                    var model = Session["EquipmentCreateFormModel"] as CreateFormModel;

                    result = EquipmentDataAccessor.SavePageState(model.MaterialModels, pageStateList);

                    if (result.IsSuccess)
                    {
                        model.MaterialModels = result.Data as List<MaterialModel>;

                        model.MaterialModels.Remove(model.MaterialModels.First(x => x.MaterialId == materialId));

                        Session["EquipmentCreateFormModel"] = model;

                        result.Success();
                    }
                }
                else if ((Define.EnumFormAction)Session["EquipmentFormAction"] == Define.EnumFormAction.Edit)
                {
                    var model = Session["EquipmentEditFormModel"] as EditFormModel;

                    result = EquipmentDataAccessor.SavePageState(model.MaterialModels, pageStateList);

                    if (result.IsSuccess)
                    {
                        model.MaterialModels = result.Data as List<MaterialModel>;

                        model.MaterialModels.Remove(model.MaterialModels.First(x => x.MaterialId == materialId));

                        Session["EquipmentEditFormModel"] = model;

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

        public ActionResult AddPartMaterial(string ePartId, string selecteds, string pageStates, string refOrganizationId)
        {
            RequestResult result = new RequestResult();

            try
            {
                List<string> selectedList = JsonConvert.DeserializeObject<List<string>>(selecteds);

                List<string> pageStateList = JsonConvert.DeserializeObject<List<string>>(pageStates);

                if ((Define.EnumFormAction)Session["EquipmentFormAction"] == Define.EnumFormAction.Create)
                {
                    var model = Session["EquipmentCreateFormModel"] as CreateFormModel;

                    result = EquipmentDataAccessor.SavePageState(model.EPartModels, pageStateList);

                    if (result.IsSuccess)
                    {
                        model.EPartModels = result.Data as List<EPartModel>;

                        var part = model.EPartModels.First(x => x.EPartId == ePartId);

                        result = EquipmentDataAccessor.AddMaterial(part.MaterialModels, selectedList, refOrganizationId);

                        if (result.IsSuccess)
                        {
                            part.MaterialModels = result.Data as List<MaterialModel>;

                            Session["EquipmentCreateFormModel"] = model;
                        }
                    }
                }
                else if ((Define.EnumFormAction)Session["EquipmentFormAction"] == Define.EnumFormAction.Edit)
                {
                    var model = Session["EquipmentEditFormModel"] as EditFormModel;

                    result = EquipmentDataAccessor.SavePageState(model.EPartModels, pageStateList);

                    if (result.IsSuccess)
                    {
                        model.EPartModels = result.Data as List<EPartModel>;

                        var part = model.EPartModels.First(x => x.EPartId == ePartId);

                        result = EquipmentDataAccessor.AddMaterial(part.MaterialModels, selectedList, refOrganizationId);

                        if (result.IsSuccess)
                        {
                            part.MaterialModels = result.Data as List<MaterialModel>;

                            Session["EquipmentEditFormModel"] = model;
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

        public ActionResult DeletePartMaterial(string ePartId, string materialId, string pageStates)
        {
            RequestResult result = new RequestResult();

            try
            {
                List<string> pageStateList = JsonConvert.DeserializeObject<List<string>>(pageStates);

                if ((Define.EnumFormAction)Session["EquipmentFormAction"] == Define.EnumFormAction.Create)
                {
                    var model = Session["EquipmentCreateFormModel"] as CreateFormModel;

                    result = EquipmentDataAccessor.SavePageState(model.EPartModels, pageStateList);

                    if (result.IsSuccess)
                    {
                        model.EPartModels = result.Data as List<EPartModel>;

                        var part = model.EPartModels.First(x => x.EPartId == ePartId);

                        part.MaterialModels.Remove(part.MaterialModels.First(x => x.MaterialId == materialId));

                        Session["EquipmentCreateFormModel"] = model;

                        result.Success();
                    }
                }
                else if ((Define.EnumFormAction)Session["EquipmentFormAction"] == Define.EnumFormAction.Edit)
                {
                    var model = Session["EquipmentEditFormModel"] as EditFormModel;

                    result = EquipmentDataAccessor.SavePageState(model.EPartModels, pageStateList);

                    if (result.IsSuccess)
                    {
                        model.EPartModels = result.Data as List<EPartModel>;

                        var part = model.EPartModels.First(x => x.EPartId == ePartId);

                        part.MaterialModels.Remove(part.MaterialModels.First(x => x.MaterialId == materialId));

                        Session["EquipmentEditFormModel"] = model;

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

        //        public ActionResult Export(Report.EquipmentMaintenance.Models.EquipmentDetail.QueryParameters Parameters)
        //        {
        //            var model = EquipmentDetailDataAccessor.Export(Parameters) as ExcelExportModel;

        //            return File(model.Data, model.ContentType, model.FileName);
        //        }

        //        public ActionResult ExportEquipment(Define.EnumExcelVersion ExcelVersion)
        //        {
        //            var excel = EquipmentDataAccessor.Export(Session["QueryResults"] as GridViewModel, ExcelVersion);

        //            return File(excel.Data, excel.ContentType, excel.FileName);
        //        }

        //        [HttpPost]
        //        public ActionResult UploadPhoto(string UniqueID)
        //        {
        //            RequestResult result = new RequestResult();

        //            try
        //            {
        //                if (Request.Files != null && Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
        //                {
        //                    var photoName = Request.Files[0].FileName.Substring(Request.Files[0].FileName.LastIndexOf('\\') + 1);
        //                    var extension = photoName.Substring(photoName.LastIndexOf('.') + 1);

        //                    result = EquipmentDataAccessor.UploadPhoto(UniqueID, extension);

        //                    if (result.IsSuccess)
        //                    {
        //                        Request.Files[0].SaveAs(Path.Combine(Config.EquipmentMaintenancePhotoFolderPath, string.Format("{0}.{1}", UniqueID, extension)));
        //                    }

        //                    result.Success();
        //                }
        //                else
        //                {
        //                    result.ReturnFailedMessage(Resources.Resource.UploadFileRequired);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                Error err = new Error(MethodBase.GetCurrentMethod(), ex);

        //                Logger.Log(err);

        //                result.ReturnError(err);
        //            }

        //            return Content(JsonConvert.SerializeObject(result));
        //        }

        //        public ActionResult DeletePhoto(string UniqueID)
        //        {
        //            return Content(JsonConvert.SerializeObject(EquipmentDataAccessor.DeletePhoto(UniqueID)));
        //        }

        //#if ASE
        //        public ActionResult ExportQRCode(string Selecteds)
        //        {

        //            try
        //            {
        //                var userList = HttpRuntime.Cache.GetOrInsert<List<Models.Shared.UserModel>>("Users", () => AccountDataAccessor.GetAllUser());

        //                var selectedList = JsonConvert.DeserializeObject<List<string>>(Selecteds);

        //                var fileName = "QRCODE_" + Guid.NewGuid().ToString() + ".xlsx";

        //                var model = EquipmentDataAccessor.ExportQRCode(userList, selectedList, Session["Account"] as Account, Define.EnumExcelVersion._2007, fileName) as RequestResult;

        //                if (model.IsSuccess)
        //                {
        //                    var guidFileName = model.Data as string;
        //                    var desFileName = "設備資料.xlsx";//depart.Name + "_" + currentDate + ".xlsx";

        //                    var tempPath = Url.Action("DownloadFile", "Utils", new { area = "Customized_ASE_QA", guidFileName = guidFileName, desFileName = desFileName });
        //                    return Json(new { success = true, data = tempPath });
        //                }
        //                else
        //                {
        //                    ModelState.AddModelError("", model.Message);
        //                }


        //            }
        //            catch (Exception ex)
        //            {

        //                ModelState.AddModelError("", ex.Message);
        //            }

        //            return Json(new { errors = GetErrorsFromModelState() });
        //        }
        //#endif
        //        protected IEnumerable<string> GetErrorsFromModelState()
        //        {
        //            return ModelState.SelectMany(x => x.Value.Errors.Select(error => error.ErrorMessage));
        //        }

        //#if !ASE
        //        public ActionResult Move(string OrganizationUniqueID, string Selecteds)
        //        {
        //            RequestResult result = new RequestResult();

        //            try
        //            {
        //                var selectedList = JsonConvert.DeserializeObject<List<string>>(Selecteds);

        //                result = EquipmentDataAccessor.Move(OrganizationUniqueID, selectedList);
        //            }
        //            catch (Exception ex)
        //            {
        //                var err = new Error(MethodBase.GetCurrentMethod(), ex);

        //                Logger.Log(err);

        //                result.ReturnError(err);
        //            }

        //            return Content(JsonConvert.SerializeObject(result));
        //        }
        //#endif
    }
}