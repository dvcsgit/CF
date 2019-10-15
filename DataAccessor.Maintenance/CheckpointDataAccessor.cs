using CF;
using CF.Models.Maintenance;
using Models.Authentication;
using Models.Maintenance.Checkpoint;
using Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Utility;
using Utility.Models;

namespace DataAccessor.Maintenance
{
    public class CheckpointDataAccessor
    {
        public static RequestResult Query(QueryParameters queryParameters, Account account)
        {
            RequestResult result = new RequestResult();

            try
            {
                using (CFContext context=new CFContext())
                {
                    var downStreamOrganizationIds = OrganizationDataAccessor.GetDownStreamOrganizationIds(new Guid(queryParameters.OrganizationId), true);

                    var query = context.Checkpoints.Where(x => downStreamOrganizationIds.Contains(x.OrganizationId) && account.QueryableOrganizationIds.Contains(x.OrganizationId)).AsQueryable();

                    if (!string.IsNullOrEmpty(queryParameters.Keyword))
                    {
                        query = query.Where(x => x.CId.Contains(queryParameters.Keyword) || x.Name.Contains(queryParameters.Keyword));
                    }

                    var organization = OrganizationDataAccessor.GetOrganization(new Guid(queryParameters.OrganizationId));

                    result.ReturnData(new GridViewModel()
                    {
                        Permission = account.OrganizationPermission(new Guid(queryParameters.OrganizationId)),
                        OrganizationId = queryParameters.OrganizationId,
                        OrganizationName = organization.Name,
                        FullOrganizationName = organization.FullName,
                        GridItems = query.ToList().Select(x => new GridItem
                        {
                            ChcekpointId = x.CheckpointId.ToString(),
                            Permission = account.OrganizationPermission(x.OrganizationId),
                            OrganizationName = OrganizationDataAccessor.GetOrganizationName(x.OrganizationId),
                            CId = x.CId,
                            Name = x.Name
                        }).OrderBy(x => x.OrganizationName).ThenBy(x => x.CId).ToList()
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

        public static RequestResult GetDetailViewModel(string checkpointId, Account account)
        {
            RequestResult result = new RequestResult();

            try
            {
                using (CFContext context = new CFContext())
                {
                    var checkpoint = context.Checkpoints.Include("CheckItems").First(x => x.CheckpointId == new Guid(checkpointId));

                    result.ReturnData(new DetailViewModel()
                    {
                        CheckpointId = checkpoint.CheckpointId.ToString(),
                        Permission = account.OrganizationPermission(checkpoint.OrganizationId),
                        OrganizationUniqueID = checkpoint.OrganizationId.ToString(),
                        ParentOrganizationFullName = OrganizationDataAccessor.GetOrganizationFullName(checkpoint.OrganizationId),
                        CId = checkpoint.CId,
                        Name = checkpoint.Name,
                        IsFeelItemDefaultNormal = checkpoint.IsFeelItemDefaultNormal,
                        TagId = checkpoint.TagId,
                        Remark = checkpoint.Remark,
                        CheckItemModels = checkpoint.CheckItems.Select(x=>new CheckItemModel()
                        {
                            CheckItemId=x.CheckItemId.ToString(),
                            CIId=x.CIId,
                            Name=x.Name,
                            Type=x.Type,
                            IsFeelItem=x.IsFeelItem,
                            IsAccumulation=x.IsAccumulation,
                            IsInherit=x.IsInherit,
                            OriLowerLimit=x.LowerLimit,
                            OriLowerAlertLimit=x.LowerAlertLimit,
                            OriUpperAlertLimit=x.UpperAlertLimit,
                            OriUpperLimit=x.UpperLimit,
                            OriAccumulationBase=x.AccumulationBase,
                            OriUnit=x.Unit,
                            OriRemark=x.Remark,
                            LowerLimit=x.LowerLimit,
                            LowerAlertLimit=x.LowerAlertLimit,
                            UpperAlertLimit=x.UpperAlertLimit,
                            UpperLimit=x.UpperLimit,
                            AccumulationBase=x.AccumulationBase,
                            Unit=x.Unit,
                            Remark=x.Remark
                        }).OrderBy(x=>x.Type).ThenBy(x=>x.CIId).ToList()                        
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

        public static RequestResult Create(CreateFormModel createFormModel)
        {
            RequestResult result = new RequestResult();

            try
            {
                using (CFContext context = new CFContext())
                {
                    var exists = context.Checkpoints.FirstOrDefault(x => x.OrganizationId == new Guid(createFormModel.OrganizationId) && x.CId == createFormModel.FormInput.CId);

                    if (exists == null)
                    {
                        if (!string.IsNullOrEmpty(createFormModel.FormInput.TagId) && context.Checkpoints.Any(x => x.TagId == createFormModel.FormInput.TagId))
                        {
                            var checkpoint = context.Checkpoints.First(x => x.TagId == createFormModel.FormInput.TagId);

                            var organization = OrganizationDataAccessor.GetOrganization(checkpoint.OrganizationId);

                            result.ReturnFailedMessage(string.Format("{0} {1} {2} {3} {4}", Resources.Resource.TagId, createFormModel.FormInput.TagId, Resources.Resource.Exists, organization.Name, checkpoint.Name));
                        }
                        else
                        {
                            Guid checkpointId = Guid.NewGuid();

                            context.Checkpoints.Add(new CF.Models.Maintenance.Checkpoint()
                            {
                                CheckpointId = checkpointId,
                                OrganizationId = new Guid(createFormModel.OrganizationId),
                                CId = createFormModel.FormInput.CId,
                                Name = createFormModel.FormInput.Name,
                                IsFeelItemDefaultNormal = createFormModel.FormInput.IsFeelItemDefaultNormal,
                                TagId = createFormModel.FormInput.TagId,
                                Remark = createFormModel.FormInput.Remark,
                                CheckItems = createFormModel.CheckItemModels.Select(x => new CF.Models.Maintenance.CheckItem()
                                {
                                    CheckItemId = new Guid(x.CheckItemId),
                                    IsInherit = x.IsInherit,
                                    LowerLimit = x.LowerLimit,
                                    LowerAlertLimit = x.LowerAlertLimit,
                                    UpperAlertLimit = x.UpperAlertLimit,
                                    UpperLimit = x.UpperLimit,
                                    AccumulationBase = x.AccumulationBase,
                                    Unit = x.Unit,
                                    Remark = x.Remark
                                }).ToList(),
                                LastModifyTime = DateTime.Now
                            });
                           
                            context.SaveChanges();

                            result.ReturnData(checkpointId, string.Format("{0} {1} {2}", Resources.Resource.Create, Resources.Resource.Checkpoint, Resources.Resource.Success));
                        }
                    }
                    else
                    {
                        result.ReturnFailedMessage(string.Format("{0} {1}", Resources.Resource.CId, Resources.Resource.Exists));
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

        public static RequestResult GetCopyFormModel(string checkpointId)
        {
            RequestResult result = new RequestResult();

            try
            {
                using (CFContext context = new CFContext())
                {
                    var checkpoint = context.Checkpoints.Include("CheckItems").First(x => x.CheckpointId == new Guid(checkpointId));

                    result.ReturnData(new CreateFormModel()
                    {
                        OrganizationId = checkpoint.OrganizationId.ToString(),
                        ParentOrganizationFullName = OrganizationDataAccessor.GetOrganizationFullName(checkpoint.OrganizationId),
                        CheckItemModels =checkpoint.CheckItems.Select(x=>new CheckItemModel()
                        {
                            CheckItemId = x.CheckItemId.ToString(),
                            CIId = x.CIId,
                            Name = x.Name,
                            Type = x.Type,
                            IsFeelItem = x.IsFeelItem,
                            IsAccumulation = x.IsAccumulation,
                            IsInherit = x.IsInherit,
                            OriLowerLimit = x.LowerLimit,
                            OriLowerAlertLimit = x.LowerAlertLimit,
                            OriUpperAlertLimit = x.UpperAlertLimit,
                            OriUpperLimit = x.UpperLimit,
                            OriAccumulationBase = x.AccumulationBase,
                            OriUnit = x.Unit,
                            OriRemark = x.Remark,
                            LowerLimit = x.LowerLimit,
                            LowerAlertLimit = x.LowerAlertLimit,
                            UpperAlertLimit = x.UpperAlertLimit,
                            UpperLimit = x.UpperLimit,
                            AccumulationBase = x.AccumulationBase,
                            Unit = x.Unit,
                            Remark = x.Remark
                        }).OrderBy(x => x.Type).ThenBy(x => x.CIId).ToList(),
                       
                        FormInput = new FormInput()
                        {
                            IsFeelItemDefaultNormal = checkpoint.IsFeelItemDefaultNormal,
                            Remark = checkpoint.Remark
                        }
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

        public static RequestResult GetEditFormModel(string checkpointId)
        {
            RequestResult result = new RequestResult();

            try
            {
                using (CFContext context = new CFContext())
                {
                    var checkpoint = context.Checkpoints.Include("CheckItems").First(x => x.CheckpointId == new Guid(checkpointId));

                    result.ReturnData(new EditFormModel()
                    {
                        CheckpointId = checkpoint.CheckpointId.ToString(),
                        OrganizationId = checkpoint.OrganizationId.ToString(),
                        ParentOrganizationFullName = OrganizationDataAccessor.GetOrganizationFullName(checkpoint.OrganizationId),
                        CheckItemModels = checkpoint.CheckItems.Select(x=>new CheckItemModel()
                        {
                            CheckItemId = x.CheckItemId.ToString(),
                            CIId = x.CIId,
                            Name = x.Name,
                            Type = x.Type,
                            IsFeelItem = x.IsFeelItem,
                            IsAccumulation = x.IsAccumulation,
                            IsInherit = x.IsInherit,
                            OriLowerLimit = x.LowerLimit,
                            OriLowerAlertLimit = x.LowerAlertLimit,
                            OriUpperAlertLimit = x.UpperAlertLimit,
                            OriUpperLimit = x.UpperLimit,
                            OriAccumulationBase = x.AccumulationBase,
                            OriUnit = x.Unit,
                            OriRemark = x.Remark,
                            LowerLimit = x.LowerLimit,
                            LowerAlertLimit = x.LowerAlertLimit,
                            UpperAlertLimit = x.UpperAlertLimit,
                            UpperLimit = x.UpperLimit,
                            AccumulationBase = x.AccumulationBase,
                            Unit = x.Unit,
                            Remark = x.Remark
                        }).OrderBy(x => x.Type).ThenBy(x => x.CIId).ToList(),

                        FormInput = new FormInput()
                        {
                            CId = checkpoint.CId,
                            Name = checkpoint.Name,
                            IsFeelItemDefaultNormal = checkpoint.IsFeelItemDefaultNormal,
                            TagId = checkpoint.TagId,
                            Remark = checkpoint.Remark
                        }
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

        public static RequestResult Edit(EditFormModel editFormModel)
        {
            RequestResult result = new RequestResult();

            try
            {
                using (CFContext context = new CFContext())
                {
                    var checkpoint = context.Checkpoints.Include("CheckItems").First(x => x.CheckpointId == new Guid(editFormModel.CheckpointId));

                    var exists = context.Checkpoints.FirstOrDefault(x => x.CheckpointId != checkpoint.CheckpointId && x.OrganizationId == checkpoint.OrganizationId && x.CId == editFormModel.FormInput.CId);

                    if (exists == null)
                    {
                        if (!string.IsNullOrEmpty(editFormModel.FormInput.TagId) && context.Checkpoints.Any(x => x.CheckpointId != checkpoint.CheckpointId && x.TagId == editFormModel.FormInput.TagId))
                        {
                            var query = context.Checkpoints.First(x => x.CheckpointId != checkpoint.CheckpointId && x.TagId == editFormModel.FormInput.TagId);

                            var organization = OrganizationDataAccessor.GetOrganization(query.OrganizationId);

                            result.ReturnFailedMessage(string.Format("{0} {1} {2} {3} {4}", Resources.Resource.TagId, editFormModel.FormInput.TagId, Resources.Resource.Exists, organization.Name, checkpoint.Name));
                        }
                        else
                        {
#if !DEBUG
                    using (TransactionScope trans = new TransactionScope())
                    {
#endif
                            #region ControlPoint
                            checkpoint.CId = editFormModel.FormInput.CId;
                            checkpoint.Name = editFormModel.FormInput.Name;
                            checkpoint.TagId = editFormModel.FormInput.TagId;
                            checkpoint.Remark = editFormModel.FormInput.Remark;
                            checkpoint.IsFeelItemDefaultNormal = editFormModel.FormInput.IsFeelItemDefaultNormal;
                            checkpoint.LastModifyTime = DateTime.Now;

                            context.SaveChanges();
                            #endregion

                            #region CheckItems
                            #region Delete
                            checkpoint.CheckItems = new HashSet<CheckItem>();                            

                            context.SaveChanges();
                            #endregion

                            #region Insert
                            checkpoint.CheckItems = editFormModel.CheckItemModels.Select(x => new CheckItem()
                            {
                                CheckItemId = new Guid(x.CheckItemId),
                                CIId = x.CIId,
                                Name = x.Name,
                                Type = x.Type,
                                IsInherit = x.IsInherit,
                                IsFeelItem = x.IsFeelItem,
                                IsAccumulation = x.IsAccumulation,
                                LowerLimit = x.LowerLimit,
                                LowerAlertLimit = x.LowerAlertLimit,
                                UpperLimit = x.UpperLimit,
                                UpperAlertLimit = x.UpperAlertLimit,
                                Unit = x.Unit,
                                Remark = x.Remark,
                                AccumulationBase = x.AccumulationBase,
                                LastModifyTime = DateTime.Now
                            }).ToList();                            

                            context.SaveChanges();
                            #endregion
                            #endregion
#if !DEBUG
                        trans.Complete();
                    }
#endif
                            result.ReturnSuccessMessage(string.Format("{0} {1} {2}", Resources.Resource.Edit, Resources.Resource.Checkpoint, Resources.Resource.Success));
                        }
                    }
                    else
                    {
                        result.ReturnFailedMessage(string.Format("{0} {1}", Resources.Resource.Checkpoint, Resources.Resource.Exists));
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

        public static RequestResult SavePageState(List<CheckItemModel> checkItemModels, List<string> pageStateList)
        {
            RequestResult result = new RequestResult();

            try
            {
                foreach (string pageState in pageStateList)
                {
                    string[] temp = pageState.Split(Define.Seperators, StringSplitOptions.None);

                    string isInherit = temp[0];
                    string checkItemId = temp[1];
                    string lowerLimit = temp[2];
                    string lowerAlertLimit = temp[3];
                    string upperAlertLimit = temp[4];
                    string upperLimit = temp[5];
                    string accumulationBase = temp[6];
                    string unit = temp[7];
                    string remark = temp[8];

                    var checkItem = checkItemModels.First(x => x.CheckItemId == checkItemId);

                    checkItem.IsInherit = isInherit == "Y";

                    if (!checkItem.IsInherit)
                    {
                        checkItem.LowerLimit = !string.IsNullOrEmpty(lowerLimit) ? double.Parse(lowerLimit) : default(double?);
                        checkItem.LowerAlertLimit = !string.IsNullOrEmpty(lowerAlertLimit) ? double.Parse(lowerAlertLimit) : default(double?);
                        checkItem.UpperAlertLimit = !string.IsNullOrEmpty(upperAlertLimit) ? double.Parse(upperAlertLimit) : default(double?);
                        checkItem.UpperLimit = !string.IsNullOrEmpty(upperLimit) ? double.Parse(upperLimit) : default(double?);
                        checkItem.AccumulationBase = !string.IsNullOrEmpty(accumulationBase) ? double.Parse(accumulationBase) : default(double?);
                        checkItem.Unit = unit;
                        checkItem.Remark = remark;
                    }
                    else
                    {
                        checkItem.LowerLimit = null;
                        checkItem.LowerAlertLimit = null;
                        checkItem.UpperAlertLimit = null;
                        checkItem.UpperLimit = null;
                        checkItem.AccumulationBase = null;
                        checkItem.Unit = string.Empty;
                        checkItem.Remark = string.Empty;
                    }
                }

                result.ReturnData(checkItemModels);
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
                using (CFContext context = new CFContext())
                {
                    DeleteHelper.DeleteCheckpoints(context, selectedList);

                    context.SaveChanges();
                }

                result.ReturnSuccessMessage(string.Format("{0} {1} {2}", Resources.Resource.Delete, Resources.Resource.Checkpoint, Resources.Resource.Success));
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return result;
        }

        public static RequestResult AddCheckItem(List<CheckItemModel> checkItemModels, List<string> selectedList, string refOrganizationId)
        {
            RequestResult result = new RequestResult();

            try
            {
                using (CFContext context = new CFContext())
                {
                    foreach (string selected in selectedList)
                    {
                        string[] temp = selected.Split(Define.Seperators, StringSplitOptions.None);

                        var organizationId = temp[0];
                        var checkType = temp[1];
                        var checkItemId = temp[2];

                        if (!string.IsNullOrEmpty(checkItemId))
                        {
                            if (!checkItemModels.Any(x => x.CheckItemId == checkItemId))
                            {
                                var checkItem = context.CheckItems.First(x => x.CheckItemId == new Guid(checkItemId));

                                checkItemModels.Add(new CheckItemModel()
                                {
                                    CheckItemId = checkItem.CheckItemId.ToString(),
                                    Type = checkItem.Type,
                                    CIId = checkItem.CIId,
                                    Name = checkItem.Name,
                                    IsFeelItem = checkItem.IsFeelItem,
                                    IsAccumulation = checkItem.IsAccumulation,
                                    IsInherit = true,
                                    OriLowerLimit = checkItem.LowerLimit,
                                    OriLowerAlertLimit = checkItem.LowerAlertLimit,
                                    OriUpperAlertLimit = checkItem.UpperAlertLimit,
                                    OriUpperLimit = checkItem.UpperLimit,
                                    OriAccumulationBase = checkItem.AccumulationBase,
                                    OriUnit = checkItem.Unit,
                                    OriRemark = checkItem.Remark
                                });
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(checkType))
                            {
                                var checkItems = context.CheckItems.Where(x => x.OrganizationId == new Guid(organizationId) && x.Type == checkType).ToList();

                                foreach (var checkItem in checkItems)
                                {
                                    if (!checkItemModels.Any(x => x.CheckItemId == checkItem.CheckItemId.ToString()))
                                    {
                                        checkItemModels.Add(new CheckItemModel()
                                        {
                                            CheckItemId = checkItem.CheckItemId.ToString(),
                                            Type = checkItem.Type,
                                            CIId = checkItem.CIId,
                                            Name = checkItem.Name,
                                            IsFeelItem = checkItem.IsFeelItem,
                                            IsAccumulation = checkItem.IsAccumulation,
                                            IsInherit = true,
                                            OriLowerLimit = checkItem.LowerLimit,
                                            OriLowerAlertLimit = checkItem.LowerAlertLimit,
                                            OriUpperAlertLimit = checkItem.UpperAlertLimit,
                                            OriUpperLimit = checkItem.UpperLimit,
                                            OriAccumulationBase = checkItem.AccumulationBase,
                                            OriUnit = checkItem.Unit,
                                            OriRemark = checkItem.Remark
                                        });
                                    }
                                }
                            }
                            else
                            {
                                var availableOrganizationIds = OrganizationDataAccessor.GetUpStreamOrganizationIds(new Guid(refOrganizationId), true);

                                var downStreamOrganizationIds = OrganizationDataAccessor.GetDownStreamOrganizationIds(new Guid(organizationId), true);

                                foreach (var downStreamOrganizationId in downStreamOrganizationIds)
                                {
                                    if (availableOrganizationIds.Any(x => x == downStreamOrganizationId))
                                    {
                                        var checkItems = context.CheckItems.Where(x => x.OrganizationId == downStreamOrganizationId).ToList();

                                        foreach (var checkItem in checkItems)
                                        {
                                            if (!checkItemModels.Any(x => x.CheckItemId == checkItem.CheckItemId.ToString()))
                                            {
                                                checkItemModels.Add(new CheckItemModel()
                                                {
                                                    CheckItemId = checkItem.CheckItemId.ToString(),
                                                    Type = checkItem.Type,
                                                    CIId = checkItem.CIId,
                                                    Name = checkItem.Name,
                                                    IsFeelItem = checkItem.IsFeelItem,
                                                    IsAccumulation = checkItem.IsAccumulation,
                                                    IsInherit = true,
                                                    OriLowerLimit = checkItem.LowerLimit,
                                                    OriLowerAlertLimit = checkItem.LowerAlertLimit,
                                                    OriUpperAlertLimit = checkItem.UpperAlertLimit,
                                                    OriUpperLimit = checkItem.UpperLimit,
                                                    OriAccumulationBase = checkItem.AccumulationBase,
                                                    OriUnit = checkItem.Unit,
                                                    OriRemark = checkItem.Remark
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                checkItemModels = checkItemModels.OrderBy(x => x.Type).ThenBy(x => x.CIId).ToList();

                result.ReturnData(checkItemModels);
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return result;
        }

        public static RequestResult GetTreeItems(List<Models.Shared.Organization> organizations, Guid organizationId, Account account)
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
                    { Define.EnumTreeAttribute.CheckpointId, string.Empty }
                };

                using (CFContext context = new CFContext())
                {
                    if (account.QueryableOrganizationIds.Contains(organizationId))
                    {
                        var checkpoints = context.Checkpoints.Where(x => x.OrganizationId == organizationId).OrderBy(x => x.CId).ToList();

                        foreach (var checkpoint in checkpoints)
                        {
                            var treeItem = new TreeItem() { Title = checkpoint.Name };

                            attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.Checkpoint.ToString();
                            attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", checkpoint.CId, checkpoint.Name);
                            attributes[Define.EnumTreeAttribute.OrganizationId] = organizationId.ToString();
                            attributes[Define.EnumTreeAttribute.CheckpointId] = checkpoint.CheckpointId.ToString();

                            foreach (var attribute in attributes)
                            {
                                treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                            }

                            treeItemList.Add(treeItem);
                        }
                    }

                    var newOrganizations = organizations.Where(x => x.ParentId == organizationId && account.VisibleOrganizationIds.Contains(x.OrganizationId)).OrderBy(x => x.OId).ToList();

                    foreach (var organization in newOrganizations)
                    {
                        var treeItem = new TreeItem() { Title = organization.Name };

                        attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.Organization.ToString();
                        attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", organization.OId, organization.Name);
                        attributes[Define.EnumTreeAttribute.OrganizationId] = organization.OrganizationId.ToString();
                        attributes[Define.EnumTreeAttribute.CheckpointId] = string.Empty;

                        foreach (var attribute in attributes)
                        {
                            treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                        }

                        if (organizations.Any(x => x.ParentId == organization.OrganizationId && account.VisibleOrganizationIds.Contains(x.OrganizationId))
                            ||
                            (account.QueryableOrganizationIds.Contains(organization.OrganizationId) && context.Checkpoints.Any(x => x.OrganizationId == organization.OrganizationId)))
                        {
                            treeItem.State = "closed";
                        }

                        treeItemList.Add(treeItem);
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

        public static RequestResult GetRootTreeItems(List<Models.Shared.Organization> organizations, Guid organizationId, Account account)
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
                    { Define.EnumTreeAttribute.CheckpointId, string.Empty }
                };

                using (CFContext context = new CFContext())
                {
                    var organization = organizations.First(x => x.OrganizationId == organizationId);

                    var treeItem = new TreeItem() { Title = organization.Name };

                    attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.Organization.ToString();
                    attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", organization.OId, organization.Name);
                    attributes[Define.EnumTreeAttribute.OrganizationId] = organization.OrganizationId.ToString();
                    attributes[Define.EnumTreeAttribute.CheckpointId] = string.Empty;

                    foreach (var attribute in attributes)
                    {
                        treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                    }

                    if (organizations.Any(x => x.ParentId == organization.OrganizationId && account.VisibleOrganizationIds.Contains(x.OrganizationId))
                        ||
                        account.QueryableOrganizationIds.Contains(organization.OrganizationId) && context.Checkpoints.Any(x => x.OrganizationId == organization.OrganizationId))
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

        public static RequestResult GetTreeItem(List<Models.Shared.Organization> organizations, string refOrganizationId, string organizationId)
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
                    { Define.EnumTreeAttribute.CheckpointId, string.Empty }
                };

                using (CFContext context = new CFContext())
                {
                    var checkpoints = context.Checkpoints.Where(x => x.OrganizationId == new Guid(organizationId)).OrderBy(x => x.CId).ToList();

                    foreach (var checkpoint in checkpoints)
                    {
                        var treeItem = new TreeItem() { Title = string.Format("{0}/{1}", checkpoint.CId, checkpoint.Name) };

                        attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.Checkpoint.ToString();
                        attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", checkpoint.CId, checkpoint.Name);
                        attributes[Define.EnumTreeAttribute.OrganizationId ] = organizationId;
                        attributes[Define.EnumTreeAttribute.CheckpointId] = checkpoint.CheckpointId.ToString();

                        foreach (var attribute in attributes)
                        {
                            treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                        }

                        treeItemList.Add(treeItem);
                    }

                    var availableOrganizationIds = OrganizationDataAccessor.GetOrganizationPermissions(new Guid(refOrganizationId)).Select(x => x.OrganizationId).ToList();

                    var newOrganizations = organizations.Where(x => x.ParentId == new Guid(organizationId)).OrderBy(x => x.OId).ToList();

                    foreach (var organization in newOrganizations)
                    {
                        var downStreamOrganizationIds = OrganizationDataAccessor.GetDownStreamOrganizationIds(organization.OrganizationId, true);

                        if (context.Checkpoints.Any(x => downStreamOrganizationIds.Contains(x.OrganizationId) && availableOrganizationIds.Contains(x.OrganizationId)))
                        {
                            var treeItem = new TreeItem() { Title = organization.Name };

                            attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.Organization.ToString();
                            attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", organization.OId, organization.Name);
                            attributes[Define.EnumTreeAttribute.OrganizationId] = organization.OrganizationId.ToString();
                            attributes[Define.EnumTreeAttribute.CheckpointId] = string.Empty;

                            foreach (var attribute in attributes)
                            {
                                treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                            }

                            treeItem.State = "closed";

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
    }
}
