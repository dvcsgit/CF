using CF;
using Models.Authentication;
using Models.Maintenance.ESpecification;
using Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Utility;
using Utility.Models;

namespace DataAccessor.Maintenance
{
    public class ESpecificationDataAccessor
    {        
        public static RequestResult GetTreeItems(List<Organization> organizationList, Guid organizationId, string equipmentType, Account account)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                var treeItems = new List<TreeItem>();

                var attributes = new Dictionary<Define.EnumTreeAttribute, string>()
                {
                    { Define.EnumTreeAttribute.NodeType, string.Empty },
                    { Define.EnumTreeAttribute.ToolTip, string.Empty },
                    { Define.EnumTreeAttribute.OrganizationId, string.Empty },
                    { Define.EnumTreeAttribute.EquipmentType, string.Empty },
                    { Define.EnumTreeAttribute.EquipmentSpecificationId, string.Empty }
                };

                using (CFContext context=new CFContext())
                {
                    if (string.IsNullOrEmpty(equipmentType))
                    {
                        if (account.QueryableOrganizationIds.Contains(organizationId))
                        {
                            var equipmentTypes = context.ESpecifications.Where(x => x.OrganizationId == organizationId).Select(x => x.EquipmentType).Distinct().OrderBy(x => x).ToList();

                            foreach (var type in equipmentTypes)
                            {
                                var treeItem = new TreeItem() { Title = equipmentType };

                                attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.EquipmentType.ToString();
                                attributes[Define.EnumTreeAttribute.ToolTip] = type;
                                attributes[Define.EnumTreeAttribute.OrganizationId] = organizationId.ToString();
                                attributes[Define.EnumTreeAttribute.EquipmentType] = type;
                                attributes[Define.EnumTreeAttribute.EquipmentSpecificationId] = string.Empty;

                                foreach (var attribute in attributes)
                                {
                                    treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                                }

                                treeItem.State = "closed";

                                treeItems.Add(treeItem);
                            }
                        }

                        var newOrganizations = organizationList.Where(x => x.ParentId == organizationId && account.VisibleOrganizationIds.Contains(x.OrganizationId)).OrderBy(x => x.OId).ToList();

                        foreach (var organization in newOrganizations)
                        {
                            var treeItem = new TreeItem() { Title = organization.Name };

                            attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.Organization.ToString();
                            attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", organization.OId, organization.Name);
                            attributes[Define.EnumTreeAttribute.OrganizationId] = organization.OrganizationId.ToString();
                            attributes[Define.EnumTreeAttribute.EquipmentType] = string.Empty;
                            attributes[Define.EnumTreeAttribute.EquipmentSpecificationId] = string.Empty;

                            foreach (var attribute in attributes)
                            {
                                treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                            }

                            var haveDownStreamOrganization = organizationList.Any(x => x.ParentId == organization.OrganizationId && account.VisibleOrganizationIds.Contains(x.OrganizationId));

                            var haveEquipmentSpecification = account.QueryableOrganizationIds.Contains(organization.OrganizationId) && context.ESpecifications.Any(x => x.OrganizationId == organization.OrganizationId);

                            if (haveDownStreamOrganization || haveEquipmentSpecification)
                            {
                                treeItem.State = "closed";
                            }

                            treeItems.Add(treeItem);
                        }
                    }
                    else
                    {
                        var specifications = context.ESpecifications.Where(x => x.OrganizationId == organizationId && x.EquipmentType == equipmentType).OrderBy(x => x.Name).ToList();

                        foreach (var specification in specifications)
                        {
                            var treeItem = new TreeItem() { Title = specification.Name };

                            attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.EquipmentSpecification.ToString();
                            attributes[Define.EnumTreeAttribute.ToolTip] = specification.Name;
                            attributes[Define.EnumTreeAttribute.OrganizationId] = specification.OrganizationId.ToString();
                            attributes[Define.EnumTreeAttribute.EquipmentType] = specification.EquipmentType;
                            attributes[Define.EnumTreeAttribute.EquipmentSpecificationId] = specification.ESpecificationId.ToString();

                            foreach (var attribute in attributes)
                            {
                                treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                            }

                            treeItems.Add(treeItem);
                        }
                    }
                }

                requestResult.ReturnData(treeItems);
            }
            catch (Exception e)
            {
                var error = new Error(MethodBase.GetCurrentMethod(), e);

                Logger.Log(error);

                requestResult.ReturnError(error);
            }

