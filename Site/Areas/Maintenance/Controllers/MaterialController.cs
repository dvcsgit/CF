using DataAccessor;
using DataAccessor.Maintenance;
using Models.Authentication;
using Models.Maintenance.Material;
using Models.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Utility;
using Utility.Models;

namespace Site.Areas.Maintenance.Controllers
{
    public class MaterialController : Controller
    {
        // GET: Maintenance/Material
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

                if (account.RootOrganizationId == new Guid())
                {
                    result = MaterialDataAccessor.GetTreeItems(organizationList, account.RootOrganizationId, "", Session["Account"] as Account);
                }
                else
                {
                    result = MaterialDataAccessor.GetRootTreeItem(organizationList, account.RootOrganizationId, Session["Account"] as Account);
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

        public ActionResult GetTreeItems(string organizationId,string materialType)
        {
            string jsonTree = string.Empty;

            try
            {
                var organizationList = HttpRuntime.Cache.GetOrInsert<List<Models.Shared.Organization>>("Organizations", () => OrganizationDataAccessor.GetAllOrganizations());

                RequestResult result = MaterialDataAccessor.GetTreeItems(organizationList, new Guid(organizationId), materialType, Session["Account"] as Account);

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
            RequestResult result = MaterialDataAccessor.Query(queryParameters, Session["Account"] as Account);

            if (result.IsSuccess)
            {
                return PartialView("_List", result.Data);
            }
            else
            {
                return PartialView("_Error", result.Error);
            }
        }

        public ActionResult Detail(string materialId)
        {
            RequestResult requestResult = MaterialDataAccessor.GetDetailViewModel(materialId, Session["Account"] as Account);

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
        public ActionResult Create(string organizationId,string materialType)
        {
            RequestResult requestResult = MaterialDataAccessor.GetCreateFormModel(organizationId, materialType);

            if (requestResult.IsSuccess)
            {
                Session["MaterialFormAction"] = Define.EnumFormAction.Create;
                Session["MaterialCreateFormModel"] = requestResult.Data;

                return PartialView("_Create", requestResult.Data);
            }
            else
            {
                return PartialView("_Error", requestResult.Error);
            }
        }

        [HttpPost]
        public ActionResult Create(CreateFormModel createFormModel, string pageStates)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                var model = Session["MaterialCreateFormModel"] as CreateFormModel;

                var pageStateList = JsonConvert.DeserializeObject<List<string>>(pageStates);

                requestResult = MaterialDataAccessor.SavePageState(model.MaterialSpecificationModels, pageStateList);

                if (requestResult.IsSuccess)
                {
                    model.MaterialSpecificationModels = requestResult.Data as List<MaterialSpecificationModel>;

                    model.FormInput = createFormModel.FormInput;

                    requestResult = MaterialDataAccessor.Create(model);

                    if (requestResult.IsSuccess)
                    {
                        Session.Remove("MaterialFormAction");
                        Session.Remove("MaterialCreateFormModel");
                    }
                }
            }
            catch (Exception e)
            {
                var error = new Error(MethodBase.GetCurrentMethod(), e);

                Logger.Log(error);

                requestResult.ReturnError(error);
            }

            return Content(JsonConvert.SerializeObject(requestResult));
        }

        [HttpGet]
        public ActionResult Edit(string materialId)
        {
            RequestResult requestResult = MaterialDataAccessor.GetEditFormModel(new Guid(materialId));

            if (requestResult.IsSuccess)
            {
                Session["MaterialFormAction"] = Define.EnumFormAction.Edit;
                Session["MaterialEditFormModel"] = requestResult.Data;

                return PartialView("_Edit", requestResult.Data);
            }
            else
            {
                return PartialView("_Error", requestResult.Error);
            }
        }

        [HttpPost]
        public ActionResult Edit(EditFormModel editFormModel, string pageStates)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                var model = Session["MaterialEditFormModel"] as EditFormModel;

                var pageStateList = JsonConvert.DeserializeObject<List<string>>(pageStates);

                requestResult = MaterialDataAccessor.SavePageState(model.MaterialSpecificationModels, pageStateList);

                if (requestResult.IsSuccess)
                {
                    model.MaterialSpecificationModels = requestResult.Data as List<MaterialSpecificationModel>;

                    model.FormInput = editFormModel.FormInput;

                    requestResult = MaterialDataAccessor.Edit(model);

                    if (requestResult.IsSuccess)
                    {
                        Session.Remove("MaterialFormAction");
                        Session.Remove("MaterialEditFormModel");
                    }
                }
            }
            catch (Exception e)
            {
                var error = new Error(MethodBase.GetCurrentMethod(), e);

                Logger.Log(error);

                requestResult.ReturnError(error);
            }

