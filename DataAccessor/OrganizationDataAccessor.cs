using CF;
using CF.Models;
using Models.Authentication;
using Models.Organization;
using Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Utility;
using Utility.Models;

namespace DataAccessor
{
    public class OrganizationDataAccessor
    {
        public static Guid GetRootOrganizationId(List<Models.Shared.Organization> allOrganizations, Account account)
        {
            var rootOrganizationId = Guid.Empty;
            try
            {
                if (account.OrganizationId == new Guid())
                {
                    rootOrganizationId = new Guid();
                }
                else
                {
                    var ancestorOrganizationId = GetAncestorOrganizationId(account.OrganizationId);
                    if (account.QueryableOrganizationIds.Contains(ancestorOrganizationId))
                    {
                        rootOrganizationId = ancestorOrganizationId;
                    }
                    else
                    {
                        var organizations = allOrganizations.Where(x => x.ParentId == ancestorOrganizationId).ToList();
                        rootOrganizationId = GetRootOrganizationId(allOrganizations, account, organizations, ancestorOrganizationId);
                    }
                }
            }
            catch (Exception e)
            {
                rootOrganizationId = Guid.Empty;
                Logger.Log(MethodBase.GetCurrentMethod(), e);
            }

            return rootOrganizationId;
        }

        public static Guid GetRootOrganizationId(List<Models.Shared.Organization> allOrganizations, Account account, List<Models.Shared.Organization> organizations, Guid parentOrganizationId)
        {
            var intersectResult = account.QueryableOrganizationIds.Intersect(organizations.Select(x => x.OrganizationId));
            if (intersectResult.Count() == 0)
            {
                var queryableOrganizationIds = new List<Guid>();
                foreach (var organization in organizations)
                {
                    var downStreamOrganizations = GetDownStreamOrganizations(allOrganizations, organization.OrganizationId, false);
                    var i = account.QueryableOrganizationIds.Intersect(downStreamOrganizations).ToList();
                    if (i.Count > 0)
                    {
                        queryableOrganizationIds.Add(organization.OrganizationId);
                    }
                }

                if (queryableOrganizationIds.Count > 1)
                {
                    return parentOrganizationId;
                }
                else
                {
                    var organizationId = queryableOrganizationIds.First();
                    var tempOrganizations = allOrganizations.Where(x => x.ParentId == organizationId).ToList();

                    return GetRootOrganizationId(allOrganizations, account, tempOrganizations, organizationId);
                }
            }
            else if (intersectResult.Count() == 1)
            {
                return intersectResult.First();
            }
            else
            {
                return parentOrganizationId;
            }
        }

