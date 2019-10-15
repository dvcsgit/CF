using DataAccessor;
using Models.Authentication;
using Models.Role;
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
    public class RoleController : Controller
    {
        // GET: Role
        public ActionResult Index()
        {
            return View(new QueryFormModel());
        }

        public ActionResult Query(QueryParameters parameters)
        {
            RequestResult requestResult = RoleDataAccessor.Query(parameters);

            if (requestResult.IsSuccess)
            {
                return PartialView("_List", requestResult.Data);
            }
            else
            {
                return PartialView("_Error", requestResult.Error);
            }
        }

        public ActionResult Detail(string roleId)
        {
            RequestResult requestResult = RoleDataAccessor.GetDetailViewModel(roleId);

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
        public ActionResult Create()
        {
            RequestResult requestResult = RoleDataAccessor.GetCreateFormModel(Session["Account"] as Account);

            if (requestResult.IsSuccess)
            {
                Session["RoleFormAction"] = Define.EnumFormAction.Create;
                Session["RoleCreateFormModel"] = requestResult.Data;

                return PartialView("_Create", Session["RoleCreateFormModel"]);
            }
            else
            {
                return PartialView("_Error", requestResult.Error);
            }
        }

        [HttpPost]
        public ActionResult Create(CreateFormModel createFormModel)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                var model = Session["RoleCreateFormModel"] as CreateFormModel;
                model.FormInput = createFormModel.FormInput;
                requestResult = RoleDataAccessor.Create(model);
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

        [HttpGet]
        public ActionResult Edit(string roleId)
        {
            RequestResult requestResult = RoleDataAccessor.GetEditFormModel(roleId, Session["Account"] as Account);

            if (requestResult.IsSuccess)
            {
                Session["RoleFormAction"] = Define.EnumFormAction.Edit;
                Session["RoleEditFormModel"] = requestResult.Data;

                return PartialView("_Edit", Session["RoleEditFormModel"]);
            }
            else
            {
                return PartialView("_Error", requestResult.Error);
            }
        }

        [HttpPost]
        public ActionResult Edit(EditFormModel editFormModel)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                var model = Session["RoleEditFormModel"] as EditFormModel;
                model.FormInput = editFormModel.FormInput;
                requestResult = RoleDataAccessor.Edit(model);
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

        public ActionResult Delete(string roleId)
        {
            return Content(JsonConvert.SerializeObject(RoleDataAccessor.Delete(roleId)));
        }

        public ActionResult GetSelectPersonTreeItem(string organizationId)
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
            catch (Exception e)
            {
                jsonTree = string.Empty;

                Logger.Log(MethodBase.GetCurrentMethod(), e);
            }

            return Content(jsonTree);
        }

        public ActionResult GetSelectedPerson()
        {
            try
            {
                if ((Define.EnumFormAction)Session["RoleFormAction"] == Define.EnumFormAction.Create)
                {
                    return PartialView("_SelectedPeople", (Session["RoleCreateFormModel"] as CreateFormModel).People);
                }
                else if ((Define.EnumFormAction)Session["RoleFormAction"] == Define.EnumFormAction.Edit)
                {
                    return PartialView("_SelectedPeople", (Session["RoleEditFormModel"] as EditFormModel).People);
                }
                else
                {
                    return PartialView("_Error", new Error(MethodInfo.GetCurrentMethod(), Resources.Resource.UnKnownOperation));
                }
            }
            catch (Exception e)
            {
                var error = new Error(MethodInfo.GetCurrentMethod(), e);
                Logger.Log(error);
                return PartialView("_Error", error);
                //throw;
            }
        }

        public ActionResult InitSelectPersonTree(string ancestorOrganizationId)
        {
            try
            {
                var organizatios = HttpRuntime.Cache.GetOrInsert<List<Organization>>("Organizations", () => OrganizationDataAccessor.GetAllOrganizations());
                RequestResult requestResult = PersonDataAccessor.GetRootTreeItem(organizatios, Session["Account"] as Account);
                if (requestResult.IsSuccess)
                {
                    return PartialView("_SelectPersonTree", JsonConvert.SerializeObject((List<TreeItem>)requestResult.Data));
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

        public ActionResult AddPerson(string selecteds)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                List<string> selectedList = JsonConvert.DeserializeObject<List<string>>(selecteds);

                if ((Define.EnumFormAction)Session["RoleFormAction"] == Define.EnumFormAction.Create)
                {
                    var model = Session["RoleCreateFormModel"] as CreateFormModel;

                    requestResult = RoleDataAccessor.AddPerson(model.People, selectedList);

                    if (requestResult.IsSuccess)
                    {
                        model.People = requestResult.Data as List<Models.Role.PersonModel>;

                        Session["RoleCreateFormModel"] = model;
                    }
                }
                else if ((Define.EnumFormAction)Session["RoleFormAction"] == Define.EnumFormAction.Edit)
                {
                    var model = Session["RoleEditFormModel"] as EditFormModel;

                    requestResult = RoleDataAccessor.AddPerson(model.People, selectedList);

                    if (requestResult.IsSuccess)
                    {
                        model.People = requestResult.Data as List<Models.Role.PersonModel>;

                        Session["RoleEditFormModel"] = model;
                    }
                }
                else
                {
                    requestResult.ReturnFailedMessage(Resources.Resource.UnKnownOperation);
                }
            }
            catch (Exception ex)
            {
                Error err = new Error(MethodInfo.GetCurrentMethod(), ex);

                Logger.Log(err);

                requestResult.ReturnError(err);
            }

            return Content(JsonConvert.SerializeObject(requestResult));
        }

        public ActionResult DeletePerson(string pId)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                if ((Define.EnumFormAction)Session["RoleFormAction"] == Define.EnumFormAction.Create)
                {
                    var model = Session["RoleCreateFormModel"] as CreateFormModel;

                    model.People.Remove(model.People.First(x => x.PId == pId));

                    Session["RoleCreateFormModel"] = model;

                    requestResult.Success();
                }
                else if ((Define.EnumFormAction)Session["RoleFormAction"] == Define.EnumFormAction.Edit)
                {
                    var model = Session["RoleEditFormModel"] as EditFormModel;

                    model.People.Remove(model.People.First(x => x.PId == pId));

                    Session["RoleEditFormModel"] = model;

                    requestResult.Success();
                }
                else
                {
                    requestResult.ReturnFailedMessage(Resources.Resource.UnKnownOperation);
                }
            }
            catch (Exception ex)
            {
                Error err = new Error(MethodInfo.GetCurrentMethod(), ex);

                Logger.Log(err);

                requestResult.ReturnError(err);
            }

            return Content(JsonConvert.SerializeObject(requestResult));
        }
    }
}