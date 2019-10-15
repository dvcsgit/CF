using CF;
using Models.Authentication;
using Models.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utility;
using Utility.Models;

namespace DataAccessor
{
    public class RoleDataAccessor
    {
        public static RequestResult Query(QueryParameters parameters)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                using (CFContext context = new CFContext())
                {
                    var query = context.Roles.AsQueryable();

                    if (!string.IsNullOrEmpty(parameters.Keyword))
                    {
                        query = query.Where(x => x.RoleId.Contains(parameters.Keyword) || x.Name.Contains(parameters.Keyword));
                    }

                    requestResult.ReturnData(new GridViewModel()
                    {
                        Parameters = parameters,
                        ItemList = query.Select(x => new GridItem()
                        {
                            RoleId = x.RoleId,
                            RoleName = x.Name
                        }).OrderBy(x => x.RoleId).ToList()
                    });
                }
            }
            catch(Exception e)
            {
                var error = new Error(MethodBase.GetCurrentMethod(), e);

                Logger.Log(error);
                requestResult.ReturnError(error);
            }

            return requestResult;
        }

        public static RequestResult GetDetailViewModel(string roleId)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                requestResult = GetWebPermissions();
                if (requestResult.IsSuccess)
                {
                    using (CFContext context = new CFContext())
                    {
                        var role = context.Roles.Include("People").First(r => r.RoleId == roleId);
                        requestResult.ReturnData(new DetailViewModel()
                        {
                            RoleId = role.RoleId,
                            RoleName = role.Name,
                            WebPermissionFunction = new WebPermissionFunctionModel()
                            {
                                WebPermissions=requestResult.Data as List<WebPermissionModel>,
                                RoleWebPermissionFunctions=context.RolePermissionFunctions.Where(wpwf=>wpwf.RoleId==role.RoleId).Select(wpwf=>new RoleWebPermissionFunctionModel { RoleId=wpwf.RoleId,WebPermissionId=wpwf.WebPermissionId,WebFunctionId=wpwf.WebFunctionId}).ToList()
                            },
                            People=role.People.Select(p=>new PersonModel
                            {
                                OrganizationName=p.OrganizationId.ToString()== "00000000-0000-0000-0000-000000000000" ? "MANAGER" : context.Organizations.Where(o=>o.OrganizationId== p.OrganizationId).Select(o=>o.Name).First(),
                                PId=p.LoginId.ToString(),
                                PersonName=p.Name
                            }).OrderBy(p=>p.OrganizationName).ThenBy(p=>p.PId).ToList()
                        });
                    }
                }
            }
            catch (Exception e)
            {
                var error = new Error(MethodBase.GetCurrentMethod(), e);
                Logger.Log(error);
                requestResult.ReturnError(error);
                //throw;
            }

            return requestResult;
        }

        public static RequestResult Create(CreateFormModel model)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                using (CFContext context = new CFContext())
                {
                    var exists = context.Roles.FirstOrDefault(r => r.RoleId == model.FormInput.RoleId);

                    if (exists == null)
                    {
                        var pIds = model.People.Select(p => p.PId);
                        context.Roles.Add(new CF.Models.Role()
                        {
                            RoleId = model.FormInput.RoleId,
                            Name = model.FormInput.Name,
                            People = context.People.Where(p => pIds.Contains(p.LoginId)).ToList(),
                            WebPermissionWebFunctions = model.FormInput.WebPermissionFunctionList.Select(x => new CF.Models.RolePermissionFunction
                            {
                                RoleId = model.FormInput.RoleId,
                                WebPermissionId = x.WebPermissionId,
                                WebFunctionId = x.WebFunctionId
                            }).ToList()
                        });

                        context.SaveChanges();

                        requestResult.ReturnSuccessMessage(string.Format("{0} {1} {2}", Resources.Resource.Create, Resources.Resource.Role, Resources.Resource.Success));
                    }
                    else
                    {
                        requestResult.ReturnFailedMessage(string.Format("{0} {1}", Resources.Resource.RoleId, Resources.Resource.Exists));
                    }
                }
            }
            catch (Exception e)
            {
                var error = new Error(MethodBase.GetCurrentMethod(), e);
                Logger.Log(error);
                requestResult.ReturnError(error);
                //throw;
            }

            return requestResult;
        }

        public static RequestResult Edit(EditFormModel editFormModel)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                using(CFContext context=new CFContext())
                {
#if !DEBUG
                    using (TransactionScope trans = new TransactionScope())
                    {
#endif
                    #region Role
                    var role = context.Roles.Include("People").First(x => x.RoleId == editFormModel.RoleId);

                    role.Name = editFormModel.FormInput.Name;

                    context.SaveChanges();
                    #endregion

                    #region RoleWebPermissionFunction
                    #region Delete
                    context.RolePermissionFunctions.RemoveRange(context.RolePermissionFunctions.Where(x => x.RoleId == role.RoleId).ToList());

                    context.SaveChanges();
                    #endregion

                    #region Insert
                    context.RolePermissionFunctions.AddRange(editFormModel.FormInput.WebPermissionFunctionList.Select(x => new CF.Models.RolePermissionFunction
                    {
                        RoleId = role.RoleId,
                        WebPermissionId = x.WebPermissionId,
                        WebFunctionId = x.WebFunctionId
                    }).ToList());

                    context.SaveChanges();
                    #endregion
                    #endregion

                    #region RolePeople
                    #region Delete
                    role.People = new List<CF.Models.Person>();

                    context.SaveChanges();
                    #endregion

                    #region Insert
                    var pIds = editFormModel.People.Select(p => p.PId);
                    role.People = context.People.Where(p => pIds.Contains(p.LoginId)).ToList();

                    context.SaveChanges();
                    #endregion
                    #endregion
#if !DEBUG
                        trans.Complete();
                    }
#endif

                    requestResult.ReturnSuccessMessage(string.Format("{0} {1} {2}", Resources.Resource.Edit, Resources.Resource.Role, Resources.Resource.Success));
                }
            }
            catch (Exception e)
            {
                var error = new Error(MethodBase.GetCurrentMethod(), e);
                Logger.Log(error);
                requestResult.ReturnError(error);
                //throw;
            }

            return requestResult;
        }

        public static object Delete(string roleId)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                using (CFContext context = new CFContext())
                {
                    context.Roles.Remove(context.Roles.Include("People").First(x => x.RoleId == roleId));                    
                    context.SaveChanges();
                }

                requestResult.ReturnSuccessMessage(string.Format("{0} {1} {2}", Resources.Resource.Delete, Resources.Resource.Role, Resources.Resource.Success));
            }
            catch (Exception e)
            {
                Error error = new Error(MethodBase.GetCurrentMethod(), e);
                Logger.Log(error);
                requestResult.ReturnError(error);
                //throw;
            }

            return requestResult;
        }

        public static RequestResult GetEditFormModel(string roleId, Account account)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                requestResult = GetWebPermissions();

                if (requestResult.IsSuccess)
                {
                    using (CFContext context = new CFContext())
                    {
                        var role = context.Roles.First(r => r.RoleId == roleId);
                        requestResult.ReturnData(new EditFormModel()
                        {
                            AncestorOrganizationId = OrganizationDataAccessor.GetAncestorOrganizationId(account.OrganizationId).ToString(),
                            RoleId = role.RoleId,
                            WebPermissionFunction = new WebPermissionFunctionModel()
                            {
                                WebPermissions = requestResult.Data as List<WebPermissionModel>,
                                RoleWebPermissionFunctions = context.RolePermissionFunctions.Where(wpwf => wpwf.RoleId == role.RoleId).Select(wpwf => new Models.Role.RoleWebPermissionFunctionModel { RoleId = wpwf.RoleId, WebPermissionId = wpwf.WebPermissionId, WebFunctionId = wpwf.WebFunctionId }).ToList()
                            },
                            FormInput = new FormInput()
                            {
                                RoleId = role.RoleId,
                                Name = role.Name
                            },
                            People = (from p in role.People
                                      join o in context.Organizations
                                      on p.OrganizationId equals o.OrganizationId
                                      select new Models.Role.PersonModel
                                      {
                                          OrganizationName = o.Name,
                                          PId = p.LoginId,
                                          PersonName = p.Name
                                      }).OrderBy(x => x.OrganizationName).ThenBy(x => x.PId).ToList()
                        });
                    }
                }
            }
            catch (Exception e)
            {
                var error = new Error(MethodBase.GetCurrentMethod(), e);
                Logger.Log(error);
                requestResult.ReturnError(error);
                //throw;
            }

            return requestResult;
        }

        public static RequestResult AddPerson(List<PersonModel> people, List<string> selectedList)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                string[] seperator = new string[] { Define.Seperator };

                using (CFContext context = new CFContext())
                {
                    foreach(string selected in selectedList)
                    {
                        string[] temp = selected.Split(seperator, StringSplitOptions.None);

                        var organizationId = temp[0];
                        var pId = temp[1];

                        if (!string.IsNullOrEmpty(pId))
                        {
                            if (!people.Any(p => p.PId == pId))
                            {
                                //var person = context.People.First(p => p.LoginId == pId);
                                people.Add(context.People.Where(p => p.LoginId == pId).Select(p => new PersonModel { PId = pId, PersonName = p.Name, OrganizationName = p.Organization.Name }).First());                                
                            }
                        }
                        else
                        {
                            var organizations = OrganizationDataAccessor.GetDownStreamOrganizations(new Guid(organizationId), true);

                            var newPeople = context.People.Where(p => organizations.Contains(p.OrganizationId)).ToList();
                            foreach(var person in newPeople)
                            {
                                if (!people.Any(p => p.PId == person.LoginId))
                                {
                                    people.Add(new PersonModel { PId = person.LoginId, PersonName = person.Name, OrganizationName = person.Organization.Name });
                                }
                            }
                        }
                    }
                }

                requestResult.ReturnData(people.OrderBy(p => p.OrganizationName).ThenBy(p => p.PId).ToList());
            }
            catch (Exception e)
            {
                var error = new Error(MethodInfo.GetCurrentMethod(), e);
                Logger.Log(error);
                requestResult.ReturnError(error);
                //throw;
            }

            return requestResult;
        }        

        private static RequestResult GetWebPermissions()
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                var items = new List<WebPermissionModel>();

                using (CFContext context = new CFContext())
                {
                    //A
                    var ancestors = context.WebPermissions.Where(wp => wp.ParentId == "*").OrderBy(wp => wp.Seq).ToList();

                    foreach (var ancestor in ancestors)
                    {
                        var item = new WebPermissionModel()
                        {
                            WebPermissionId = ancestor.WebPermissionId,
                            PermissionName = context.WebPermissionNames.Where(wpn => wpn.WebPermissionId == ancestor.WebPermissionId).ToDictionary(wpn => wpn.Language, wpn => wpn.Name),
                            SubItems = new List<WebPermissionModel>(),
                            WebFunctions = null
                        };
                        //AB
                        var parents = context.WebPermissions.Include("WebPermissionWebFunctions").Where(wp => wp.ParentId == ancestor.WebPermissionId).OrderBy(wp => wp.Seq).ToList();//AB
                        
                        foreach (var parent in parents)
                        {
                            if (!string.IsNullOrEmpty(parent.Controller))
                            {
                                var webFunctionIds = parent.WebPermissionWebFunctions.Select(wpwf => wpwf.WebFunctionId).Distinct();
                                List<WebFunctionModel> webFunctionModels = new List<WebFunctionModel>();
                                foreach(var wfi in webFunctionIds)
                                {
                                    var webFunctionModel = new WebFunctionModel
                                    {
                                        WebFunctionId = wfi,
                                        FunctionName = context.WebFunctionNames.Where(wfd => wfd.WebFunctionId == wfi).ToDictionary(wfd => wfd.Language, wfd => wfd.Name)
                                    };
                                    webFunctionModels.Add(webFunctionModel);
                                }

                                item.SubItems.Add(new WebPermissionModel
                                {
                                    WebPermissionId = parent.WebPermissionId,
                                    PermissionName = context.WebPermissionNames.Where(wpd => wpd.WebPermissionId == parent.WebPermissionId).ToDictionary(wpd => wpd.Language, wpd => wpd.Name),
                                    SubItems = null,
                                    WebFunctions = webFunctionModels
                                });
                            }
                            else
                            {
                                var permissions = context.WebPermissions.Include("WebPermissionWebFunctions").Where(wp => wp.ParentId == parent.WebPermissionId).OrderBy(wp => wp.Seq).ToList();//ABC
                               
                                item.SubItems.AddRange(permissions.Select(p => new WebPermissionModel
                                {
                                    WebPermissionId = p.WebPermissionId,
                                    PermissionName = context.WebPermissionNames.Where(wpn => wpn.WebPermissionId == p.WebPermissionId).ToDictionary(wpn => wpn.Language, wpn => wpn.Name),
                                    SubItems = null,
                                    WebFunctions = p.WebPermissionWebFunctions.Select(wpwf => new WebFunctionModel
                                    {
                                        WebFunctionId = wpwf.WebFunctionId,
                                        FunctionName = context.WebFunctionNames.Where(wfn => wfn.WebFunctionId == wpwf.WebFunctionId).ToDictionary(wfn => wfn.Language, wfn => wfn.Name)
                                    }).Distinct(new PropertyCompare<WebFunctionModel>("WebFunctionId")).ToList()//.GroupBy(wf=>wf.WebFunctionId).Select(x=>x.First()) The usage of method GroupBy is for distinct WebFunctions.
                                }));
                            }
                        }
                        if (item.SubItems != null && item.SubItems.Count > 0)
                        {
                            items.Add(item);
                        }
                    }
                }
                requestResult.ReturnData(items);
            }
            catch (Exception e)
            {
                var error = new Error(MethodBase.GetCurrentMethod(), e);
                Logger.Log(error);
                requestResult.ReturnError(error);
                //throw;
            }

            return requestResult;
        }

        public static RequestResult GetCreateFormModel(Account account)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                requestResult = GetWebPermissions();

                if (requestResult.IsSuccess)
                {
                    requestResult.ReturnData(new CreateFormModel()
                    {
                        AncestorOrganizationId = OrganizationDataAccessor.GetAncestorOrganizationId(account.OrganizationId).ToString(),
                        WebPermissionFunction = new WebPermissionFunctionModel()
                        {
                            WebPermissions = requestResult.Data as List<WebPermissionModel>
                        }
                    });
                }
            }
            catch (Exception e)
            {
                var error = new Error(MethodBase.GetCurrentMethod(), e);
                Logger.Log(error);
                requestResult.ReturnError(error);
                //throw;
            }

            return requestResult;
        }
    }
}
