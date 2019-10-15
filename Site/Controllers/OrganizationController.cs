using DataAccessor;
using Models.Authentication;
using Models.Organization;
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
    public class OrganizationController : Controller
    {
        // GET: Organization
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Create(string parentOrganizationId)
        {
            try
            {
                if (parentOrganizationId == "00000000-0000-0000-0000-000000000000")
                {
                    Session["OrganizationCreateFormModel"] = new CreateFormModel()
                    {
                        AncestorOrganizationId = "00000000-0000-0000-0000-000000000000",
                        ParentId = "00000000-0000-0000-0000-000000000000",
                        ParentOrganizationFullName = "*"
                    };
                }
                else
                {
                    var parentOrganization = OrganizationDataAccessor.GetOrganization(new Guid(parentOrganizationId));

                    Session["OrganizationCreateFormModel"] = new CreateFormModel()
                    {
                        AncestorOrganizationId = parentOrganization.AncestorOrganizationId.ToString(),
                        ParentId = parentOrganization.OrganizationId.ToString(),
                        ParentOrganizationFullName = parentOrganization.FullName
                    };
                }

                Session["OrganizationFormAction"] = Define.EnumFormAction.Create;

                return PartialView("_Create", Session["OrganizationCreateFormModel"]);
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                return PartialView("_Error", err);
            }
        }

        [HttpPost]
        public ActionResult Create(CreateFormModel createFormModel)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                var model = Session["OrganizationCreateFormModel"] as CreateFormModel;

                model.FormInput = createFormModel.FormInput;

                requestResult = OrganizationDataAccessor.Create(model);

                if (requestResult.IsSuccess)
                {
                    HttpRuntime.Cache.Remove("Organizations");

                    Session.Remove("OrganizationFormAction");
                    Session.Remove("OrganizationCreateFormModel");
                    var account = Session["Account"] as Account;
                    account.OrganizationPermissions = OrganizationDataAccessor.GetOrganizationPermissions(account.OrganizationId);
                    Session["Account"] = account;
                }
            }
            catch (Exception e)
            {
                var error = new Error(MethodBase.GetCurrentMethod(), e);
                Logger.Log(error);
                requestResult.ReturnError(error);
                //throw;
            }

            return Content(JsonConvert.SerializeObject(requestResult));
        }

        public ActionResult Detail(string organizationId)
        {
            try
            {
                RequestResult requestResult = OrganizationDataAccessor.GetDetailViewModel(new Guid(organizationId), Session["Account"] as Account);

                if (requestResult.IsSuccess)
                {
                    return PartialView("_Detail", requestResult.Data);
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
            }
        }

        [HttpGet]
        public ActionResult Edit(string organizationId)
        {
            try
            {
                RequestResult requestResult = new RequestResult();
                requestResult = OrganizationDataAccessor.GetEditFormModel(new Guid(organizationId));

                if (requestResult.IsSuccess)
                {
                    Session["OrganizationFormAction"] = Define.EnumFormAction.Edit;
                    Session["OrganizationEditFormModel"] = requestResult.Data;

                    return PartialView("_Edit", Session["OrganizationEditFormModel"]);
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

        [HttpPost]
        public ActionResult Edit(EditFormModel editFormModel)
        {
            RequestResult requestResult = new RequestResult();
            try
            {
                var model = Session["OrganizationEditFormModel"] as EditFormModel;
                model.FormInput = editFormModel.FormInput;
                requestResult = OrganizationDataAccessor.Edit(model);
                if (requestResult.IsSuccess)
                {
                    HttpRuntime.Cache.Remove("Organizations");

                    Session.Remove("OrganizationFormAction");
                    Session.Remove("OrganizationEditFormModel");

                    var account = Session["Account"] as Account;
                    account.OrganizationPermissions = OrganizationDataAccessor.GetOrganizationPermissions(account.OrganizationId);
                    Session["Account"] = account;
                }
            }
            catch (Exception e)
            {
                var error = new Error(MethodBase.GetCurrentMethod(), e);
                Logger.Log(error);
                requestResult.ReturnError(error);
                //throw;
            }

            return Content(JsonConvert.SerializeObject(requestResult));
        }

        public ActionResult Delete(string organizationId)
        {
            RequestResult requestResult = OrganizationDataAccessor.Delete(new Guid(organizationId));

            if (requestResult.IsSuccess)
            {
                HttpRuntime.Cache.Remove("Organizations");
            }

            return Content(JsonConvert.SerializeObject(requestResult));
        }

        public ActionResult InitTree()
        {
            try
            {
                var account = Session["Account"] as Account;
                account.OrganizationPermissions = OrganizationDataAccessor.GetOrganizationPermissions(account.OrganizationId);
                Session["Account"] = account;

                var organizations = HttpRuntime.Cache.GetOrInsert("Organizations", () => OrganizationDataAccessor.GetAllOrganizations());

                RequestResult requestResult = new RequestResult();

                if (account.RootOrganizationId == new Guid())
                {
                    requestResult = OrganizationDataAccessor.GetTreeItem(organizations, account.RootOrganizationId, account);
                }
                else
                {
                    requestResult = OrganizationDataAccessor.GetRootTreeItem(organizations, account.RootOrganizationId, account);
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
                throw;
            }
        }

        public ActionResult GetTreeItem(string organizationId)
        {
            string jsonTree = string.Empty;

            try
            {
                var organizationList = HttpRuntime.Cache.GetOrInsert("Organizations", () => OrganizationDataAccessor.GetAllOrganizations());

                RequestResult requestResult = OrganizationDataAccessor.GetTreeItem(organizationList,new Guid(organizationId), Session["Account"] as Account);

                if (requestResult.IsSuccess)
                {
                    jsonTree = JsonConvert.SerializeObject((List<TreeItem>)requestResult.Data);
                }
                else
                {
                    jsonTree = string.Empty;
                }
            }
            catch (Exception e)
            {
                jsonTree = string.Empty;

                Logger.Log(MethodBase.GetCurrentMethod(), e);
            }

            return Content(jsonTree);
        }

        public ActionResult InitQueryableOrganizationSelectTree(Guid editableAncestorOrganizationId, Guid ancestorOrganizationId)
        {
            try
            {
                var organizations = HttpRuntime.Cache.GetOrInsert("Organizations", () => OrganizationDataAccessor.GetAllOrganizations());

                RequestResult result = OrganizationDataAccessor.GetQueryableOrganizationRootTreeItem(organizations, editableAncestorOrganizationId, ancestorOrganizationId, Session["Account"] as Account);

                if (result.IsSuccess)
                {
                    return PartialView("_QueryableOrganizationSelectTree", JsonConvert.SerializeObject((List<TreeItem>)result.Data));
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

        public ActionResult GetQueryableOrganizationSelectTreeItem(Guid editableAncestorOrganizationId, Guid organizationId)
        {
            string jsonTree = string.Empty;

            try
            {
                var organizations = HttpRuntime.Cache.GetOrInsert("Organizations", () => OrganizationDataAccessor.GetAllOrganizations());

                RequestResult result = OrganizationDataAccessor.GetQueryableOrganizationTreeItem(organizations, editableAncestorOrganizationId, organizationId, Session["Account"] as Account);

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

        public ActionResult GetQueryableOrganizationSelectedList()
        {
            try
            {
                if ((Define.EnumFormAction)Session["OrganizationFormAction"] == Define.EnumFormAction.Create)
                {
                    return PartialView("_QueryableOrganizationSelectedList", (Session["OrganizationCreateFormModel"] as CreateFormModel).QueryableOrganizations);
                }
                else if ((Define.EnumFormAction)Session["OrganizationFormAction"] == Define.EnumFormAction.Edit)
                {
                    return PartialView("_QueryableOrganizationSelectedList", (Session["OrganizationEditFormModel"] as EditFormModel).QueryableOrganizations);
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

        public ActionResult AddEditableOrganization(string Selecteds)
        {
            RequestResult result = new RequestResult();

            try
            {
                List<Guid> selectedList = JsonConvert.DeserializeObject<List<Guid>>(Selecteds);

                if ((Define.EnumFormAction)Session["OrganizationFormAction"] == Define.EnumFormAction.Create)
                {
                    var model = Session["OrganizationCreateFormModel"] as CreateFormModel;

                    result = OrganizationDataAccessor.AddEditableOrganization(model.EditableOrganizations, selectedList, Session["Account"] as Account);

                    if (result.IsSuccess)
                    {
                        model.EditableOrganizations = result.Data as List<EditableOrganizationModel>;

                        Session["OrganizationCreateFormModel"] = model;
                    }
                }
                else if ((Define.EnumFormAction)Session["OrganizationFormAction"] == Define.EnumFormAction.Edit)
                {
                    var model = Session["OrganizationEditFormModel"] as EditFormModel;

                    result = OrganizationDataAccessor.AddEditableOrganization(model.EditableOrganizations, selectedList, Session["Account"] as Account);

                    if (result.IsSuccess)
                    {
                        model.EditableOrganizations = result.Data as List<EditableOrganizationModel>;

                        Session["OrganizationEditFormModel"] = model;
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

        public ActionResult DeleteEditableOrganization(string OrganizationId)
        {
            RequestResult result = new RequestResult();

            try
            {
                if ((Define.EnumFormAction)Session["OrganizationFormAction"] == Define.EnumFormAction.Create)
                {
                    var model = Session["OrganizationCreateFormModel"] as CreateFormModel;

                    model.EditableOrganizations.Remove(model.EditableOrganizations.First(x => x.OrganizationId == new Guid(OrganizationId)));

                    Session["OrganizationCreateFormModel"] = model;

                    result.Success();
                }
                else if ((Define.EnumFormAction)Session["OrganizationFormAction"] == Define.EnumFormAction.Edit)
                {
                    var model = Session["OrganizationEditFormModel"] as EditFormModel;

                    model.EditableOrganizations.Remove(model.EditableOrganizations.First(x => x.OrganizationId == new Guid(OrganizationId)));

                    Session["OrganizationEditFormModel"] = model;

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

        public ActionResult AddQueryableOrganization(string Selecteds)
        {
            RequestResult result = new RequestResult();

            try
            {
                List<Guid> selectedList = JsonConvert.DeserializeObject<List<Guid>>(Selecteds);

                if ((Define.EnumFormAction)Session["OrganizationFormAction"] == Define.EnumFormAction.Create)
                {
                    var model = Session["OrganizationCreateFormModel"] as CreateFormModel;

                    result = OrganizationDataAccessor.AddQueryableOrganization(model.QueryableOrganizations, selectedList, Session["Account"] as Account);

                    if (result.IsSuccess)
                    {
                        model.QueryableOrganizations = result.Data as List<QueryableOrganizationModel>;

                        Session["OrganizationCreateFormModel"] = model;
                    }
                }
                else if ((Define.EnumFormAction)Session["OrganizationFormAction"] == Define.EnumFormAction.Edit)
                {
                    var model = Session["OrganizationEditFormModel"] as EditFormModel;

                    result = OrganizationDataAccessor.AddQueryableOrganization(model.QueryableOrganizations, selectedList, Session["Account"] as Account);

                    if (result.IsSuccess)
                    {
                        model.QueryableOrganizations = result.Data as List<QueryableOrganizationModel>;

                        Session["OrganizationEditFormModel"] = model;
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

        public ActionResult DeleteQueryableOrganization(string OrganizationId)
        {
            RequestResult result = new RequestResult();

            try
            {
                if ((Define.EnumFormAction)Session["OrganizationFormAction"] == Define.EnumFormAction.Create)
                {
                    var model = Session["OrganizationCreateFormModel"] as CreateFormModel;

                    model.QueryableOrganizations.Remove(model.QueryableOrganizations.First(x => x.OrganizationId == new Guid(OrganizationId)));

                    Session["OrganizationCreateFormModel"] = model;

                    result.Success();
                }
                else if ((Define.EnumFormAction)Session["OrganizationFormAction"] == Define.EnumFormAction.Edit)
                {
                    var model = Session["OrganizationEditFormModel"] as EditFormModel;

                    model.QueryableOrganizations.Remove(model.QueryableOrganizations.First(x => x.OrganizationId == new Guid(OrganizationId)));

                    Session["OrganizationEditFormModel"] = model;

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

        public ActionResult InitEditableOrganizationSelectTree(Guid editableAncestorOrganizationId, Guid ancestorOrganizationId)
        {
            try
            {
                var organizationList = HttpRuntime.Cache.GetOrInsert("Organizations", () => OrganizationDataAccessor.GetAllOrganizations());

                RequestResult result = OrganizationDataAccessor.GetEditableOrganizationRootTreeItem(organizationList, editableAncestorOrganizationId, ancestorOrganizationId, Session["Account"] as Account);

                if (result.IsSuccess)
                {
                    return PartialView("_EditableOrganizationSelectTree", JsonConvert.SerializeObject((List<TreeItem>)result.Data));
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

        public ActionResult GetEditableOrganizationSelectTreeItem(Guid editableAncestorOrganizationId, Guid organizationId)
        {
            string jsonTree = string.Empty;

            try
            {
                var organizationList = HttpRuntime.Cache.GetOrInsert("Organizations", () => OrganizationDataAccessor.GetAllOrganizations());

                RequestResult result = OrganizationDataAccessor.GetEditableOrganizationTreeItem(organizationList, editableAncestorOrganizationId, organizationId, Session["Account"] as Account);

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

        public ActionResult GetEditableOrganizationSelectedList()
        {
            try
            {
                if ((Define.EnumFormAction)Session["OrganizationFormAction"] == Define.EnumFormAction.Create)
                {
                    return PartialView("_EditableOrganizationSelectedList", (Session["OrganizationCreateFormModel"] as CreateFormModel).EditableOrganizations);
                }
                else if ((Define.EnumFormAction)Session["OrganizationFormAction"] == Define.EnumFormAction.Edit)
                {
                    return PartialView("_EditableOrganizationSelectedList", (Session["OrganizationEditFormModel"] as EditFormModel).EditableOrganizations);
                }
                else
                {
                    return PartialView("_Error", new Error(MethodBase.GetCurrentMethod(), Resources.Resource.UnKnownOperation));
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
    }
}