        public static RequestResult GetDetailViewModel(Guid organizationId, Account account)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                using (CFContext context = new CFContext())
                {
                    var organization = context.Organizations.Include("Manager").Where(o => o.OrganizationId == organizationId).Select(o => new { o.OrganizationId, o.ParentId, o.OId, o.Name, o.Manager }).First();

                    requestResult.ReturnData(new DetailViewModel()
                    {
                        OrganizationId = organization.OrganizationId.ToString(),
                        Permission = account.OrganizationPermission(organizationId),
                        ParentOrganizationFullName = GetOrganizationFullName(organization.ParentId),
                        OId = organization.OId,
                        Name = organization.Name,
                        EditableOrganizations = GetEditableOrganizationIds(organizationId).Union(GetDownStreamOrganizationIds(organizationId, true)).Select(x => GetOrganizationFullName(x)).OrderBy(x => x).ToList(),
                        QueryableOrganizations = GetQueryableOrganizationIds(organizationId).Select(x => GetOrganizationFullName(x)).OrderBy(x => x).ToList(),
                        //ManagerList=organization.Manager
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

        public static RequestResult GetEditFormModel(Guid organizationId)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                using (CFContext context = new CFContext())
                {
                    var organization = context.Organizations.Where(o => o.OrganizationId == organizationId).FirstOrDefault();

                    requestResult.ReturnData(new EditFormModel()
                    {
                        OrganizationId = organization.OrganizationId.ToString(),
                        AncestorOrganizationId = GetAncestorOrganizationId(organization.OrganizationId).ToString(),
                        ParentOrganizationFullName = GetOrganizationFullName(organization.ParentId),
                        FormInput = new FormInput()
                        {
                            OId = organization.OId,
                            Name = organization.Name,
                        },
                        ManagerList = context.OrganizationManagers.Where(om => om.OrganizationId == organizationId).Select(om => om.ManagerId).ToList(),
                        EditableOrganizations = GetEditableOrganizationIds(organizationId).Select(eo => new EditableOrganizationModel
                        {
                            CanDelete = true,
                            OrganizationId = eo,
                            FullName = GetOrganizationFullName(eo)
                        }).Union(GetDownStreamOrganizationIds(organizationId, true).Select(o => new EditableOrganizationModel
                        {
                            CanDelete = false,
                            OrganizationId = o,
                            FullName = GetOrganizationFullName(o)
                        })).OrderBy(x => x.FullName).ToList(),
                        QueryableOrganizations = GetQueryableOrganizationIds(organizationId).Select(qo => new QueryableOrganizationModel
                        {
                            OrganizationId = qo,
                            FullName = GetOrganizationFullName(qo)
                        }).OrderBy(x => x.FullName).ToList()
                    });
                }
            }
            catch (Exception e)
            {
                var error = new Error(MethodBase.GetCurrentMethod(), e);
                Logger.Log(error);
                //throw;
            }

            return requestResult;
        }

        public static string GetOrganizationFullName(Guid organizationId)
        {
            string name = string.Empty;

            try
            {
                if (organizationId == new Guid("00000000-0000-0000-0000-000000000000"))
                {
                    name = "*";
                }
                else
                {
                    using (CFContext context = new CFContext())
                    {
                        var organization = context.Organizations.First(x => x.OrganizationId == organizationId);

                        name = organization.Name;

                        while (organization.ParentId != new Guid("00000000-0000-0000-0000-000000000000"))
                        {
                            organization = context.Organizations.First(x => x.OrganizationId == organization.ParentId);

                            name = organization.Name + " -> " + name;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                name = string.Empty;

                Logger.Log(MethodBase.GetCurrentMethod(), ex);
            }

            return name;
        }

        public static string GetOrganizationName(Guid organizationId)
        {
            string name = string.Empty;

            try
            {
                using (CFContext context = new CFContext())
                {
                    var organization = context.Organizations.FirstOrDefault(x => x.OrganizationId == organizationId);
                    if (organization != null)
                    {
                        name = organization.Name;
                    }
                }
            }
            catch (Exception e)
            {
                name = string.Empty;
                Logger.Log(MethodBase.GetCurrentMethod(), e);
                //throw;
            }

            return name;
        }

        public static RequestResult Delete(Guid organizationId)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                var organizationIds = GetDownStreamOrganizationIds(organizationId, true);

                using (CFContext context = new CFContext())
                {
                    DeleteHelper.DeleteOrganization(context, organizationIds);
                    context.SaveChanges();
                }

                requestResult.ReturnSuccessMessage(string.Format("{0} {1} {2}", Resources.Resource.Delete, Resources.Resource.Organization, Resources.Resource.Success));
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

        public static RequestResult Edit(EditFormModel model)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                using (CFContext context = new CFContext())
                {
                    var organization = context.Organizations.First(o => o.OrganizationId.ToString() == model.OrganizationId);
                    var exists = context.Organizations.FirstOrDefault(o => o.OrganizationId != organization.OrganizationId && o.ParentId == organization.ParentId && o.OId == model.FormInput.OId);
                    if (exists == null)
                    {
                        organization.OId = model.FormInput.OId;
                        organization.Name = model.FormInput.Name;

                        context.SaveChanges();

                        context.OrganizationManagers.RemoveRange(context.OrganizationManagers.Where(o => o.OrganizationId.ToString() == model.OrganizationId));
                        context.EditableOrganizations.RemoveRange(context.EditableOrganizations.Where(o => o.OrganizationId.ToString() == model.OrganizationId));
                        context.QueryableOrganizations.RemoveRange(context.QueryableOrganizations.Where(o => o.OrganizationId.ToString() == model.OrganizationId));

                        context.SaveChanges();

                        if (!string.IsNullOrEmpty(model.FormInput.Managers))
                        {
                            string organizationId = model.OrganizationId;
                            context.OrganizationManagers.AddRange(model.FormInput.Managers.Split(',').ToList().Select(o => new OrganizationManager()
                            {
                                OrganizationId = new Guid(organizationId),
                                ManagerId = o
                            }));
                        }

                        context.EditableOrganizations.AddRange(model.EditableOrganizations.Where(e => e.CanDelete).Select(e => new EditableOrganization
                        {
                            OrganizationId = new Guid(model.OrganizationId),
                            EditableOrganizationId = e.OrganizationId
                        }));

                        context.QueryableOrganizations.AddRange(model.QueryableOrganizations.Select(q => new QueryableOrganization
                        {
                            OrganizationId = new Guid(model.OrganizationId),
                            QueryableOrganizationId = q.OrganizationId
                        }));

                        context.SaveChanges();

                        requestResult.ReturnSuccessMessage(string.Format("{0} {1} {2}", Resources.Resource.Edit, Resources.Resource.Organization, Resources.Resource.Success));
                    }
                    else
                    {
                        requestResult.ReturnFailedMessage(string.Format("{0} {1}", Resources.Resource.OrganizationId, Resources.Resource.Exists));
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

        private static List<Guid> GetQueryableOrganizationIds(Guid organizationId)
        {
            var items = new List<Guid>();
            try
            {
                using (CFContext context = new CFContext())
                {
                    items = context.QueryableOrganizations.Where(qo => qo.OrganizationId == organizationId).Select(qo => qo.QueryableOrganizationId).ToList();
                }
            }
            catch (Exception e)
            {
                items = null;
                Logger.Log(MethodBase.GetCurrentMethod(), e);
                //throw;
            }

            return items;
        }

        private static List<Guid> GetEditableOrganizationIds(Guid organizationId)
        {
            var items = new List<Guid>();
            try
            {
                using (CFContext context = new CFContext())
                {
                    items = context.EditableOrganizations.Where(eo => eo.OrganizationId == organizationId).Select(eo => eo.EditableOrganizationId).ToList();
                }
            }
            catch (Exception e)
            {
                items = null;
                Logger.Log(MethodBase.GetCurrentMethod(), e);
                //throw;
            }

            return items;
        }

        public static Guid GetAncestorOrganizationId(Guid organizationId)
        {
            Guid ancestorOrganizationId = Guid.Empty;
            try
            {
                var upStreamOrganizationIds = GetUpStreamOrganizationIds(organizationId, true);
                using (CFContext context = new CFContext())
                {
                    ancestorOrganizationId = context.Organizations.First(x => x.ParentId==new Guid() && upStreamOrganizationIds.Contains(x.OrganizationId)).OrganizationId;
                }
            }
            catch (Exception e)
            {
                ancestorOrganizationId = Guid.Empty;
                Logger.Log(MethodBase.GetCurrentMethod(), e);
            }

            return ancestorOrganizationId;
        }

        public static List<Guid> GetUpStreamOrganizationIds(Guid organizationId, bool include)
        {
            var itemList = new List<Guid>();
            try
            {
                if (include)
                {
                    itemList.Add(organizationId);
                }

                using (CFContext context = new CFContext())
                {
                    if (organizationId != new Guid())
                    {
                        var organization = context.Organizations.First(x => x.OrganizationId == organizationId);
                        while (organization.ParentId != new Guid())
                        {
                            organization = context.Organizations.First(x => x.OrganizationId == organization.ParentId);
                            itemList.Add(organization.OrganizationId);
                        }
                        itemList.Add(new Guid());
                    }
                }
            }
            catch (Exception e)
            {
                itemList = null;
                Logger.Log(MethodBase.GetCurrentMethod(), e);
            }

            return itemList;
        }

        public static List<Guid> GetDownStreamOrganizationIds(Guid organizationId, bool include)
        {
            var itemList = new List<Guid>();
            try
            {
                if (include)
                {
                    itemList.Add(organizationId);
                }

                using (CFContext context = new CFContext())
                {
                    var organizationIds = context.Organizations.Where(x => x.ParentId == organizationId && x.OrganizationId != x.ParentId).Select(x => x.OrganizationId).ToList();
                    while (organizationIds.Count > 0)
                    {
                        itemList.AddRange(organizationIds);
                        organizationIds = context.Organizations.Where(x => organizationIds.Contains(x.ParentId)).Select(x => x.OrganizationId).ToList();
                    }
                }
            }
            catch (Exception e)
            {
                itemList = null;
                Logger.Log(MethodBase.GetCurrentMethod(), e);
            }

            return itemList;
        }

        public static List<Guid> GetDownStreamOrganizations(List<Models.Shared.Organization> organizations, Guid organizationId, bool include)
        {
            var itemList = new List<Guid>();
            try
            {
                if (include)
                {
                    itemList.Add(organizationId);
                }

                var tempOrganizations = organizations.Where(x => x.ParentId == organizationId).Select(x => x.OrganizationId).ToList();

                while (tempOrganizations.Count > 0)
                {
                    itemList.AddRange(tempOrganizations);
                    tempOrganizations = organizations.Where(x => tempOrganizations.Contains(x.ParentId)).Select(x => x.OrganizationId).ToList();
                }
            }
            catch (Exception e)
            {
                itemList = null;
                Logger.Log(MethodBase.GetCurrentMethod(), e);
            }

            return itemList;
        }

        public static List<Guid> GetDownStreamOrganizations(Guid OrganizationId, bool Include)
        {
            var itemList = new List<Guid>();

            try
            {
                if (Include)
                {
                    itemList.Add(OrganizationId);
                }

                using (CFContext context = new CFContext())
                {
                    //向下搜尋
                    var organizations = context.Organizations.Where(x => x.ParentId == OrganizationId).Select(x => x.OrganizationId).ToList();

                    while (organizations.Count > 0)
                    {
                        itemList.AddRange(organizations);

                        organizations = context.Organizations.Where(x => organizations.Contains(x.ParentId)).Select(x => x.OrganizationId).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                itemList = null;

                Logger.Log(MethodBase.GetCurrentMethod(), ex);
            }

            return itemList;
        }

        public static List<Models.Shared.Organization> GetAllOrganizations()
        {
            var organizations = new List<Models.Shared.Organization>();
            try
            {
                using (CFContext context = new CFContext())
                {
                    organizations = context.Organizations.Select(x => new Models.Shared.Organization
                    {
                        OrganizationId = x.OrganizationId,
                        ParentId = x.ParentId,
                        OId = x.OId,
                        Name = x.Name
                    }).ToList();
                }
            }
            catch (Exception e)
            {
                organizations = new List<Models.Shared.Organization>();
                Logger.Log(MethodBase.GetCurrentMethod(), e);
            }

            return organizations;
        }

        public static OrganizationModel GetOrganization(Guid parentOrganizationId)
        {
            OrganizationModel organizationModel = null;

            try
            {
                using (CFContext db = new CFContext())
                {
                    var query = db.Organizations.Include("Manager").First(x => x.OrganizationId == parentOrganizationId);

                    organizationModel = new OrganizationModel()
                    {
                        AncestorOrganizationId = GetAncestorOrganizationId(query.OrganizationId),
                        OrganizationId = query.OrganizationId,
                        OId = query.OId,
                        Name = query.Name,
                        FullName = GetOrganizationFullName(query.OrganizationId),
                        //ManagerId = query.Manager.ManagerId
                    };
                }
            }
            catch (Exception ex)
            {
                organizationModel = null;

                Logger.Log(MethodBase.GetCurrentMethod(), ex);
            }

            return organizationModel;
        }

        //private static string GetOrganizationFullName(Guid organizationId)
        //{
        //    string name = string.Empty;

        //    try
        //    {
        //        if (organizationId == new Guid())
        //        {
        //            name = "*";
        //        }
        //        else
        //        {
        //            using (CFContext context = new CFContext())
        //            {
        //                var organization = context.Organizations.First(o => o.OrganizationId == organizationId);
        //                name = organization.Name;

        //                while (organization.ParentId.ToString() != "00000000-0000-0000-0000-000000000000")
        //                {
        //                    organization = context.Organizations.First(o => o.OrganizationId == organization.ParentId);
        //                    name = organization.Name + "->" + name;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        name = string.Empty;
        //        Logger.Log(MethodBase.GetCurrentMethod(), e);
        //        throw;
        //    }

        //    return name;
        //}

        public static List<OrganizationPermission> GetOrganizationPermissions(Guid organizationId)
        {
            var permissions = new List<OrganizationPermission>();
            try
            {
                var upStreamOrganizationIds = GetUpStreamOrganizationIds(organizationId, false);
                permissions.AddRange(upStreamOrganizationIds.Select(x => new OrganizationPermission
                {
                    OrganizationId = x,
                    Permission = Define.EnumOrganizationPermission.Visible
                }).ToList());

                var downStreamOrganizations = GetDownStreamOrganizationIds(organizationId, true);
                permissions.AddRange(downStreamOrganizations.Select(x => new OrganizationPermission
                {
                    OrganizationId = x,
                    Permission = Define.EnumOrganizationPermission.Editable
                }).ToList());
            }
            catch (Exception e)
            {
                permissions = null;
                Logger.Log(MethodBase.GetCurrentMethod(), e);
            }

            return permissions;
        }


        public static RequestResult GetTreeItem(List<Models.Shared.Organization> organizations, Guid organizationId, Account account)
        {
            RequestResult requestResult = new RequestResult();
            try
            {
                var treeItems = new List<TreeItem>();

                var attributes = new Dictionary<Define.EnumTreeAttribute, string>()
                {
                    { Define.EnumTreeAttribute.NodeType, Define.EnumTreeNodeType.Organization.ToString() },
                    { Define.EnumTreeAttribute.ToolTip, string.Empty },
                    { Define.EnumTreeAttribute.OrganizationPermission, string.Empty },
                    { Define.EnumTreeAttribute.OrganizationId, string.Empty }
                };

                var newOrganizations = organizations.Where(x => x.ParentId == organizationId && account.VisibleOrganizationIds.Contains(x.OrganizationId)).OrderBy(x => x.OId).ToList();

                foreach (var o in newOrganizations)
                {
                    var treeItem = new TreeItem() { Title = o.Name };
                    attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", o.OId, o.Name);
                    attributes[Define.EnumTreeAttribute.OrganizationPermission] = account.OrganizationPermission(o.OrganizationId).ToString();
                    attributes[Define.EnumTreeAttribute.OrganizationId] = o.OrganizationId.ToString();

                    foreach (var attribute in attributes)
                    {
                        treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                    }

                    if (organizations.Any(x => x.ParentId == o.OrganizationId && account.VisibleOrganizationIds.Contains(x.OrganizationId)))
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

        public static RequestResult GetRootTreeItem(List<Models.Shared.Organization> organizations, Guid ancestorOrganizationId, Account account)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                var treeItems = new List<TreeItem>();

                var attributes = new Dictionary<Define.EnumTreeAttribute, string>()
                {
                    { Define.EnumTreeAttribute.NodeType, Define.EnumTreeNodeType.Organization.ToString() },
                    { Define.EnumTreeAttribute.ToolTip, string.Empty },
                    { Define.EnumTreeAttribute.OrganizationPermission, string.Empty },
                    { Define.EnumTreeAttribute.OrganizationId, string.Empty }
                };

                using (CFContext context = new CFContext())
                {
                    var organization = organizations.First(x => x.OrganizationId == ancestorOrganizationId);
                    var treeItem = new TreeItem() { Title = organization.Name };
                    attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", organization.OrganizationId, organization.Name);
                    attributes[Define.EnumTreeAttribute.OrganizationPermission] = account.OrganizationPermission(organization.OrganizationId).ToString();
                    attributes[Define.EnumTreeAttribute.OrganizationId] = organization.OrganizationId.ToString();

                    foreach (var attribute in attributes)
                    {
                        treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                    }

                    if (organizations.Any(x => x.ParentId == organization.OrganizationId && account.VisibleOrganizationIds.Contains(x.OrganizationId)))
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

        public static RequestResult Create(CreateFormModel createFormModel)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                using (CFContext context = new CFContext())
                {
                    var exists = context.Organizations.FirstOrDefault(x => x.ParentId.ToString() == createFormModel.ParentId && x.OId == createFormModel.FormInput.OId);

                    if (exists == null)
                    {
                        //OrganizationId为Guid自动生成，此处先形成organization对象，为了下面获取数据库中自动生成的OrganizationId。
                        //var organization = new CF.Models.Organization()
                        //{                            
                        //    ParentId = createFormModel.ParentId,
                        //    OId = createFormModel.FormInput.OId,
                        //    Name = createFormModel.FormInput.Name,
                        //};
                        //context.Organizations.Add(organization);

                        var organization = new CF.Models.Organization()
                        {
                            OrganizationId = Guid.NewGuid(),
                            ParentId = new Guid(createFormModel.ParentId),
                            OId = createFormModel.FormInput.OId,
                            Name = createFormModel.FormInput.Name
                        };
                        context.Organizations.Add(organization);


                        if (!string.IsNullOrEmpty(createFormModel.FormInput.Managers))
                        {
                            context.OrganizationManagers.AddRange(createFormModel.FormInput.Managers.Split(',').ToList().Select(om => new OrganizationManager()
                            {
                                OrganizationId = organization.OrganizationId,
                                ManagerId = om
                            }));
                        }

                        context.EditableOrganizations.AddRange(createFormModel.EditableOrganizations.Where(x => x.CanDelete).Select(x => new EditableOrganization
                        {
                            OrganizationId = organization.OrganizationId,
                            EditableOrganizationId = x.OrganizationId
                        }).ToList());

                        context.QueryableOrganizations.AddRange(createFormModel.QueryableOrganizations.Select(x => new QueryableOrganization
                        {
                            OrganizationId = organization.OrganizationId,
                            QueryableOrganizationId = x.OrganizationId
                        }).ToList());

                        context.SaveChanges();

                        requestResult.ReturnData(organization.OrganizationId, string.Format("{0} {1} {2}", Resources.Resource.Create, Resources.Resource.Organization, Resources.Resource.Success));
                    }
                    else
                    {
                        requestResult.ReturnFailedMessage(string.Format("{0} {1}", Resources.Resource.OrganizationId, Resources.Resource.Exists));
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

        public static RequestResult GetQueryableOrganizationTreeItem(List<Models.Shared.Organization> organizations, Guid editableAncestorOrganizationId, Guid organizationId, Account account)
        {
            RequestResult result = new RequestResult();

            try
            {
                var treeItems = new List<TreeItem>();

                var attributes = new Dictionary<Define.EnumTreeAttribute, string>()
                {
                    { Define.EnumTreeAttribute.NodeType, Define.EnumTreeNodeType.Organization.ToString() },
                    { Define.EnumTreeAttribute.ToolTip, string.Empty },
                    { Define.EnumTreeAttribute.OrganizationId, string.Empty }
                };

                var editableOrganizationIds = new List<Guid>();

                if (editableAncestorOrganizationId != Guid.Empty)
                {
                    editableOrganizationIds = GetDownStreamOrganizationIds(editableAncestorOrganizationId, true);
                }

                var organizationList = organizations.Where(x => x.ParentId == organizationId && account.VisibleOrganizationIds.Contains(x.OrganizationId) && !editableOrganizationIds.Contains(x.OrganizationId)).OrderBy(x => x.OId).ToList();

                foreach (var organization in organizationList)
                {
                    var treeItem = new TreeItem() { Title = organization.Name };

                    attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", organization.OId, organization.Name);
                    attributes[Define.EnumTreeAttribute.OrganizationId] = organization.OrganizationId.ToString();

                    foreach (var attribute in attributes)
                    {
                        treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                    }

                    if (organizations.Any(x => x.ParentId == organization.OrganizationId && account.VisibleOrganizationIds.Contains(x.OrganizationId) && !editableOrganizationIds.Contains(x.OrganizationId)))
                    {
                        treeItem.State = "closed";
                    }

                    treeItems.Add(treeItem);
                }

                result.ReturnData(treeItems);
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return result;
        }

        public static RequestResult GetQueryableOrganizationRootTreeItem(List<Models.Shared.Organization> organizations, Guid editableAncestorOrganizationId, Guid ancestorOrganizationId, Account account)
        {
            RequestResult result = new RequestResult();

            try
            {
                var treeItemList = new List<TreeItem>();

                if (editableAncestorOrganizationId != Guid.Empty || (editableAncestorOrganizationId != Guid.Empty && editableAncestorOrganizationId != ancestorOrganizationId))
                {
                    var attributes = new Dictionary<Define.EnumTreeAttribute, string>()
                    {
                        { Define.EnumTreeAttribute.NodeType, Define.EnumTreeNodeType.Organization.ToString() },
                        { Define.EnumTreeAttribute.ToolTip, string.Empty },
                        { Define.EnumTreeAttribute.OrganizationId, string.Empty }
                    };

                    var editableOrganizations = new List<Guid>();

                    if (editableAncestorOrganizationId != Guid.Empty)
                    {
                        editableOrganizations = GetDownStreamOrganizationIds(editableAncestorOrganizationId, true);
                    }

                    var organization = organizations.First(x => x.OrganizationId == ancestorOrganizationId);

                    var treeItem = new TreeItem() { Title = organization.Name };

                    attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", organization.OId, organization.Name);
                    attributes[Define.EnumTreeAttribute.OrganizationId] = account.OrganizationPermission(organization.OrganizationId).ToString();
                    attributes[Define.EnumTreeAttribute.OrganizationId] = organization.OrganizationId.ToString();

                    foreach (var attribute in attributes)
                    {
                        treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                    }

                    if (organizations.Any(x => x.ParentId == organization.OrganizationId && account.VisibleOrganizationIds.Contains(x.OrganizationId) && !editableOrganizations.Contains(x.OrganizationId)))
                    {
                        treeItem.State = "closed";
                    }

                    treeItemList.Add(treeItem);
                }

                result.ReturnData(treeItemList);
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return result;
        }

        public static RequestResult AddEditableOrganization(List<EditableOrganizationModel> editableOrganizations, List<Guid> selectedOrganizations, Account account)
        {
            RequestResult result = new RequestResult();

            try
            {
                using (CFContext db = new CFContext())
                {
                    foreach (Guid organizationId in selectedOrganizations)
                    {
                        var downStreamOrganizationList = GetDownStreamOrganizationIds(organizationId, true);

                        foreach (var downStreamOrganization in downStreamOrganizationList)
                        {
                            if (!editableOrganizations.Any(x => x.OrganizationId == downStreamOrganization))
                            {
                                editableOrganizations.Add(new EditableOrganizationModel()
                                {
                                    CanDelete = true,
                                    OrganizationId = downStreamOrganization,
                                    FullName = GetOrganizationFullName(downStreamOrganization)
                                });
                            }
                        }
                    }
                }

                result.ReturnData(editableOrganizations.OrderBy(x => x.FullName).ToList());
            }
            catch (Exception ex)
            {
                var err = new Error(MethodInfo.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return result;
        }

        public static RequestResult AddQueryableOrganization(List<QueryableOrganizationModel> queryableOrganizations, List<Guid> selectedOrganizations, Account Account)
        {
            RequestResult result = new RequestResult();

            try
            {
                using (CFContext db = new CFContext())
                {
                    foreach (Guid organizationId in selectedOrganizations)
                    {
                        var downStreamOrganizationList = GetDownStreamOrganizationIds(organizationId, true);

                        foreach (var downStreamOrganization in downStreamOrganizationList)
                        {
                            if (!queryableOrganizations.Any(x => x.OrganizationId == downStreamOrganization))
                            {
                                queryableOrganizations.Add(new QueryableOrganizationModel()
                                {
                                    OrganizationId = downStreamOrganization,
                                    FullName = GetOrganizationFullName(downStreamOrganization)
                                });
                            }
                        }
                    }
                }

                result.ReturnData(queryableOrganizations.OrderBy(x => x.FullName).ToList());
            }
            catch (Exception ex)
            {
                var err = new Error(MethodInfo.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return result;
        }

        public static RequestResult GetEditableOrganizationRootTreeItem(List<Models.Shared.Organization> organizations, Guid editableAncestorOrganizationId, Guid ancestorOrganizationId, Account account)
        {
            RequestResult result = new RequestResult();

            try
            {
                var treeItemList = new List<TreeItem>();

                if (editableAncestorOrganizationId != Guid.Empty || (editableAncestorOrganizationId != Guid.Empty && editableAncestorOrganizationId != ancestorOrganizationId))
                {
                    var attributes = new Dictionary<Define.EnumTreeAttribute, string>()
                    {
                        { Define.EnumTreeAttribute.NodeType, Define.EnumTreeNodeType.Organization.ToString() },
                        { Define.EnumTreeAttribute.ToolTip, string.Empty },
                        { Define.EnumTreeAttribute.OrganizationId, string.Empty }
                    };

                    var editableOrganizations = new List<Guid>();

                    if (editableAncestorOrganizationId != Guid.Empty)
                    {
                        editableOrganizations = GetDownStreamOrganizationIds(editableAncestorOrganizationId, true);
                    }

                    var organization = organizations.First(x => x.OrganizationId == ancestorOrganizationId);

                    var treeItem = new TreeItem() { Title = organization.Name };

                    attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", organization.OId, organization.Name);
                    attributes[Define.EnumTreeAttribute.OrganizationId] = account.OrganizationPermission(organization.OrganizationId).ToString();
                    attributes[Define.EnumTreeAttribute.OrganizationId] = organization.OrganizationId.ToString();

                    foreach (var attribute in attributes)
                    {
                        treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                    }

                    if (organizations.Any(x => x.ParentId == organization.OrganizationId && account.VisibleOrganizationIds.Contains(x.OrganizationId) && !editableOrganizations.Contains(x.OrganizationId)))
                    {
                        treeItem.State = "closed";
                    }

                    treeItemList.Add(treeItem);
                }

                result.ReturnData(treeItemList);
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return result;
        }

        public static RequestResult GetEditableOrganizationTreeItem(List<Models.Shared.Organization> organizations, Guid editableAncestorOrganizationId, Guid organizationId, Account account)
        {
            RequestResult result = new RequestResult();

            try
            {
                var treeItemList = new List<TreeItem>();

                var attributes = new Dictionary<Define.EnumTreeAttribute, string>()
                {
                    { Define.EnumTreeAttribute.NodeType, Define.EnumTreeNodeType.Organization.ToString() },
                    { Define.EnumTreeAttribute.ToolTip, string.Empty },
                    { Define.EnumTreeAttribute.OrganizationId, string.Empty }
                };

                var editableOrganizations = new List<Guid>();

                if (editableAncestorOrganizationId != Guid.Empty)
                {
                    editableOrganizations = GetDownStreamOrganizationIds(editableAncestorOrganizationId, true);
                }

                var organizationList = organizations.Where(x => x.ParentId == organizationId && account.VisibleOrganizationIds.Contains(x.OrganizationId) && !editableOrganizations.Contains(x.OrganizationId)).OrderBy(x => x.OId).ToList();

                foreach (var organization in organizationList)
                {
                    var treeItem = new TreeItem() { Title = organization.Name };

                    attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", organization.OId, organization.Name);
                    attributes[Define.EnumTreeAttribute.OrganizationId] = organization.OrganizationId.ToString();

                    foreach (var attribute in attributes)
                    {
                        treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                    }

                    if (organizations.Any(x => x.ParentId == organization.OrganizationId && account.VisibleOrganizationIds.Contains(x.OrganizationId) && !editableOrganizations.Contains(x.OrganizationId)))
                    {
                        treeItem.State = "closed";
                    }

                    treeItemList.Add(treeItem);
                }

                result.ReturnData(treeItemList);
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return result;
        }
    }
}