            return Content(JsonConvert.SerializeObject(requestResult));
        }

        public ActionResult Delete(string selecteds)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                var selectedList = JsonConvert.DeserializeObject<List<string>>(selecteds);

                requestResult = MaterialDataAccessor.Delete(selectedList);
            }
            catch (Exception e)
            {
                var error = new Error(MethodBase.GetCurrentMethod(), e);

                Logger.Log(error);

                requestResult.ReturnError(error);
            }

            return Content(JsonConvert.SerializeObject(requestResult));
        }

        public ActionResult Copy(string materialId)
        {
            RequestResult requestResult = MaterialDataAccessor.GetCopyFormModel(new Guid(materialId));

            if (requestResult.IsSuccess)
            {
                Session["MaterialFormAction"] = Define.EnumFormAction.Create;
                Session["MaterialCreateFormModel"] = requestResult.Data;

                return PartialView("_Create", requestResult.Data);
            }
            else
            {
                return PartialView("_Error", requestResult.Error);
            }
        }

        public ActionResult GetSelectedList()
        {
            try
            {
                if ((Define.EnumFormAction)Session["MaterialFormAction"] == Define.EnumFormAction.Create)
                {
                    return PartialView("_SelectedList", (Session["MaterialCreateFormModel"] as CreateFormModel).MaterialSpecificationModels);
                }
                else if ((Define.EnumFormAction)Session["MaterialFormAction"] == Define.EnumFormAction.Edit)
                {
                    return PartialView("_SelectedList", (Session["MaterialEditFormModel"] as EditFormModel).MaterialSpecificationModels);
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

        public ActionResult InitSelectTree(string refOrganizationId)
        {
            try
            {
                var organizationList = HttpRuntime.Cache.GetOrInsert<List<Models.Shared.Organization>>("Organizations", () => OrganizationDataAccessor.GetAllOrganizations());

                RequestResult result = MSpecificationDataAccessor.GetTreeItems(organizationList, new Guid(refOrganizationId), new Guid(), "");

                if (result.IsSuccess)
                {
                    return PartialView("_SelectTree", JsonConvert.SerializeObject((List<TreeItem>)result.Data));
                }
                else
                {
                    return PartialView("_Error", result.Error);
                }
            }
            catch (Exception e)
            {
                var error = new Error(MethodBase.GetCurrentMethod(), e);

                Logger.Log(error);

                return PartialView("_Error", error);
            }
        }

        public ActionResult GetSelectTreeItems(string refOrganizationId, string organizationId, string materialType)
        {
            string jsonTree = string.Empty;

            try
            {
                var organizationList = HttpRuntime.Cache.GetOrInsert<List<Models.Shared.Organization>>("Organizations", () => OrganizationDataAccessor.GetAllOrganizations());

                RequestResult result = MSpecificationDataAccessor.GetTreeItems(organizationList, new Guid(refOrganizationId), new Guid(organizationId), materialType);

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

        public ActionResult AddSelect(string selecteds, string pageStates, string refOrganizationId)
        {
            RequestResult result = new RequestResult();

            try
            {
                List<string> selectedList = JsonConvert.DeserializeObject<List<string>>(selecteds);

                List<string> pageStateList = JsonConvert.DeserializeObject<List<string>>(pageStates);

                if ((Define.EnumFormAction)Session["MaterialFormAction"] == Define.EnumFormAction.Create)
                {
                    var model = Session["MaterialCreateFormModel"] as CreateFormModel;

                    result = MaterialDataAccessor.SavePageState(model.MaterialSpecificationModels, pageStateList);

                    if (result.IsSuccess)
                    {
                        model.MaterialSpecificationModels = result.Data as List<MaterialSpecificationModel>;

                        result = MaterialDataAccessor.AddMaterialSpecification(model.MaterialSpecificationModels, selectedList, refOrganizationId);

                        if (result.IsSuccess)
                        {
                            model.MaterialSpecificationModels = result.Data as List<MaterialSpecificationModel>;

                            Session["MaterialCreateFormModel"] = model;
                        }
                    }
                }
                else if ((Define.EnumFormAction)Session["MaterialFormAction"] == Define.EnumFormAction.Edit)
                {
                    var model = Session["MaterialEditFormModel"] as EditFormModel;

                    result = MaterialDataAccessor.SavePageState(model.MaterialSpecificationModels, pageStateList);

                    if (result.IsSuccess)
                    {
                        model.MaterialSpecificationModels = result.Data as List<MaterialSpecificationModel>;

                        result = MaterialDataAccessor.AddMaterialSpecification(model.MaterialSpecificationModels, selectedList, refOrganizationId);

                        if (result.IsSuccess)
                        {
                            model.MaterialSpecificationModels = result.Data as List<MaterialSpecificationModel>;

                            Session["MaterialEditFormModel"] = model;
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

        public ActionResult DeleteSelected(string materialSpecificationId, string pageStates)
        {
            RequestResult result = new RequestResult();

            try
            {
                List<string> pageStateList = JsonConvert.DeserializeObject<List<string>>(pageStates);

                if ((Define.EnumFormAction)Session["MaterialFormAction"] == Define.EnumFormAction.Create)
                {
                    var model = Session["MaterialCreateFormModel"] as CreateFormModel;

                    result = MaterialDataAccessor.SavePageState(model.MaterialSpecificationModels, pageStateList);

                    if (result.IsSuccess)
                    {
                        model.MaterialSpecificationModels = result.Data as List<MaterialSpecificationModel>;

                        model.MaterialSpecificationModels.Remove(model.MaterialSpecificationModels.First(x => x.MaterialSpecificationId == materialSpecificationId));

                        model.MaterialSpecificationModels = model.MaterialSpecificationModels.OrderBy(x => x.Seq).ToList();

                        int seq = 1;

                        foreach (var spec in model.MaterialSpecificationModels)
                        {
                            spec.Seq = seq;

                            seq++;
                        }

                        Session["MaterialCreateFormModel"] = model;

                        result.Success();
                    }
                }
                else if ((Define.EnumFormAction)Session["MaterialFormAction"] == Define.EnumFormAction.Edit)
                {
                    var model = Session["MaterialEditFormModel"] as EditFormModel;

                    result = MaterialDataAccessor.SavePageState(model.MaterialSpecificationModels, pageStateList);

                    if (result.IsSuccess)
                    {
                        model.MaterialSpecificationModels = result.Data as List<MaterialSpecificationModel>;

                        model.MaterialSpecificationModels.Remove(model.MaterialSpecificationModels.First(x => x.MaterialSpecificationId == materialSpecificationId));

                        model.MaterialSpecificationModels = model.MaterialSpecificationModels.OrderBy(x => x.Seq).ToList();

                        int seq = 1;

                        foreach (var spec in model.MaterialSpecificationModels)
                        {
                            spec.Seq = seq;

                            seq++;
                        }

                        Session["MaterialEditFormModel"] = model;

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


        protected IEnumerable<string> GetErrorsFromModelState()
        {
            return ModelState.SelectMany(x => x.Value.Errors.Select(error => error.ErrorMessage));
        }

        public ActionResult DeletePhoto(string UniqueID)
        {
            return Content(JsonConvert.SerializeObject(MaterialDataAccessor.DeletePhoto(UniqueID)));
        }

        [HttpPost]
        public ActionResult UploadPhoto(string UniqueID)
        {
            RequestResult result = new RequestResult();

            try
            {
                if (Request.Files != null && Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
                {
                    var photoName = Request.Files[0].FileName.Substring(Request.Files[0].FileName.LastIndexOf('\\') + 1);
                    var extension = photoName.Substring(photoName.LastIndexOf('.') + 1);

                    result = MaterialDataAccessor.UploadPhoto(UniqueID, extension);

                    if (result.IsSuccess)
                    {
                        Request.Files[0].SaveAs(Path.Combine(Config.MaintenancePhotoFolderPath, string.Format("{0}.{1}", UniqueID, extension)));
                    }

                    result.Success();
                }
                else
                {
                    result.ReturnFailedMessage(Resources.Resource.UploadFileRequired);
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