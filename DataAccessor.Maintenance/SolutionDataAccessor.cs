using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using CF;
using Models.Authentication;
using Models.Maintenance.Solution;
using Models.Shared;
using Utility;
using Utility.Models;

namespace DataAccessor.Maintenance
{
    public class SolutionDataAccessor
    {
        public static RequestResult GetTreeItems(List<Organization> organizationList, Guid organizationId, string solutionType, Account account)
        {
            RequestResult result = new RequestResult();

            try
            {
                var treeItemList = new List<TreeItem>();

                var attributes = new Dictionary<Define.EnumTreeAttribute, string>()
                {
                    { Define.EnumTreeAttribute.NodeType, string.Empty },
                    { Define.EnumTreeAttribute.ToolTip, string.Empty },
                    { Define.EnumTreeAttribute.OrganizationId, string.Empty },
                    { Define.EnumTreeAttribute.SolutionType, string.Empty },
                    { Define.EnumTreeAttribute.SolutionId, string.Empty }
                };

                using (CFContext context=new CFContext())
                {
                    if (string.IsNullOrEmpty(solutionType))
                    {
                        if (account.QueryableOrganizationIds.Contains(organizationId))
                        {
                            var solutionTypes = context.Solutions.Where(x => x.OrganizationId == organizationId).Select(x => x.Type).Distinct().OrderBy(x => x).ToList();

                            foreach (var type in solutionTypes)
                            {
                                var treeItem = new TreeItem() { Title = type };

                                attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.SolutionType.ToString();
                                attributes[Define.EnumTreeAttribute.ToolTip] = type;
                                attributes[Define.EnumTreeAttribute.OrganizationId] = organizationId.ToString();
                                attributes[Define.EnumTreeAttribute.SolutionType] = type;
                                attributes[Define.EnumTreeAttribute.SolutionId] = string.Empty;

                                foreach (var attribute in attributes)
                                {
                                    treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                                }

                                treeItem.State = "closed";

                                treeItemList.Add(treeItem);
                            }
                        }

                        var newOrganizations = organizationList.Where(x => x.ParentId == organizationId && account.VisibleOrganizationIds.Contains(x.OrganizationId)).OrderBy(x => x.OId).ToList();

                        foreach (var organization in newOrganizations)
                        {
                            var treeItem = new TreeItem() { Title = organization.Name };

                            attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.Organization.ToString();
                            attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", organization.OId, organization.Name);
                            attributes[Define.EnumTreeAttribute.OrganizationId] = organization.OrganizationId.ToString();
                            attributes[Define.EnumTreeAttribute.SolutionType] = string.Empty;
                            attributes[Define.EnumTreeAttribute.SolutionId] = string.Empty;

                            foreach (var attribute in attributes)
                            {
                                treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                            }

                            if (organizationList.Any(x => x.ParentId == organization.OrganizationId && account.VisibleOrganizationIds.Contains(x.OrganizationId))
                                ||
                                (account.QueryableOrganizationIds.Contains(organization.OrganizationId) && context.Solutions.Any(x => x.OrganizationId == organization.OrganizationId)))
                            {
                                treeItem.State = "closed";
                            }

                            treeItemList.Add(treeItem);
                        }
                    }
                    else
                    {
                        var solutions = context.Solutions.Where(x => x.OrganizationId == organizationId && x.Type == solutionType).OrderBy(x => x.SId).ToList();

                        foreach (var solution in solutions)
                        {
                            var treeItem = new TreeItem() { Title = solution.Name };

                            attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.Solution.ToString();
                            attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", solution.SId, solution.Name);
                            attributes[Define.EnumTreeAttribute.OrganizationId] = organizationId.ToString();
                            attributes[Define.EnumTreeAttribute.SolutionType] = solution.Type;
                            attributes[Define.EnumTreeAttribute.SolutionId] = solution.SolutionId.ToString();

                            foreach (var attribute in attributes)
                            {
                                treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                            }

                            treeItemList.Add(treeItem);
                        }
                    }
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

        public static RequestResult GetTreeItems(List<Models.Shared.Organization> organizations, Guid refOrganizationId, Guid organizationId, string type)
        {
            RequestResult result = new RequestResult();

            try
            {
                var treeItemList = new List<TreeItem>();

                var attributes = new Dictionary<Define.EnumTreeAttribute, string>()
                {
                    { Define.EnumTreeAttribute.NodeType, string.Empty },
                    { Define.EnumTreeAttribute.ToolTip, string.Empty },
                    { Define.EnumTreeAttribute.OrganizationId, string.Empty },
                    { Define.EnumTreeAttribute.SolutionType, string.Empty },
                    { Define.EnumTreeAttribute.SolutionId, string.Empty }
                };

                using (CFContext context = new CFContext())
                {
                    if (string.IsNullOrEmpty(type))
                    {
                        var solutionTypes = context.Solutions.Where(x => x.OrganizationId == organizationId).Select(x => x.Type).Distinct().OrderBy(x => x).ToList();

                        foreach (var solutionType in solutionTypes)
                        {
                            var treeItem = new TreeItem() { Title = solutionType };

                            attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.SolutionType.ToString();
                            attributes[Define.EnumTreeAttribute.ToolTip] = solutionType;
                            attributes[Define.EnumTreeAttribute.OrganizationId] = organizationId.ToString();
                            attributes[Define.EnumTreeAttribute.SolutionType] = solutionType;
                            attributes[Define.EnumTreeAttribute.SolutionId] = string.Empty;

                            foreach (var attribute in attributes)
                            {
                                treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                            }

                            treeItem.State = "closed";

                            treeItemList.Add(treeItem);
                        }

                        var availableOrganizationIds = OrganizationDataAccessor.GetUpStreamOrganizationIds(refOrganizationId, true);

                        var newOrganizations = organizations.Where(x => x.ParentId == organizationId && availableOrganizationIds.Contains(x.OrganizationId)).OrderBy(x => x.OId).ToList();

                        foreach (var organization in newOrganizations)
                        {
                            var downStreamOrganizationIds = OrganizationDataAccessor.GetDownStreamOrganizationIds(organization.OrganizationId, true);

                            if (context.Solutions.Any(x => downStreamOrganizationIds.Contains(x.OrganizationId) && availableOrganizationIds.Contains(x.OrganizationId)))
                            {
                                var treeItem = new TreeItem() { Title = organization.Name };

                                attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.Organization.ToString();
                                attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", organization.OId, organization.Name);
                                attributes[Define.EnumTreeAttribute.OrganizationId] = organization.OrganizationId.ToString();
                                attributes[Define.EnumTreeAttribute.SolutionType] = string.Empty;
                                attributes[Define.EnumTreeAttribute.SolutionId] = string.Empty;

                                foreach (var attribute in attributes)
                                {
                                    treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                                }

                                treeItem.State = "closed";

                                treeItemList.Add(treeItem);
                            }
                        }
                    }
                    else
                    {
                        var solutions = context.Solutions.Where(x => x.OrganizationId == organizationId && x.Type == type).OrderBy(x => x.SId).ToList();

                        foreach (var solution in solutions)
                        {
                            var treeItem = new TreeItem() { Title = solution.Name };

                            attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.Solution.ToString();
                            attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", solution.SId, solution.Name);
                            attributes[Define.EnumTreeAttribute.OrganizationId] = solution.OrganizationId.ToString();
                            attributes[Define.EnumTreeAttribute.SolutionType] = solution.Type;
                            attributes[Define.EnumTreeAttribute.SolutionId] = solution.SolutionId.ToString();

                            foreach (var attribute in attributes)
                            {
                                treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                            }

                            treeItemList.Add(treeItem);
                        }
                    }
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

        public static RequestResult Delete(List<string> selectedList)
        {
            RequestResult result = new RequestResult();

            try
            {
                using (CFContext context=new CFContext())
                {
                    DeleteHelper.DeleteSolutions(context, selectedList);

                    context.SaveChanges();
                }

                result.ReturnSuccessMessage(string.Format("{0} {1} {2}", Resources.Resource.Delete, Resources.Resource.Solution, Resources.Resource.Success));
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return result;
        }

        public static object Edit(EditFormModel editFormModel)
        {
            RequestResult result = new RequestResult();

            try
            {
                if (editFormModel.FormInput.Type == Define.Other || editFormModel.FormInput.Type == Define.New)
                {
                    result.ReturnFailedMessage(string.Format("{0} {1}", Resources.Resource.Unsupported, Resources.Resource.SolutionType));
                }
                else
                {
                    using (CFContext context=new CFContext())
                    {
                        var solution = context.Solutions.First(x => x.SolutionId == new Guid(editFormModel.SolutionId));

                        var exists = context.Solutions.FirstOrDefault(x => x.SolutionId != solution.SolutionId && x.OrganizationId == solution.OrganizationId && x.Type == editFormModel.FormInput.Type && x.SId == editFormModel.FormInput.SId);

                        if (exists == null)
                        {
#if !DEBUG
                    using (TransactionScope trans = new TransactionScope())
                    {
#endif
                            solution.Type = editFormModel.FormInput.Type;
                            solution.SId = editFormModel.FormInput.SId;
                            solution.Name = editFormModel.FormInput.Name;
                            solution.LastModifyTime = DateTime.Now;

                            context.SaveChanges();
#if !DEBUG
                        trans.Complete();
                    }
#endif
                            result.ReturnSuccessMessage(string.Format("{0} {1} {2}", Resources.Resource.Edit, Resources.Resource.Solution, Resources.Resource.Success));
                        }
                        else
                        {
                            result.ReturnFailedMessage(string.Format("{0} {1}", Resources.Resource.SId, Resources.Resource.Exists));
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

            return result;
        }

        public static RequestResult GetEditFormModel(string solutionId)
        {
            RequestResult result = new RequestResult();

            try
            {
                using (CFContext context=new CFContext())
                {
                    var solution = context.Solutions.First(x => x.SolutionId == new Guid(solutionId));

                    var model = new EditFormModel()
                    {
                        SolutionId = solution.SolutionId.ToString(),
                        OrganizationId = solution.OrganizationId.ToString(),
                        ParentOrganizationFullName = OrganizationDataAccessor.GetOrganizationFullName(solution.OrganizationId),
                        SolutionTypes = new List<SelectListItem>()
                        {
                            Define.DefaultSelectListItem(Resources.Resource.Select),
                            new SelectListItem()
                            {
                                Text = Resources.Resource.Create + "...",
                                Value = Define.New
                            }
                        },
                        FormInput = new FormInput()
                        {
                            Type = solution.Type,
                            SId = solution.SId,
                            Name = solution.Name
                        }
                    };

                    var upStreamOrganizationIds = OrganizationDataAccessor.GetUpStreamOrganizationIds(new Guid(model.OrganizationId), true);

                    model.SolutionTypes.AddRange(context.Solutions.Where(x => upStreamOrganizationIds.Contains(x.OrganizationId)).Select(x => x.Type).Distinct().OrderBy(x => x).Select(x => new SelectListItem
                    {
                        Value = x,
                        Text = x
                    }).ToList());

                    if (!string.IsNullOrEmpty(model.FormInput.Type) && model.SolutionTypes.Any(x => x.Value == model.FormInput.Type))
                    {
                        model.SolutionTypes.First(x => x.Value == model.FormInput.Type).Selected = true;
                    }

                    result.ReturnData(model);
                }
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return result;
        }

        public static object Create(CreateFormModel createFormModel)
        {
            RequestResult result = new RequestResult();

            try
            {
                if (createFormModel.FormInput.Type == Define.Other || createFormModel.FormInput.Type == Define.New)
                {
                    result.ReturnFailedMessage(string.Format("{0} {1}", Resources.Resource.Unsupported, Resources.Resource.SolutionType));
                }
                else
                {
                    using (CFContext context=new CFContext())
                    {
                        var exists = context.Solutions.FirstOrDefault(x => x.OrganizationId == new Guid(createFormModel.OrganizationId) && x.Type == createFormModel.FormInput.Type && x.SId == createFormModel.FormInput.SId);

                        if (exists == null)
                        {
                            Guid solutionId = Guid.NewGuid();

                            context.Solutions.Add(new CF.Models.Maintenance.Solution()
                            {
                                SolutionId = solutionId,
                                OrganizationId = new Guid(createFormModel.OrganizationId),
                                Type = createFormModel.FormInput.Type,
                                SId = createFormModel.FormInput.SId,
                                Name = createFormModel.FormInput.Name,
                                LastModifyTime = DateTime.Now
                            });

                            context.SaveChanges();

                            result.ReturnData(solutionId, string.Format("{0} {1} {2}", Resources.Resource.Create, Resources.Resource.Solution, Resources.Resource.Success));
                        }
                        else
                        {
                            result.ReturnFailedMessage(string.Format("{0} {1}", Resources.Resource.SId, Resources.Resource.Exists));
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

            return result;
        }

        public static RequestResult GetCreateFormModel(string organizationId, string solutionType)
        {
            RequestResult result = new RequestResult();

            try
            {
                using (CFContext context=new CFContext())
                {
                    var model = new CreateFormModel()
                    {
                        OrganizationId = organizationId,
                        ParentOrganizationFullName = OrganizationDataAccessor.GetOrganizationFullName(new Guid(organizationId)),
                        SolutionTypes = new List<SelectListItem>()
                        {
                            Define.DefaultSelectListItem(Resources.Resource.Select),
                            new SelectListItem()
                            {
                                Text = Resources.Resource.Create + "...",
                                Value = Define.New
                            }
                        },
                        FormInput = new FormInput()
                        {
                            Type = solutionType
                        }
                    };

                    var upStreamOrganizationIds = OrganizationDataAccessor.GetUpStreamOrganizationIds(new Guid(organizationId), true);

                    model.SolutionTypes.AddRange(context.Solutions.Where(x => upStreamOrganizationIds.Contains(x.OrganizationId)).Select(x => x.Type).Distinct().OrderBy(x => x).Select(x => new SelectListItem
                    {
                        Value = x,
                        Text = x
                    }).ToList());

                    if (!string.IsNullOrEmpty(solutionType) && model.SolutionTypes.Any(x => x.Value == solutionType))
                    {
                        model.SolutionTypes.First(x => x.Value == solutionType).Selected = true;
                    }

                    result.ReturnData(model);
                }
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return result;
        }

        public static RequestResult GetDetailViewModel(string solutionId, Account account)
        {
            RequestResult result = new RequestResult();

            try
            {
                using (CFContext context=new CFContext())
                {
                    var solution = context.Solutions.First(x => x.SolutionId == new Guid(solutionId));

                    result.ReturnData(new DetailViewModel()
                    {
                        SolutionId = solution.SolutionId.ToString(),
                        Permission = account.OrganizationPermission(solution.OrganizationId),
                        OrganizationId = solution.OrganizationId.ToString(),
                        ParentOrganizationFullName = OrganizationDataAccessor.GetOrganizationFullName(solution.OrganizationId),
                        SolutionType = solution.Type,
                        SId = solution.SId,
                        Name = solution.Name
                    });
                }
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return result;
        }

        public static RequestResult Query(QueryParameters queryParameters, Account account)
        {
            RequestResult result = new RequestResult();

            try
            {
                using (CFContext context=new CFContext())
                {
                    var downStreamOrganizationIds = OrganizationDataAccessor.GetDownStreamOrganizationIds(new Guid(queryParameters.OrganizationId), true);

                    var query = context.Solutions.Where(x => downStreamOrganizationIds.Contains(x.OrganizationId) && account.QueryableOrganizationIds.Contains(x.OrganizationId)).AsQueryable();

                    if (!string.IsNullOrEmpty(queryParameters.SolutionType))
                    {
                        query = query.Where(x => x.OrganizationId == new Guid(queryParameters.OrganizationId) && x.Type == queryParameters.SolutionType);
                    }

                    if (!string.IsNullOrEmpty(queryParameters.Keyword))
                    {
                        query = query.Where(x => x.SId.Contains(queryParameters.Keyword) || x.Name.Contains(queryParameters.Keyword));
                    }

                    var organization = OrganizationDataAccessor.GetOrganization(new Guid(queryParameters.OrganizationId));

                    result.ReturnData(new GridViewModel()
                    {
                        OrganizationId = queryParameters.OrganizationId,
                        Permission = account.OrganizationPermission(new Guid(queryParameters.OrganizationId)),
                        SolutionType = queryParameters.SolutionType,
                        OrganizationName = organization.Name,
                        FullOrganizationName = organization.FullName,
                        Items = query.ToList().Select(x => new GridItem()
                        {
                            SolutionId = x.SolutionId.ToString(),
                            Permission = account.OrganizationPermission(x.OrganizationId),
                            OrganizationName = OrganizationDataAccessor.GetOrganizationName(x.OrganizationId),
                            SolutionType = x.Type,
                            SId = x.SId,
                            Name = x.Name
                        }).OrderBy(x => x.OrganizationName).ThenBy(x => x.SolutionType).ThenBy(x => x.SId).ToList()
                    });
                }
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return result;
        }

        public static RequestResult GetRootTreeItems(List<Organization> organizationList, Guid organizationId, Account account)
        {
            RequestResult result = new RequestResult();

            try
            {
                var treeItemList = new List<TreeItem>();

                var attributes = new Dictionary<Define.EnumTreeAttribute, string>()
                {
                    { Define.EnumTreeAttribute.NodeType, string.Empty },
                    { Define.EnumTreeAttribute.ToolTip, string.Empty },
                    { Define.EnumTreeAttribute.OrganizationId, string.Empty },
                    { Define.EnumTreeAttribute.SolutionType, string.Empty },
                    { Define.EnumTreeAttribute.SolutionId, string.Empty }
                };

                using (CFContext context=new CFContext())
                {
                    var organization = organizationList.First(x => x.OrganizationId == organizationId);

                    var treeItem = new TreeItem() { Title = organization.Name };

                    attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.Organization.ToString();
                    attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", organization.OId, organization.Name);
                    attributes[Define.EnumTreeAttribute.OrganizationId] = organization.OrganizationId.ToString();
                    attributes[Define.EnumTreeAttribute.SolutionType] = string.Empty;
                    attributes[Define.EnumTreeAttribute.SolutionId] = string.Empty;

                    foreach (var attribute in attributes)
                    {
                        treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                    }

                    if (organizationList.Any(x => x.ParentId == organization.OrganizationId && account.VisibleOrganizationIds.Contains(x.OrganizationId))
                        ||
                        (account.QueryableOrganizationIds.Contains(organization.OrganizationId) && context.Solutions.Any(x => x.OrganizationId == organization.OrganizationId)))
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