            return requestResult;
        }

        public static RequestResult GetTreeItems(List<Models.Shared.Organization> organizationList, Guid refOrganizationId, Guid organizationId, string equipmentType)
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
                    { Define.EnumTreeAttribute.EquipmentType, string.Empty },
                    { Define.EnumTreeAttribute.EquipmentSpecificationId, string.Empty }
                };

                using (CFContext context=new CFContext())
                {
                    if (string.IsNullOrEmpty(equipmentType))
                    {
                        var equipmentTypes = context.ESpecifications.Where(x => x.OrganizationId == organizationId).Select(x => x.EquipmentType).Distinct().OrderBy(x => x).ToList();

                        foreach (var type in equipmentTypes)
                        {
                            var treeItem = new TreeItem() { Title = type };

                            attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.EquipmentType.ToString();
                            attributes[Define.EnumTreeAttribute.ToolTip] = type;
                            attributes[Define.EnumTreeAttribute.OrganizationId] = organizationId.ToString();
                            attributes[Define.EnumTreeAttribute.EquipmentType] = equipmentType;
                            attributes[Define.EnumTreeAttribute.EquipmentSpecificationId] = string.Empty;

                            foreach (var attribute in attributes)
                            {
                                treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                            }

                            treeItem.State = "closed";

                            treeItemList.Add(treeItem);
                        }

                        var availableOrganizationIds = OrganizationDataAccessor.GetUpStreamOrganizationIds(refOrganizationId, true);

                        var organizations = organizationList.Where(x => x.ParentId == organizationId && availableOrganizationIds.Contains(x.OrganizationId)).OrderBy(x => x.OId).ToList();

                        foreach (var organization in organizations)
                        {
                            var downStreamOrganizationList = OrganizationDataAccessor.GetDownStreamOrganizationIds(organization.OrganizationId, true);

                            if (context.ESpecifications.Any(x => downStreamOrganizationList.Contains(x.OrganizationId) && availableOrganizationIds.Contains(x.OrganizationId)))
                            {
                                var treeItem = new TreeItem() { Title = organization.Name };

                                attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.Organization.ToString();
                                attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", organization.OId, organization.Name);
                                attributes[Define.EnumTreeAttribute.OrganizationId] = organization.OrganizationId.ToString();
                                attributes[Define.EnumTreeAttribute.EquipmentType] = string.Empty;
                                attributes[Define.EnumTreeAttribute.EquipmentSpecificationId] = string.Empty;

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
                        var specList = context.ESpecifications.Where(x => x.OrganizationId == organizationId && x.EquipmentType == equipmentType).OrderBy(x => x.Name).ToList();

                        foreach (var spec in specList)
                        {
                            var treeItem = new TreeItem() { Title = spec.Name };

                            attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.EquipmentSpecification.ToString();
                            attributes[Define.EnumTreeAttribute.ToolTip] = spec.Name;
                            attributes[Define.EnumTreeAttribute.OrganizationId] = spec.OrganizationId.ToString();
                            attributes[Define.EnumTreeAttribute.EquipmentType] = spec.EquipmentType;
                            attributes[Define.EnumTreeAttribute.EquipmentSpecificationId] = spec.ESpecificationId.ToString();

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
                    DeleteHelper.DeleteEquipmentSpecification(context, selectedList);

                    context.SaveChanges();
                }

                result.ReturnSuccessMessage(string.Format("{0} {1} {2}", Resources.Resource.Delete, Resources.Resource.EquipmentSpecification, Resources.Resource.Success));
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return result;
        }

        public static RequestResult GetCopyFormModel(string equipmentSpecificationId)
        {
            RequestResult result = new RequestResult();

            try
            {
                using (CFContext context=new CFContext())
                {
                    var spec = context.ESpecifications.First(x => x.ESpecificationId == new Guid(equipmentSpecificationId));

                    var model = new CreateFormModel()
                    {
                        OrganizationId = spec.OrganizationId.ToString(),
                        ParentOrganizationFullName = OrganizationDataAccessor.GetOrganizationFullName(spec.OrganizationId),
                        EquipmentTypeSelectItems = new List<SelectListItem>()
                        {
                            Define.DefaultSelectListItem(Resources.Resource.Select),
                            new SelectListItem()
                            {
                                Text = string.Format("{0}...", Resources.Resource.Create),
                                Value = Define.New
                            }
                        },
                        FormInput = new FormInput()
                        {
                            EquipmentType = spec.EquipmentType
                        },
                        EquipmentSpecificationOptionModels = context.ESOptions.Where(x => x.ESpecificationId == spec.ESpecificationId).Select(x => new EquipmentSpecificationOptionModel
                        {
                            Name = x.Name,
                            Seq = x.Seq
                        }).OrderBy(x => x.Seq).ToList()
                    };

                    var upStreamOrganizationIds = OrganizationDataAccessor.GetUpStreamOrganizationIds(spec.OrganizationId, true);

                    model.EquipmentTypeSelectItems.AddRange(context.ESpecifications.Where(x => upStreamOrganizationIds.Contains(x.OrganizationId)).Select(x => x.EquipmentType).Distinct().OrderBy(x => x).Select(x => new SelectListItem
                    {
                        Value = x,
                        Text = x
                    }).ToList());

                    model.EquipmentTypeSelectItems.First(x => x.Value == spec.EquipmentType).Selected = true;

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

        public static object Edit(EditFormModel editFormModel)
        {
            RequestResult result = new RequestResult();

            try
            {
                if (editFormModel.FormInput.EquipmentType == Define.New)
                {
                    result.ReturnFailedMessage(string.Format("{0} {1}", Resources.Resource.Unsupported, Resources.Resource.EquipmentType));
                }
                else
                {
                    using (CFContext context=new CFContext())
                    {

                        var spec = context.ESpecifications.First(x => x.ESpecificationId == new Guid(editFormModel.EquipmentSpecificationId));

                        var exists = context.ESpecifications.FirstOrDefault(x => x.ESpecificationId != spec.ESpecificationId && x.OrganizationId == spec.OrganizationId && x.EquipmentType == editFormModel.FormInput.EquipmentType && x.Name == editFormModel.FormInput.Name);

                        if (exists == null)
                        {
#if !DEBUG
                    using (TransactionScope trans = new TransactionScope())
                    {
#endif
                            #region EquipmentSpecification
                            spec.EquipmentType = editFormModel.FormInput.EquipmentType;
                            spec.Name = editFormModel.FormInput.Name;

                            context.SaveChanges();
                            #endregion

                            #region EquipmentSpecificationOption
                            #region Delete
                            context.EquipmentSpecificationOptions.RemoveRange(context.EquipmentSpecificationOptions.Where(x => x.ESpecificationId == new Guid(editFormModel.EquipmentSpecificationId)).ToList());

                            context.SaveChanges();
                            #endregion

                            #region Insert
                            context.ESOptions.AddRange(editFormModel.FormInput.EquipmentSpecificationOptionModels.Select(x => new CF.Models.Maintenance.ESOption
                            {
                                ESOptionId = !string.IsNullOrEmpty(x.EquipmentSpecificationOptionId) ? new Guid(x.EquipmentSpecificationOptionId) : Guid.NewGuid(),
                                ESpecificationId = spec.ESpecificationId,
                                Name = x.Name,
                                Seq = x.Seq
                            }).ToList());

                            context.SaveChanges();
                            #endregion
                            #endregion

                            #region EquipmentSpecificationOptionValue
                            var optionList = editFormModel.FormInput.EquipmentSpecificationOptionModels.Where(x => !string.IsNullOrEmpty(x.EquipmentSpecificationOptionId)).Select(x => x.EquipmentSpecificationOptionId).ToList();

                            var specValueList = context.EquipmentSpecificationOptions.Where(x => x.ESpecificationId == spec.ESpecificationId && !optionList.Contains(x.ESOptionId.ToString())).ToList();

                            foreach (var specValue in specValueList)
                            {
                                specValue.ESOptionId = Guid.Empty;
                            }

                            context.SaveChanges();
                            #endregion
#if !DEBUG
                        trans.Complete();
                    }
#endif
                            result.ReturnSuccessMessage(string.Format("{0} {1} {2}", Resources.Resource.Edit, Resources.Resource.EquipmentSpecification, Resources.Resource.Success));
                        }
                        else
                        {
                            result.ReturnFailedMessage(string.Format("{0} {1}", Resources.Resource.EquipmentSpecificationName, Resources.Resource.Exists));
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

        public static RequestResult GetEditFormModel(string equipmentSpecificationId)
        {
            RequestResult result = new RequestResult();

            try
            {
                using (CFContext context=new CFContext())
                {
                    var query = context.ESpecifications.First(x => x.ESpecificationId == new Guid(equipmentSpecificationId));

                    var model = new EditFormModel()
                    {
                        EquipmentSpecificationId = query.ESpecificationId.ToString(),
                        OrganizationId = query.OrganizationId.ToString(),
                        ParentOrganizationFullName = OrganizationDataAccessor.GetOrganizationFullName(query.OrganizationId),
                        EquipmentTypeSelectItems = new List<SelectListItem>()
                        {
                            Define.DefaultSelectListItem(Resources.Resource.Select),
                            new SelectListItem()
                            {
                                Text = string.Format("{0}...", Resources.Resource.Create),
                                Value = Define.New
                            }
                        },
                        FormInput = new FormInput()
                        {
                            EquipmentType = query.EquipmentType,
                            Name = query.Name
                        },
                        EquipmentSpecificationOptionModels = context.ESOptions.Where(x => x.ESpecificationId == query.ESpecificationId).Select(x => new EquipmentSpecificationOptionModel
                        {
                            EquipmentSpecificationOptionId = x.ESOptionId.ToString(),
                            Name = x.Name,
                            Seq = x.Seq
                        }).OrderBy(x => x.Seq).ToList()
                    };

                    var upStreamOrganizationIds = OrganizationDataAccessor.GetUpStreamOrganizationIds(new Guid(model.OrganizationId), true);

                    model.EquipmentTypeSelectItems.AddRange(context.ESpecifications.Where(x => upStreamOrganizationIds.Contains(x.OrganizationId)).Select(x => x.EquipmentType).Distinct().OrderBy(x => x).Select(x => new SelectListItem
                    {
                        Value = x,
                        Text = x
                    }).ToList());

                    if (!string.IsNullOrEmpty(model.FormInput.EquipmentType) && model.EquipmentTypeSelectItems.Any(x => x.Value == model.FormInput.EquipmentType))
                    {
                        model.EquipmentTypeSelectItems.First(x => x.Value == model.FormInput.EquipmentType).Selected = true;
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
                if (createFormModel.FormInput.EquipmentType == Define.New)
                {
                    result.ReturnFailedMessage(string.Format("{0} {1}", Resources.Resource.Unsupported, Resources.Resource.EquipmentType));
                }
                else
                {
                    using (CFContext context=new CFContext())
                    {
                        var exists = context.ESpecifications.FirstOrDefault(x => x.OrganizationId == new Guid(createFormModel.OrganizationId) && x.EquipmentType == createFormModel.FormInput.EquipmentType && x.Name == createFormModel.FormInput.Name);

                        if (exists == null)
                        {
                            Guid equipmentSpecificationId = Guid.NewGuid();

                            context.ESpecifications.Add(new CF.Models.Maintenance.ESpecification()
                            {
                                ESpecificationId = equipmentSpecificationId,
                                OrganizationId = new Guid(createFormModel.OrganizationId),
                                EquipmentType = createFormModel.FormInput.EquipmentType,
                                Name = createFormModel.FormInput.Name
                            });

                            context.ESOptions.AddRange(createFormModel.FormInput.EquipmentSpecificationOptionModels.Select(x => new CF.Models.Maintenance.ESOption
                            {
                                ESOptionId = Guid.NewGuid(),
                                ESpecificationId = equipmentSpecificationId,
                                Name = x.Name,
                                Seq = x.Seq
                            }).ToList());

                            context.SaveChanges();

                            result.ReturnData(equipmentSpecificationId, string.Format("{0} {1} {2}", Resources.Resource.Create, Resources.Resource.EquipmentSpecification, Resources.Resource.Success));
                        }
                        else
                        {
                            result.ReturnFailedMessage(string.Format("{0} {1}", Resources.Resource.EquipmentSpecificationName, Resources.Resource.Exists));
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

        public static RequestResult GetCreateFormModel(string organizationId, string equipmentType)
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
                        EquipmentTypeSelectItems = new List<SelectListItem>()
                        {
                            Define.DefaultSelectListItem(Resources.Resource.Select),
                            new SelectListItem()
                            {
                                Text = string.Format("{0}...", Resources.Resource.Create),
                                Value = Define.New
                            }
                        },
                        FormInput = new FormInput()
                        {
                            EquipmentType = equipmentType
                        }
                    };

                    var upStreamOrganizationIds = OrganizationDataAccessor.GetUpStreamOrganizationIds(new Guid(organizationId), true);

                    model.EquipmentTypeSelectItems.AddRange(context.ESpecifications.Where(x => upStreamOrganizationIds.Contains(x.OrganizationId)).Select(x => x.EquipmentType).Distinct().OrderBy(x => x).Select(x => new SelectListItem
                    {
                        Value = x,
                        Text = x
                    }).ToList());

                    if (!string.IsNullOrEmpty(equipmentType) && model.EquipmentTypeSelectItems.Any(x => x.Value == equipmentType))
                    {
                        model.EquipmentTypeSelectItems.First(x => x.Value == equipmentType).Selected = true;
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

        public static RequestResult GetDetailViewModel(string equipmentSpecificationId, Account account)
        {
            RequestResult result = new RequestResult();

            try
            {
                using (CFContext context=new CFContext())
                {
                    var specification = context.ESpecifications.First(x => x.ESpecificationId == new Guid(equipmentSpecificationId));

                    result.ReturnData(new DetailViewModel()
                    {
                        EquipmentSpecificationId = specification.ESpecificationId.ToString(),
                        Permission = account.OrganizationPermission(specification.OrganizationId),
                        OrganizationId = specification.OrganizationId.ToString(),
                        ParentOrganizationFullName = OrganizationDataAccessor.GetOrganizationFullName(specification.OrganizationId),
                        EquipmentType = specification.EquipmentType,
                        Name = specification.Name,
                        EquipmentSpecificationOptonNameList = context.ESOptions.Where(x => x.ESpecificationId == specification.ESpecificationId).OrderBy(x => x.Seq).Select(x => x.Name).ToList()
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

                    var query = context.ESpecifications.Where(x => downStreamOrganizationIds.Contains(x.OrganizationId) && account.QueryableOrganizationIds.Contains(x.OrganizationId)).AsQueryable();

                    if (!string.IsNullOrEmpty(queryParameters.EquipmentType))
                    {
                        query = query.Where(x => x.OrganizationId == new Guid(queryParameters.OrganizationId) && x.EquipmentType == queryParameters.EquipmentType);
                    }

                    if (!string.IsNullOrEmpty(queryParameters.Keyword))
                    {
                        query = query.Where(x => x.Name.Contains(queryParameters.Keyword));
                    }

                    result.ReturnData(new GridViewModel()
                    {
                        OrganizationId = queryParameters.OrganizationId,
                        Permission = account.OrganizationPermission(new Guid(queryParameters.OrganizationId)),
                        EquipmentType = queryParameters.EquipmentType,
                        FullOrganizationName = OrganizationDataAccessor.GetOrganizationFullName(new Guid(queryParameters.OrganizationId)),
                        OrganizationName = OrganizationDataAccessor.GetOrganizationName(new Guid(queryParameters.OrganizationId)),
                        Items = query.ToList().Select(x => new GridItem()
                        {
                            EquipmentSpecificationId = x.ESpecificationId.ToString(),
                            Permission = account.OrganizationPermission(x.OrganizationId),
                            OrganizationName = OrganizationDataAccessor.GetOrganizationName(x.OrganizationId),
                            EquipmentType = x.EquipmentType,
                            Name = x.Name
                        }).OrderBy(x => x.OrganizationName).ThenBy(x => x.EquipmentType).ThenBy(x => x.Name).ToList()
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
                var treeItems = new List<TreeItem>();

                var attributes = new Dictionary<Define.EnumTreeAttribute, string>()
                {
                    { Define.EnumTreeAttribute.NodeType, string.Empty },
                    { Define.EnumTreeAttribute.ToolTip, string.Empty },
                    { Define.EnumTreeAttribute.OrganizationId, string.Empty },
                    { Define.EnumTreeAttribute.EquipmentType, string.Empty },
                    { Define.EnumTreeAttribute.EquipmentSpecificationId, string.Empty }
                };

                using (CFContext context=new CFContext())
                {
                    var organization = organizationList.First(x => x.OrganizationId == organizationId);

                    var treeItem = new TreeItem() { Title = organization.Name };

                    attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.Organization.ToString();
                    attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", organization.OId, organization.Name);
                    attributes[Define.EnumTreeAttribute.OrganizationId] = organization.OrganizationId.ToString();
                    attributes[Define.EnumTreeAttribute.EquipmentType] = string.Empty;
                    attributes[Define.EnumTreeAttribute.EquipmentSpecificationId] = string.Empty;

                    foreach (var attribute in attributes)
                    {
                        treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                    }

                    var haveDownStreamOrganization = organizationList.Any(x => x.ParentId == organization.OrganizationId && account.VisibleOrganizationIds.Contains(x.OrganizationId));

                    var haveEquipmentSpecification = account.QueryableOrganizationIds.Contains(organization.OrganizationId) && context.ESpecifications.Any(x => x.OrganizationId == organization.OrganizationId);

                    if (haveDownStreamOrganization || haveEquipmentSpecification)
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
    }
}
