using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using CF;
using Models.Authentication;
using Models.Maintenance.AbnormalReason;
using Models.Shared;
using Utility;
using Utility.Models;

namespace DataAccessor.Maintenance
{
    public class AbnormalReasonDataAccessor
    {
        public static RequestResult Query(QueryParameters queryParameters, Account account)
        {
            RequestResult result = new RequestResult();

            try
            {
                using (CFContext context=new CFContext())
                {
                    var downStreamOrganizationIds = OrganizationDataAccessor.GetDownStreamOrganizationIds(new Guid(queryParameters.OrganizationId), true);

                    var query = context.AbnormalReasons.Where(x => downStreamOrganizationIds.Contains(x.OrganizationId) && account.QueryableOrganizationIds.Contains(x.OrganizationId)).AsQueryable();

                    if (!string.IsNullOrEmpty(queryParameters.Type))
                    {
                        query = query.Where(x => x.OrganizationId == new Guid(queryParameters.OrganizationId) && x.Type == queryParameters.Type);
                    }

                    if (!string.IsNullOrEmpty(queryParameters.Keyword))
                    {
                        query = query.Where(x => x.ARId.Contains(queryParameters.Keyword) || x.Name.Contains(queryParameters.Keyword));
                    }

                    var organization = OrganizationDataAccessor.GetOrganization(new Guid(queryParameters.OrganizationId));

                    result.ReturnData(new GridViewModel()
                    {
                        OrganizationId = queryParameters.OrganizationId,
                        Permission = account.OrganizationPermission(new Guid(queryParameters.OrganizationId)),
                        Type = queryParameters.Type,
                        OrganizationName = organization.Name,
                        FullOrganizationName = organization.FullName,
                        GridItems = query.ToList().Select(x => new GridItem()
                        {
                            AbnormalReasonId = x.AbnormalReasonId.ToString(),
                            Permission = account.OrganizationPermission(x.OrganizationId),
                            OrganizationName = OrganizationDataAccessor.GetOrganizationName(x.OrganizationId),
                            Type = x.Type,
                            ARId = x.ARId,
                            Name = x.Name
                        }).OrderBy(x => x.OrganizationName).ThenBy(x => x.Type).ThenBy(x => x.ARId).ToList()
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

        public static RequestResult GetCopyFormModel(string abnormalReasonId)
        {
            RequestResult result = new RequestResult();

            try
            {
                using (CFContext context=new CFContext())
                {
                    var abnormalReason = context.AbnormalReasons.Include("Solutions").First(x => x.AbnormalReasonId == new Guid(abnormalReasonId));

                    var model = new CreateFormModel()
                    {
                        OrganizationId = abnormalReason.OrganizationId.ToString(),
                        ParentOrganizationFullName = OrganizationDataAccessor.GetOrganizationFullName(abnormalReason.OrganizationId),
                        Types = new List<SelectListItem>()
                        {
                            Define.DefaultSelectListItem(Resources.Resource.Select),
                            new SelectListItem()
                            {
                                Text = Resources.Resource.Create + "...",
                                Value = Define.New
                            }
                        },
                        SolutionModels = abnormalReason.Solutions.Select(x => new SolutionModel
                        {
                            SolutionId = x.SolutionId.ToString(),
                            Type = x.Type,
                            SId = x.SId,
                            Name = x.Name,
                        }).OrderBy(x => x.Type).ThenBy(x => x.SId).ToList(),                        
                        FormInput = new FormInput()
                        {
                            Type = abnormalReason.Type
                        }
                    };

                    var upStreamOrganizationIds = OrganizationDataAccessor.GetUpStreamOrganizationIds(abnormalReason.OrganizationId, true);

                    model.Types.AddRange(context.AbnormalReasons.Where(x => upStreamOrganizationIds.Contains(x.OrganizationId)).Select(x => x.Type).Distinct().OrderBy(x => x).Select(x => new SelectListItem
                    {
                        Value = x,
                        Text = x
                    }).ToList());

                    model.Types.First(x => x.Value == abnormalReason.Type).Selected = true;

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

        public static RequestResult Delete(List<string> selectedList)
        {
            RequestResult result = new RequestResult();

            try
            {
#if !DEBUG
                using (TransactionScope trans = new TransactionScope())
                {
#endif
                using (CFContext context=new CFContext())
                {
                    DeleteHelper.DeleteAbnormalReason(context, selectedList);

                    context.SaveChanges();
                }

#if !DEBUG
                    trans.Complete();
                }
#endif
                result.ReturnSuccessMessage(string.Format("{0} {1} {2}", Resources.Resource.Delete, Resources.Resource.AbnormalReason, Resources.Resource.Success));
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return result;
        }

        public static RequestResult GetRootTreeItems(List<Organization> organizations, Guid rootOrganizationId, Account account)
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
                    { Define.EnumTreeAttribute.AbnormalType, string.Empty },
                    { Define.EnumTreeAttribute.AbnormalReasonId, string.Empty }
                };

                using (CFContext context=new CFContext())
                {
                    var organization = organizations.First(x => x.OrganizationId == rootOrganizationId);

                    var treeItem = new TreeItem() { Title = organization.Name };

                    attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.Organization.ToString();
                    attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", organization.OId, organization.Name);
                    attributes[Define.EnumTreeAttribute.OrganizationId] = organization.OrganizationId.ToString();
                    attributes[Define.EnumTreeAttribute.AbnormalType] = string.Empty;
                    attributes[Define.EnumTreeAttribute.AbnormalReasonId] = string.Empty;

                    foreach (var attribute in attributes)
                    {
                        treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                    }

                    if (organizations.Any(x => x.ParentId == organization.OrganizationId && account.VisibleOrganizationIds.Contains(x.OrganizationId))
                        ||
                        (account.QueryableOrganizationIds.Contains(organization.OrganizationId) && context.AbnormalReasons.Any(x => x.OrganizationId == organization.OrganizationId)))
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

        public static RequestResult GetTreeItems(List<Organization> organizations, Guid rootOrganizationId, string type, Account account)
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
                    { Define.EnumTreeAttribute.AbnormalType, string.Empty },
                    { Define.EnumTreeAttribute.AbnormalReasonId, string.Empty }
                };

                using (CFContext context=new CFContext())
                {
                    if (string.IsNullOrEmpty(type))
                    {
                        if (account.QueryableOrganizationIds.Contains(rootOrganizationId))
                        {
                            var abnormalReasonTypes = context.AbnormalReasons.Where(x => x.OrganizationId == rootOrganizationId).Select(x => x.Type).Distinct().OrderBy(x => x).ToList();

                            foreach (var abnormalReasonType in abnormalReasonTypes)
                            {
                                var treeItem = new TreeItem() { Title = abnormalReasonType };

                                attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.AbnormalType.ToString();
                                attributes[Define.EnumTreeAttribute.ToolTip] = abnormalReasonType;
                                attributes[Define.EnumTreeAttribute.OrganizationId] = rootOrganizationId.ToString();
                                attributes[Define.EnumTreeAttribute.AbnormalType] = abnormalReasonType;
                                attributes[Define.EnumTreeAttribute.AbnormalReasonId] = string.Empty;

                                foreach (var attribute in attributes)
                                {
                                    treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                                }

                                treeItem.State = "closed";

                                treeItemList.Add(treeItem);
                            }
                        }

                        var newOrganizations = organizations.Where(x => x.ParentId == rootOrganizationId && account.VisibleOrganizationIds.Contains(x.OrganizationId)).OrderBy(x => x.OId).ToList();

                        foreach (var organization in newOrganizations)
                        {
                            var treeItem = new TreeItem() { Title = organization.Name };

                            attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.Organization.ToString();
                            attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", organization.OId, organization.Name);
                            attributes[Define.EnumTreeAttribute.OrganizationId] = organization.OrganizationId.ToString();
                            attributes[Define.EnumTreeAttribute.AbnormalType] = string.Empty;
                            attributes[Define.EnumTreeAttribute.AbnormalReasonId] = string.Empty;

                            foreach (var attribute in attributes)
                            {
                                treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                            }

                            if (organizations.Any(x => x.ParentId == organization.OrganizationId && account.VisibleOrganizationIds.Contains(x.OrganizationId))
                                ||
                                (account.QueryableOrganizationIds.Contains(organization.OrganizationId) && context.AbnormalReasons.Any(x => x.OrganizationId == organization.OrganizationId)))
                            {
                                treeItem.State = "closed";
                            }

                            treeItemList.Add(treeItem);
                        }
                    }
                    else
                    {
                        var abnormalReasons = context.AbnormalReasons.Where(x => x.OrganizationId == rootOrganizationId && x.Type == type).OrderBy(x => x.ARId).ToList();

                        foreach (var abnormalReason in abnormalReasons)
                        {
                            var treeItem = new TreeItem() { Title = abnormalReason.Name };

                            attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.AbnormalReason.ToString();
                            attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", abnormalReason.ARId, abnormalReason.Name);
                            attributes[Define.EnumTreeAttribute.OrganizationId] = rootOrganizationId.ToString();
                            attributes[Define.EnumTreeAttribute.AbnormalType] = abnormalReason.Type;
                            attributes[Define.EnumTreeAttribute.AbnormalReasonId] = abnormalReason.AbnormalReasonId.ToString();

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
                    { Define.EnumTreeAttribute.AbnormalType, string.Empty },
                    { Define.EnumTreeAttribute.AbnormalReasonId, string.Empty }
                };

                using (CFContext context=new CFContext())
                {
                    if (string.IsNullOrEmpty(type))
                    {
                        var abnormalReasonTypes = context.AbnormalReasons.Where(x => x.OrganizationId == organizationId).Select(x => x.Type).Distinct().OrderBy(x => x).ToList();

                        foreach (var abnormalReasonType in abnormalReasonTypes)
                        {
                            var treeItem = new TreeItem() { Title = abnormalReasonType };

                            attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.AbnormalType.ToString();
                            attributes[Define.EnumTreeAttribute.ToolTip] = abnormalReasonType;
                            attributes[Define.EnumTreeAttribute.OrganizationId] = organizationId.ToString();
                            attributes[Define.EnumTreeAttribute.AbnormalType] = abnormalReasonType;
                            attributes[Define.EnumTreeAttribute.AbnormalReasonId] = string.Empty;

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

                            if (context.AbnormalReasons.Any(x => downStreamOrganizationIds.Contains(x.OrganizationId) && availableOrganizationIds.Contains(x.OrganizationId)))
                            {
                                var treeItem = new TreeItem() { Title = organization.Name };

                                attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.Organization.ToString();
                                attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", organization.OId, organization.Name);
                                attributes[Define.EnumTreeAttribute.OrganizationId] = organization.OrganizationId.ToString();
                                attributes[Define.EnumTreeAttribute.AbnormalType] = string.Empty;
                                attributes[Define.EnumTreeAttribute.AbnormalReasonId] = string.Empty;

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
                        var abnormalReasons = context.AbnormalReasons.Where(x => x.OrganizationId == organizationId && x.Type == type).OrderBy(x => x.ARId).ToList();

                        foreach (var abnormalReason in abnormalReasons)
                        {
                            var treeItem = new TreeItem() { Title = abnormalReason.Name };

                            attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.AbnormalReason.ToString();
                            attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", abnormalReason.ARId, abnormalReason.Name);
                            attributes[Define.EnumTreeAttribute.OrganizationId] = abnormalReason.OrganizationId.ToString();
                            attributes[Define.EnumTreeAttribute.AbnormalType] = abnormalReason.Type;
                            attributes[Define.EnumTreeAttribute.AbnormalReasonId] = abnormalReason.AbnormalReasonId.ToString();

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

        public static RequestResult AddSolution(List<SolutionModel> solutionModels, List<string> selectedList, Guid refOrganizationId)
        {
            RequestResult result = new RequestResult();

            try
            {
                using (CFContext context=new CFContext())
                {
                    foreach (string selected in selectedList)
                    {
                        string[] temp = selected.Split(Define.Seperators, StringSplitOptions.None);

                        var organizationId = temp[0];
                        var solutionType = temp[1];
                        var solutionId = temp[2];

                        if (!string.IsNullOrEmpty(solutionId))
                        {
                            if (!solutionModels.Any(x => x.SolutionId == solutionId))
                            {
                                var solution = context.Solutions.First(x => x.SolutionId == new Guid(solutionId));

                                solutionModels.Add(new SolutionModel()
                                {
                                    SolutionId = solution.SolutionId.ToString(),
                                    Type = solution.Type,
                                    SId = solution.SId,
                                    Name = solution.Name
                                });
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(solutionType))
                            {
                                var solutions = context.Solutions.Where(x => x.OrganizationId == new Guid(organizationId) && x.Type == solutionType).ToList();

                                foreach (var solution in solutions)
                                {
                                    if (!solutionModels.Any(x => x.SolutionId == solution.SolutionId.ToString()))
                                    {
                                        solutionModels.Add(new SolutionModel()
                                        {
                                            SolutionId = solution.SolutionId.ToString(),
                                            Type = solution.Type,
                                            SId = solution.SId,
                                            Name = solution.Name
                                        });
                                    }
                                }
                            }
                            else
                            {
                                var availableOrganizationIds = OrganizationDataAccessor.GetUpStreamOrganizationIds(refOrganizationId, true);

                                var downStreamOrganizationIds = OrganizationDataAccessor.GetDownStreamOrganizationIds(new Guid(organizationId), true);

                                foreach (var downStreamOrganizationId in downStreamOrganizationIds)
                                {
                                    if (availableOrganizationIds.Any(x => x == downStreamOrganizationId))
                                    {
                                        var solutions = context.Solutions.Where(x => x.OrganizationId == downStreamOrganizationId).ToList();

                                        foreach (var solution in solutions)
                                        {
                                            if (!solutionModels.Any(x => x.SolutionId == solution.SolutionId.ToString()))
                                            {
                                                solutionModels.Add(new SolutionModel()
                                                {
                                                    SolutionId = solution.SolutionId.ToString(),
                                                    Type = solution.Type,
                                                    SId = solution.SId,
                                                    Name = solution.Name
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                result.ReturnData(solutionModels.OrderBy(x => x.Type).ThenBy(x => x.SId).ToList());
            }
            catch (Exception ex)
            {
                var err = new Error(MethodInfo.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return result;
        }

        public static RequestResult Edit(EditFormModel editFormModel)
        {
            RequestResult result = new RequestResult();

            try
            {
                if (editFormModel.FormInput.Type == Define.Other || editFormModel.FormInput.Type == Define.New)
                {
                    result.ReturnFailedMessage(string.Format("{0} {1}", Resources.Resource.Unsupported, Resources.Resource.AbnormalReasonType));
                }
                else
                {
                    using (CFContext context=new CFContext())
                    {
                        var abnormalReason = context.AbnormalReasons.Include("Soutions").First(x => x.AbnormalReasonId == new Guid(editFormModel.AbnormalReasonId));

                        var exists = context.AbnormalReasons.FirstOrDefault(x => x.AbnormalReasonId != abnormalReason.AbnormalReasonId && x.OrganizationId == abnormalReason.OrganizationId && x.Type == editFormModel.FormInput.Type && x.ARId == editFormModel.FormInput.ARId);

                        if (exists == null)
                        {
#if !DEBUG
                    using (TransactionScope trans = new TransactionScope())
                    {
#endif
                            #region AbnormalReason
                            abnormalReason.Type = editFormModel.FormInput.Type;
                            abnormalReason.ARId = editFormModel.FormInput.ARId;
                            abnormalReason.Name = editFormModel.FormInput.Name;
                            abnormalReason.LastModifyTime = DateTime.Now;

                            context.SaveChanges();
                            #endregion

                            #region AbnormalReasonHandlingMethod
                            #region Delete
                            abnormalReason.Solutions = new HashSet<CF.Models.Maintenance.Solution>();
                            context.SaveChanges();
                            #endregion

                            #region Insert
                            abnormalReason.Solutions = editFormModel.SolutionModels.Select(x => new CF.Models.Maintenance.Solution()
                            {
                                SolutionId = new Guid(x.SolutionId),
                                SId = x.SId,
                                Name = x.Name,
                                Type = x.Type,
                                OrganizationId = new Guid(editFormModel.OrganizationId)
                            }).ToList();                            
                            context.SaveChanges();
                            #endregion
                            #endregion
#if !DEBUG
                        trans.Complete();
                    }
#endif
                            result.ReturnSuccessMessage(string.Format("{0} {1} {2}", Resources.Resource.Edit, Resources.Resource.AbnormalReason, Resources.Resource.Success));
                        }
                        else
                        {
                            result.ReturnFailedMessage(string.Format("{0} {1}", Resources.Resource.ARId, Resources.Resource.Exists));
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

        public static RequestResult GetEditFormModel(string abnormalReasonId)
        {
            RequestResult result = new RequestResult();

            try
            {
                using (CFContext context=new CFContext())
                {
                    var abnormalReason = context.AbnormalReasons.Include("Solutions").First(x => x.AbnormalReasonId == new Guid(abnormalReasonId));

                    var model = new EditFormModel()
                    {
                        AbnormalReasonId = abnormalReason.AbnormalReasonId.ToString(),
                        OrganizationId = abnormalReason.OrganizationId.ToString(),
                        ParentOrganizationFullName = OrganizationDataAccessor.GetOrganizationFullName(abnormalReason.OrganizationId),
                        TypeSelectListItems = new List<SelectListItem>()
                        {
                            Define.DefaultSelectListItem(Resources.Resource.Select),
                            new SelectListItem()
                            {
                                Text = Resources.Resource.Create + "...",
                                Value = Define.New
                            }
                        },
                        SolutionModels = abnormalReason.Solutions.Select(x => new SolutionModel()
                        {
                            SolutionId = x.SolutionId.ToString(),
                            SId = x.SId,
                            Name = x.Name,
                            Type = x.Type
                        }).OrderBy(x => x.Type).ThenBy(x => x.SId).ToList(),                        
                        FormInput = new FormInput()
                        {
                            Type = abnormalReason.Type,
                            ARId = abnormalReason.ARId,
                            Name = abnormalReason.Name
                        }
                    };

                    var upStreamOrganizationIds = OrganizationDataAccessor.GetUpStreamOrganizationIds(new Guid(model.OrganizationId), true);

                    model.TypeSelectListItems.AddRange(context.AbnormalReasons.Where(x => upStreamOrganizationIds.Contains(x.OrganizationId)).Select(x => x.Type).Distinct().OrderBy(x => x).Select(x => new SelectListItem
                    {
                        Value = x,
                        Text = x
                    }).ToList());

                    if (!string.IsNullOrEmpty(model.FormInput.Type) && model.TypeSelectListItems.Any(x => x.Value == model.FormInput.Type))
                    {
                        model.TypeSelectListItems.First(x => x.Value == model.FormInput.Type).Selected = true;
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

        public static RequestResult Create(CreateFormModel createFormModel)
        {
            RequestResult result = new RequestResult();

            try
            {
                if (createFormModel.FormInput.Type == Define.Other || createFormModel.FormInput.Type == Define.New)
                {
                    result.ReturnFailedMessage(string.Format("{0} {1}", Resources.Resource.Unsupported, Resources.Resource.AbnormalReasonType));
                }
                else
                {
                    using (CFContext context=new CFContext())
                    {
                        var exists = context.AbnormalReasons.FirstOrDefault(x => x.OrganizationId == new Guid(createFormModel.OrganizationId) && x.Type == createFormModel.FormInput.Type && x.ARId == createFormModel.FormInput.ARId);

                        if (exists == null)
                        {
                            Guid abnormalReasonId = Guid.NewGuid();

                            context.AbnormalReasons.Add(new CF.Models.Maintenance.AbnormalReason()
                            {
                                AbnormalReasonId = abnormalReasonId,
                                OrganizationId = new Guid(createFormModel.OrganizationId),
                                Type = createFormModel.FormInput.Type,
                                ARId = createFormModel.FormInput.ARId,
                                Name = createFormModel.FormInput.Name,
                                LastModifyTime = DateTime.Now,
                                Solutions=createFormModel.SolutionModels.Select(x=>new CF.Models.Maintenance.Solution()
                                {
                                    SolutionId=new Guid(x.SolutionId),
                                    SId=x.SId,
                                    Name=x.Name,
                                    Type=x.Type,
                                    OrganizationId=new Guid(createFormModel.OrganizationId)
                                }).ToList(),                                
                            });

                            context.SaveChanges();

                            result.ReturnData(abnormalReasonId, string.Format("{0} {1} {2}", Resources.Resource.Create, Resources.Resource.AbnormalReason, Resources.Resource.Success));
                        }
                        else
                        {
                            result.ReturnFailedMessage(string.Format("{0} {1}", Resources.Resource.ARId, Resources.Resource.Exists));
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

        public static RequestResult GetCreateFormModel(string organizationId, string abnormalType)
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
                        Types = new List<SelectListItem>()
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
                            Type = abnormalType
                        }
                    };

                    var upStreamOrganizationIds = OrganizationDataAccessor.GetUpStreamOrganizationIds(new Guid(organizationId), true);

                    model.Types.AddRange(context.AbnormalReasons.Where(x => upStreamOrganizationIds.Contains(x.OrganizationId)).Select(x => x.Type).Distinct().OrderBy(x => x).Select(x => new SelectListItem
                    {
                        Value = x,
                        Text = x
                    }).ToList());

                    if (!string.IsNullOrEmpty(abnormalType) && model.Types.Any(x => x.Value == abnormalType))
                    {
                        model.Types.First(x => x.Value == abnormalType).Selected = true;
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

        public static RequestResult GetDetailViewModel(string abnormalReasonId, Account account)
        {
            RequestResult result = new RequestResult();

            try
            {
                using (CFContext context=new CFContext())
                {
                    var abnormalReason = context.AbnormalReasons.Include("Solutions").First(x => x.AbnormalReasonId == new Guid(abnormalReasonId));

                    result.ReturnData(new DetailViewModel()
                    {
                        AbnormalReasonId = abnormalReason.AbnormalReasonId.ToString(),
                        Permission = account.OrganizationPermission(abnormalReason.OrganizationId),
                        OrganizationId = abnormalReason.OrganizationId.ToString(),
                        ParentOrganizationFullName = OrganizationDataAccessor.GetOrganizationFullName(abnormalReason.OrganizationId),
                        Type = abnormalReason.Type,
                        ARId = abnormalReason.ARId,
                        Name = abnormalReason.Name,
                        SolutionNames = abnormalReason.Solutions.Select(x=>x.Name).ToList()
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
    }
}
