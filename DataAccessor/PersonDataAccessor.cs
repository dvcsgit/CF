using CF;
using Models.Authentication;
using Models.Person;
using Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Utility;
using Utility.Models;

namespace DataAccessor
{
    public class PersonDataAccessor
    {
        public static RequestResult GetPersonOptions(List<PersonModel> personModels, string term, bool isInit)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                var query = personModels.Select(x => new PersonModel
                {
                    ID = x.ID,
                    Name = x.Name,
                    Email = x.Email,
                    OrganizationName = x.OrganizationName
                }).ToList();

                if (!string.IsNullOrEmpty(term))
                {
                    if (isInit)
                    {
                        var itemList = term.Split(',').ToList();
                        query = query.Where(x => itemList.Contains(x.ID)).ToList();
                    }
                    else
                    {
                        query = query.Where(x => x.Person.Contains(term)).ToList();
                    }
                }

                requestResult.ReturnData(query.Select(x => new SelectListItem { Value = x.ID, Text = x.Person }).Distinct().ToList());
            }
            catch (Exception e)
            {
                Error error = new Error(MethodInfo.GetCurrentMethod(), e);
                Logger.Log(error);
                requestResult.ReturnError(error);
                //throw;
            }

            return requestResult;
        }

        public static RequestResult GetCreateFormModel(Guid organizationId)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                using (CFContext context = new CFContext())
                {
                    requestResult.ReturnData(new CreateFormModel()
                    {
                        OrganizationId = organizationId.ToString(),
                        ParentOrganizationFullName = OrganizationDataAccessor.GetOrganizationFullName(organizationId),
                        RoleModels = context.Roles.Select(r => new RoleModel { RoleId = r.RoleId, RoleName = r.Name }).OrderBy(r => r.RoleId).ToList()
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

        public static RequestResult GetEditFormModel(string pId)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                using (CFContext context = new CFContext())
                {
                    var person = context.People.Include("Roles").First(p => p.LoginId == pId);

                    requestResult.ReturnData(new EditFormModel()
                    {
                        PId = person.LoginId,
                        ParentOrganizationFullName = OrganizationDataAccessor.GetOrganizationFullName(person.OrganizationId),
                        RoleModels = context.Roles.Select(r => new RoleModel { RoleId = r.RoleId, RoleName = r.Name }).OrderBy(r => r.RoleId).ToList(),
                        PersonRoleIds=person.Roles.Select(r=>r.RoleId).ToList(),
                        FormInput = new FormInput()
                        {
                            Id = person.LoginId,
                            Name = person.Name,
                            Title = person.Title,
                            EMail = person.Email,
                            IsMobilePerson = person.IsMobilePerson
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

        public static RequestResult Delete(List<string> selectedList)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
#if !DEBUG
                using (TransactionScope trans = new TransactionScope())
                {
#endif
                using (CFContext context=new CFContext())
                {
                    foreach (var pId in selectedList)
                    {
                        var person = context.People.Include("Roles").First(x => x.LoginId == pId);
                        context.People.Remove(person);                                               
                    }

                    context.SaveChanges();
                }

#if EquipmentPatrol || EquipmentMaintenance
                using (CFContext context=new CFContext())
                    {
                        foreach (var pId in selectedList)
                        {
                            //db.JobUser.RemoveRange(db.JobUser.Where(x => x.UserID == userID).ToList());

                            //db.MJobUser.RemoveRange(db.MJobUser.Where(x => x.UserID == userID).ToList());
                        }

                        db.SaveChanges();
                    }
#endif

#if !DEBUG
                    trans.Complete();
                }
#endif

                requestResult.ReturnSuccessMessage(string.Format("{0} {1} {2}", Resources.Resource.Delete, Resources.Resource.Person, Resources.Resource.Success));
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

        public static RequestResult ResetPassword(string pId)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                using (CFContext context = new CFContext())
                {
                    var person = context.People.First(x => x.LoginId == pId);
                    person.Password = person.LoginId;
                    context.SaveChanges();
                    requestResult.ReturnSuccessMessage(string.Format("{0} {1}", Resources.Resource.ResetPassword, Resources.Resource.Success));
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

        public static RequestResult Move(string organizationId, List<string> selectedList)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
#if !DEBUG
                using (TransactionScope trans = new TransactionScope())
                {
#endif
                using (CFContext context=new CFContext())
                {
                    foreach (var pId in selectedList)
                    {
                        context.People.First(x => x.LoginId == pId).OrganizationId =new Guid(organizationId);
                    }

                    context.SaveChanges();
                }

#if EquipmentPatrol || EquipmentMaintenance
                using (CFContext context=new CFContext())
                    {
                        //foreach (var userID in selectedList)
                        //{
                        //    db.JobUser.RemoveRange(db.JobUser.Where(x => x.UserID == userID).ToList());
                        //}

                        //db.SaveChanges();
                    }
#endif

#if EquipmentMaintenance
                using (CFContext context=new CFContext())
                    {
                        //foreach (var userID in SelectedList)
                        //{
                        //    db.MJobUser.RemoveRange(db.MJobUser.Where(x => x.UserID == userID).ToList());
                        //}

                        //db.SaveChanges();
                    }
#endif

#if !DEBUG
                    trans.Complete();
                }
#endif

                requestResult.ReturnSuccessMessage(string.Format("{0} {1} {2}", Resources.Resource.Move, Resources.Resource.Person, Resources.Resource.Success));
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
                using (CFContext context = new CFContext())
                {
                    var person = context.People.Include("Roles").First(x => x.LoginId == editFormModel.PId);

                    bool exceedsLimit = false;

                    if (editFormModel.FormInput.IsMobilePerson)
                    {
                        var upStreamOrganizationIds = OrganizationDataAccessor.GetUpStreamOrganizationIds(person.OrganizationId, true);

                        PopulationLimit populationLimit = null;

                        foreach (var item in Config.PopulationLimits)
                        {
                            if (upStreamOrganizationIds.Contains(item.OrganizationId))
                            {
                                populationLimit = item;

                                break;
                            }
                        }

                        if (populationLimit != null)
                        {
                            var organization = OrganizationDataAccessor.GetOrganization(populationLimit.OrganizationId);

                            var downStreamOrganizations = OrganizationDataAccessor.GetDownStreamOrganizations(populationLimit.OrganizationId, true);

                            var mobilePeople = context.People.Where(x => x.LoginId != person.LoginId && x.IsMobilePerson && downStreamOrganizations.Contains(x.OrganizationId)).ToList();

                            if (mobilePeople.Count + 1 > populationLimit.NumberOfMobilePeople)
                            {
                                exceedsLimit = true;

                                requestResult.ReturnFailedMessage(string.Format(Resources.Resource.ExceedsMobilePopulationLimit, organization.Name, populationLimit.NumberOfMobilePeople));
                            }
                        }
                    }

                    if (!exceedsLimit)
                    {
#if !DEBUG
                    using (TransactionScope trans = new TransactionScope())
                    {
#endif
                        #region Person
                        person.Name = editFormModel.FormInput.Name;
                        person.Title = editFormModel.FormInput.Title;
                        person.Email = editFormModel.FormInput.EMail;
                        //person.UID = editFormModel.FormInput.UID;
                        person.IsMobilePerson = editFormModel.FormInput.IsMobilePerson;
                        person.LastModifyTime = DateTime.Now;

                        context.SaveChanges();
                        #endregion

                        #region PersonRoles
                        #region Delete
                        if (person != null)
                        {
                            person.Roles = new List<CF.Models.Role>();
                            context.SaveChanges();
                        }

                        #endregion

                        #region Insert
                        foreach(var roleId in editFormModel.FormInput.RoleIds)
                        {
                            var role = context.Roles.First(r => r.RoleId == roleId);
                            person.Roles.Add(role);
                        }
                        context.SaveChanges();
                        #endregion
                        #endregion
#if !DEBUG
                        trans.Complete();
                    }
#endif
                        requestResult.ReturnSuccessMessage(string.Format("{0} {1} {2}", Resources.Resource.Edit, Resources.Resource.Person, Resources.Resource.Success));
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

        public static RequestResult GetCopyFormModel(string pId)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                using (CFContext context = new CFContext())
                {
                    var person = context.People.First(p => p.LoginId == pId);

                    requestResult.ReturnData(new CreateFormModel()
                    {
                        OrganizationId = person.Organization.ToString(),
                        ParentOrganizationFullName = OrganizationDataAccessor.GetOrganizationFullName(person.OrganizationId),
                        RoleModels = context.Roles.Select(r => new RoleModel { RoleId = r.RoleId, RoleName = r.Name }).OrderBy(r => r.RoleId).ToList(),
                        FormInput = new FormInput()
                        {
                            Title = person.Title,
                            IsMobilePerson = person.IsMobilePerson
                        }
                    });
                }
            }
            catch (Exception e)
            {
                var error = new Error(MethodBase.GetCurrentMethod(), e);
                Logger.Log(error);
                requestResult.ReturnError(error);
                throw;
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
                    var exists = context.People.FirstOrDefault(p => p.LoginId == model.FormInput.Id);
                    
                    if (exists == null)
                    {
                        bool exceedsLimit = false;

                        var upStreamOrganizationIds = OrganizationDataAccessor.GetUpStreamOrganizationIds(new Guid(model.OrganizationId), true);

                        PopulationLimit populationLimit = null;

                        foreach(var item in Config.PopulationLimits)
                        {
                            if (upStreamOrganizationIds.Contains(item.OrganizationId))
                            {
                                populationLimit = item;
                                break;
                            }
                        }
                        if (populationLimit != null)
                        {                            
                            var organization = OrganizationDataAccessor.GetOrganization(populationLimit.OrganizationId);
                            var downStreamOrganizations = OrganizationDataAccessor.GetDownStreamOrganizations(populationLimit.OrganizationId, true);
                            var people = context.People.Where(p => downStreamOrganizations.Contains(p.OrganizationId)).ToList();
                            if (people.Count + 1 > populationLimit.NumberOfPeople)
                            {
                                exceedsLimit = true;
                                requestResult.ReturnFailedMessage(string.Format(Resources.Resource.ExceedsPopulationLimit, organization.Name, populationLimit.NumberOfPeople));
                            }
                            else
                            {
                                if (model.FormInput.IsMobilePerson && people.Count(x => x.IsMobilePerson) + 1 > populationLimit.NumberOfMobilePeople)
                                {
                                    exceedsLimit = true;
                                    requestResult.ReturnFailedMessage(string.Format(Resources.Resource.ExceedsMobilePopulationLimit, organization.Name, populationLimit.NumberOfMobilePeople));
                                }
                            }
                        }
                        if (!exceedsLimit)
                        {
                            var roles = context.Roles.Where(r => model.FormInput.RoleIdsString.Contains(r.RoleId)).ToList();
                            context.People.Add(new CF.Models.Person()
                            {
                                OrganizationId = new Guid(model.OrganizationId),
                                LoginId = model.FormInput.Id,
                                Name = model.FormInput.Name,
                                Password = model.FormInput.Id,
                                Title = model.FormInput.Title,
                                Email = model.FormInput.EMail,
                                IsMobilePerson = model.FormInput.IsMobilePerson,
                                LastModifyTime = DateTime.Now,
                                Roles=roles
                            });

                            context.SaveChanges();
                            requestResult.ReturnData(model.FormInput.Id, string.Format("{0} {1} {2}", Resources.Resource.Create, Resources.Resource.Person, Resources.Resource.Success));
                        }
                    }
                    else
                    {
                        requestResult.ReturnFailedMessage(string.Format("{0} {1}", Resources.Resource.PId, Resources.Resource.Exists));
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

        public static RequestResult GetDetailViewModel(string pId, Account account)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                using (CFContext context = new CFContext())
                {
                    var person = context.People.First(p => p.LoginId == pId);

                    requestResult.ReturnData(new DetailViewModel()
                    {
                        Permission = account.OrganizationPermission(person.OrganizationId),
                        OrganizationId = person.OrganizationId.ToString(),
                        ParentOrganizationFullName = OrganizationDataAccessor.GetOrganizationFullName(person.OrganizationId),
                        PId = person.LoginId,
                        Name = person.Name,
                        EMail = person.Email,
                        Title = person.Title,
                        IsMobilePerson = person.IsMobilePerson,
                        RoleNames = person.Roles.Select(r => r.Name).ToList()
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

        public static RequestResult Query(QueryParameters queryParameters, Account account)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                using(CFContext context=new CFContext())
                {
                    var downStreamOrganizations = OrganizationDataAccessor.GetDownStreamOrganizationIds(new Guid(queryParameters.OrganizationId), true);

                    var query = (from p in context.People
                                 join o in context.Organizations
                                 on p.OrganizationId equals o.OrganizationId
                                 where downStreamOrganizations.Contains(p.OrganizationId) && account.QueryableOrganizationIds.Contains(p.OrganizationId)
                                 select new
                                 {
                                     Person = p,
                                     OrganizationName = o.Name
                                 }).AsQueryable();
                    if (!string.IsNullOrEmpty(queryParameters.Keyword))
                    {
                        query = query.Where(x => x.Person.Title.Contains(queryParameters.Keyword) || x.Person.LoginId.Contains(queryParameters.Keyword) || x.Person.Name.Contains(queryParameters.Keyword));
                    }

                    var organization = OrganizationDataAccessor.GetOrganization(new Guid(queryParameters.OrganizationId));

                    var items = query.ToList();

                    var model = new GridViewModel()
                    {
                        Permission = account.OrganizationPermission(new Guid(queryParameters.OrganizationId)),
                        OrganizationId = queryParameters.OrganizationId,
                        OrganizationName = organization.Name,
                        FullOrganizationName = organization.FullName,
                        Items = items.Select(x => new GridItem()
                        {
                            Permission = account.OrganizationPermission(x.Person.OrganizationId),
                            OrganizationName = x.OrganizationName,
                            Title = x.Person.Title,
                            Id = x.Person.LoginId,
                            Name = x.Person.Name,
                            Email = x.Person.Email,
                            IsMobilePerson = x.Person.IsMobilePerson,
                            RoleNameList = x.Person.Roles.Select(r => r.Name).ToList()
                        }).OrderBy(x => x.OrganizationName).ThenBy(x => x.Title).ThenBy(x => x.Id).ToList()
                    };

                    var ancestorOrganizationId = OrganizationDataAccessor.GetAncestorOrganizationId(new Guid(queryParameters.OrganizationId));

                    var downStreams = OrganizationDataAccessor.GetDownStreamOrganizations(ancestorOrganizationId, true);

                    foreach(var downStream in downStreams)
                    {
                        if (account.EditableOrganizationIds.Any(x => x == downStream))
                        {

                            model.MoveToTargets.Add(new MoveToTarget()
                            {
                                Id = downStream,
                                Name = OrganizationDataAccessor.GetOrganizationFullName(downStream),
                                Direction = Define.EnumMoveDirection.Down
                            });
                        }
                    }

                    model.MoveToTargets = model.MoveToTargets.OrderBy(x => x.Name).ToList();

                    requestResult.ReturnData(model);
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

        public static RequestResult GetTreeItem(List<Organization> organizations, Guid rootOrganizationId, Account account)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                var treeItems = new List<TreeItem>();
                var attributes = new Dictionary<Define.EnumTreeAttribute, string>()
                {
                    { Define.EnumTreeAttribute.NodeType,string.Empty },
                    {Define.EnumTreeAttribute.ToolTip,string.Empty },
                    {Define.EnumTreeAttribute.OrganizationId,string.Empty },
                    {Define.EnumTreeAttribute.PersonId,string.Empty }
                };

                using (CFContext context = new CFContext())
                {
                    if (account.QueryableOrganizationIds.Contains(rootOrganizationId))
                    {
                        var people = context.People.Where(p => p.OrganizationId == rootOrganizationId).OrderBy(p => p.LoginId).ToList();

                        foreach(var p in people)
                        {
                            var treeItem = new TreeItem() { Title = p.Name };

                            attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.Person.ToString();
                            attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", p.LoginId, p.Name);
                            attributes[Define.EnumTreeAttribute.OrganizationId] = rootOrganizationId.ToString();
                            attributes[Define.EnumTreeAttribute.LoginId] = p.LoginId;

                            foreach(var a in attributes)
                            {
                                treeItem.Attributes[a.Key.ToString()] = a.Value;
                            }

                            treeItems.Add(treeItem);
                        }
                    }

                    var newOrganizations = organizations.Where(o => o.ParentId == rootOrganizationId && account.VisibleOrganizationIds.Contains(o.OrganizationId)).OrderBy(o => o.OId).ToList();
                    foreach(var o in newOrganizations)
                    {
                        var treeItem = new TreeItem() { Title = o.Name };

                        attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.Organization.ToString();
                        attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", o.OId, o.Name);
                        attributes[Define.EnumTreeAttribute.OrganizationId] = o.OrganizationId.ToString();
                        attributes[Define.EnumTreeAttribute.LoginId] = string.Empty;

                        foreach(var a in attributes)
                        {
                            treeItem.Attributes[a.Key.ToString()] = a.Value;
                        }

                        bool haveDownStreamOrganization = organizations.Any(os => os.ParentId == o.OrganizationId && account.VisibleOrganizationIds.Contains(os.OrganizationId));

                        bool havePerson = account.QueryableOrganizationIds.Contains(o.OrganizationId) && context.People.Any(p => p.OrganizationId == o.OrganizationId);

                        if (haveDownStreamOrganization || havePerson)
                        {
                            treeItem.State = "closed";
                        }

                        treeItems.Add(treeItem);
                    }
                }

                requestResult.ReturnData(treeItems);
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

        public static RequestResult GetRootTreeItem(List<Models.Shared.Organization> organizations, Account account)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                var treeItems = new List<TreeItem>();
                var attributes = new Dictionary<Define.EnumTreeAttribute, string>()
                {
                    {Define.EnumTreeAttribute.NodeType,Define.EnumTreeNodeType.Organization.ToString()},
                    {Define.EnumTreeAttribute.ToolTip,string.Empty },
                    {Define.EnumTreeAttribute.OrganizationId,string.Empty },
                    {Define.EnumTreeAttribute.PersonId,string.Empty }
                };

                using (CFContext context = new CFContext())
                {
                    var organization = organizations.First(o => o.OrganizationId == account.OrganizationId);
                    var treeItem = new TreeItem() { Title = organization.Name };

                    attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", organization.OId, organization.Name);
                    attributes[Define.EnumTreeAttribute.OrganizationId] = organization.OrganizationId.ToString();
                    attributes[Define.EnumTreeAttribute.PersonId] = string.Empty;

                    foreach(var attribute in attributes)
                    {
                        treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                    }

                    bool haveDownStreamOrganization = organizations.Any(o => o.ParentId == organization.OrganizationId && account.VisibleOrganizationIds.Contains(o.OrganizationId));
                    bool havePerson = account.QueryableOrganizationIds.Contains(organization.OrganizationId) && context.People.Any(p => p.OrganizationId == organization.OrganizationId);
                    if (haveDownStreamOrganization || havePerson)
                    {
                        treeItem.State = "closed";
                    }

                    treeItems.Add(treeItem);
                }

                requestResult.ReturnData(treeItems);
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
