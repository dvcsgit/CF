using CF;
using Models.Authentication;
using Models.Maintenance.MSpecification;
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
    public class MSpecificationDataAccessor
    {
        public static RequestResult GetTreeItems(List<Organization> organizationList, Guid refOrganizationId, Guid organizationId, string materialType)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                var treeItems = new List<TreeItem>();

                var attributes = new Dictionary<Define.EnumTreeAttribute, string>()
                {
                    {Define.EnumTreeAttribute.NodeType,string.Empty },
                    {Define.EnumTreeAttribute.ToolTip,string.Empty },
                    {Define.EnumTreeAttribute.OrganizationId,string.Empty },
                    {Define.EnumTreeAttribute.MaterialType,string.Empty },
                    {Define.EnumTreeAttribute.MaterialSpecificationId,string.Empty }
                };

                using(CFContext context =new CFContext())
                {
                    if (string.IsNullOrEmpty(materialType))
                    {
                        var materialTypes = context.MSpecifications.Where(x => x.OrganizationId == organizationId).Select(x => x.MaterialType).Distinct().OrderBy(x => x).ToList();

                        foreach(var type in materialTypes)
                        {
                            var treeItem = new TreeItem() { Title = type };

                            attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.MaterialType.ToString();
                            attributes[Define.EnumTreeAttribute.ToolTip] = type;
                            attributes[Define.EnumTreeAttribute.OrganizationId] = organizationId.ToString();
                            attributes[Define.EnumTreeAttribute.MaterialType] = type;
                            attributes[Define.EnumTreeAttribute.MaterialSpecificationId] = string.Empty;

                            foreach (var attribute in attributes)
                            {
                                treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                            }

                            treeItem.State = "closed";

                            treeItems.Add(treeItem);
                        }

                        var availableOrganizationIds = OrganizationDataAccessor.GetUpStreamOrganizationIds(refOrganizationId, true);

                        var organizations = organizationList.Where(x => x.ParentId == organizationId && availableOrganizationIds.Contains(x.OrganizationId)).OrderBy(x => x.OId).ToList();

                        foreach(var organization in organizations)
                        {
                            var downStreamOrganizationIds = OrganizationDataAccessor.GetDownStreamOrganizationIds(organization.OrganizationId, true);
                            if (context.MSpecifications.Any(x => downStreamOrganizationIds.Contains(x.OrganizationId) && availableOrganizationIds.Contains(x.OrganizationId)))
                            {
                                var treeItem = new TreeItem() { Title = organization.Name };

                                attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.Organization.ToString();
                                attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", organization.OId, organization.Name);
                                attributes[Define.EnumTreeAttribute.OrganizationId] = organization.OrganizationId.ToString();
                                attributes[Define.EnumTreeAttribute.MaterialType] = string.Empty;
                                attributes[Define.EnumTreeAttribute.MaterialSpecificationId] = string.Empty;

                                foreach (var attribute in attributes)
                                {
                                    treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                                }

                                treeItem.State = "closed";

                                treeItems.Add(treeItem);
                            }
                        }
                    }
                    else
                    {
                        var materialSpecifications = context.MSpecifications.Where(x => x.OrganizationId == organizationId && x.MaterialType == materialType).OrderBy(x => x.Name).ToList();
                        foreach(var specification in materialSpecifications)
                        {
                            var treeItem = new TreeItem() { Title = specification.Name };

                            attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.MaterialSpecification.ToString();
                            attributes[Define.EnumTreeAttribute.ToolTip] = specification.Name;
                            attributes[Define.EnumTreeAttribute.OrganizationId] = specification.OrganizationId.ToString();
                            attributes[Define.EnumTreeAttribute.MaterialType] = specification.MaterialType;
                            attributes[Define.EnumTreeAttribute.MaterialSpecificationId] = specification.MSpecificationId.ToString();

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
                //throw;
            }

            return requestResult;
        }

        public static RequestResult GetCopyFormModel(string materialSpecificationId)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                using (CFContext context=new CFContext())
                {
                    var specification = context.MSpecifications.First(x => x.MSpecificationId == new Guid(materialSpecificationId));

                    var createFormModel = new CreateFormModel()
                    {
                        OrganizationId = specification.OrganizationId.ToString(),
                        ParentOrganizationFullName = OrganizationDataAccessor.GetOrganizationFullName(specification.OrganizationId),
                        MaterialTypeSelectItems = new List<SelectListItem>()
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
                            MaterialType = specification.MaterialType
                        },
                        MaterialSpecificationOptionModels = context.MSOptions.Where(x => x.MSpecificationId == specification.MSpecificationId).Select(x => new MaterialSpecificationOptionModel
                        {
                            Name = x.Name,
                            Seq = x.Seq
                        }).OrderBy(x => x.Seq).ToList()
                    };

                    var upStreamOrganizationIds = OrganizationDataAccessor.GetUpStreamOrganizationIds(specification.OrganizationId, true);

                    createFormModel.MaterialTypeSelectItems.AddRange(context.MSpecifications.Where(x => upStreamOrganizationIds.Contains(x.OrganizationId)).Select(x => x.MaterialType).Distinct().OrderBy(x => x).Select(x => new SelectListItem
                    {
                        Value = x,
                        Text = x
                    }).ToList());

                    createFormModel.MaterialTypeSelectItems.First(x => x.Value == specification.MaterialType).Selected = true;

                    requestResult.ReturnData(createFormModel);
                }
            }
            catch (Exception e)
            {
                var error = new Error(MethodBase.GetCurrentMethod(), e);

                Logger.Log(error);

                requestResult.ReturnError(error);
            }

            return requestResult;
        }

        public static RequestResult Delete(List<string> selectedList)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                using (CFContext context=new CFContext())
                {
                    DeleteHelper.DeleteMaterialSpecificaton(context, selectedList);

                    context.SaveChanges();
                }

                requestResult.ReturnSuccessMessage(string.Format("{0} {1} {2}", Resources.Resource.Delete, Resources.Resource.MaterialSpecification, Resources.Resource.Success));
            }
            catch (Exception e)
            {
                var error = new Error(MethodBase.GetCurrentMethod(), e);

                Logger.Log(error);

                requestResult.ReturnError(error);
            }

            return requestResult;
        }

        public static object Edit(EditFormModel editFormModel)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                if (editFormModel.FormInput.MaterialType == Define.New)
                {
                    requestResult.ReturnFailedMessage(string.Format("{0} {1}", Resources.Resource.Unsupported, Resources.Resource.MaterialType));
                }
                else
                {
                    using (CFContext context=new CFContext())
                    {

                        var specification = context.MSpecifications.First(x => x.MSpecificationId == new Guid(editFormModel.MaterialSpecificationId));

                        var exists = context.MSpecifications.FirstOrDefault(x => x.MSpecificationId != specification.MSpecificationId && x.OrganizationId == specification.OrganizationId && x.MaterialType == editFormModel.FormInput.MaterialType && x.Name == editFormModel.FormInput.Name);

                        if (exists == null)
                        {
#if !DEBUG
                    using (TransactionScope trans = new TransactionScope())
                    {
#endif
                            #region MaterialSpecification
                            specification.Name = editFormModel.FormInput.Name;
                            specification.MaterialType = editFormModel.FormInput.MaterialType;

                            context.SaveChanges();
                            #endregion

                            #region MaterialSpecificationOption
                            #region Delete
                            context.MaterialSpecificationOptions.RemoveRange(context.MaterialSpecificationOptions.Where(x => x.MSpecificationId == new Guid(editFormModel.MaterialSpecificationId)).ToList());

                            context.SaveChanges();
                            #endregion

                            #region Insert
                            context.MSOptions.AddRange(editFormModel.FormInput.MaterialSpecificationOptionModels.Select(x => new CF.Models.Maintenance.MSOption
                            {
                                MSOptionId = !string.IsNullOrEmpty(x.MaterialSpecificationOptionId) ? new Guid(x.MaterialSpecificationOptionId) : Guid.NewGuid(),
                                MSpecificationId = specification.MSpecificationId,
                                Name = x.Name,
                                Seq = x.Seq
                            }).ToList());

                            context.SaveChanges();
                            #endregion
                            #endregion

                            #region MaterialSpecificaitonOptionValue
                            var optionList = editFormModel.FormInput.MaterialSpecificationOptionModels.Where(x => !string.IsNullOrEmpty(x.MaterialSpecificationOptionId)).Select(x => x.MaterialSpecificationOptionId).ToList();

                            var specValueList = context.MaterialSpecificationOptions.Where(x => x.MSpecificationId == specification.MSpecificationId && !optionList.Contains(x.MSOptionId.ToString())).ToList();

                            foreach (var specValue in specValueList)
                            {
                                specValue.MSOptionId = Guid.Empty;
                            }

                            context.SaveChanges();
                            #endregion
#if !DEBUG
                        trans.Complete();
                    }
#endif
                            requestResult.ReturnSuccessMessage(string.Format("{0} {1} {2}", Resources.Resource.Edit, Resources.Resource.MaterialSpecification, Resources.Resource.Success));
                        }
                        else
                        {
                            requestResult.ReturnFailedMessage(string.Format("{0} {1}", Resources.Resource.MaterialSpecificationName, Resources.Resource.Exists));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                var error = new Error(MethodBase.GetCurrentMethod(), e);

                Logger.Log(error);

                requestResult.ReturnError(error);
            }

            return requestResult;
        }

        public static RequestResult GetEditFormModel(string materialSpecificationId)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                using (CFContext context=new CFContext())
                {
                    var specification = context.MSpecifications.First(x => x.MSpecificationId == new Guid(materialSpecificationId));

                    var editFormModel = new EditFormModel()
                    {
                        MaterialSpecificationId = specification.MSpecificationId.ToString(),
                        OrganizationId = specification.OrganizationId.ToString(),
                        ParentOrganizationFullName = OrganizationDataAccessor.GetOrganizationFullName(specification.OrganizationId),
                        MaterialTypeSelectItems = new List<SelectListItem>()
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
                            MaterialType = specification.MaterialType,
                            Name = specification.Name
                        },
                        MaterialSpecificationOptionModels = context.MSOptions.Where(x => x.MSpecificationId == specification.MSpecificationId).Select(x => new MaterialSpecificationOptionModel
                        {
                            MaterialSpecificationOptionId = x.MSOptionId.ToString(),
                            Name = x.Name,
                            Seq = x.Seq
                        }).OrderBy(x => x.Seq).ToList()
                    };

                    var upStreamOrganizationIds = OrganizationDataAccessor.GetUpStreamOrganizationIds(specification.OrganizationId, true);

                    editFormModel.MaterialTypeSelectItems.AddRange(context.MSpecifications.Where(x => upStreamOrganizationIds.Contains(x.OrganizationId)).Select(x => x.MaterialType).Distinct().OrderBy(x => x).Select(x => new SelectListItem
                    {
                        Value = x,
                        Text = x
                    }).ToList());

                    if (!string.IsNullOrEmpty(specification.MaterialType) && editFormModel.MaterialTypeSelectItems.Any(x => x.Value == specification.MaterialType))
                    {
                        editFormModel.MaterialTypeSelectItems.First(x => x.Value == specification.MaterialType).Selected = true;
                    }

                    requestResult.ReturnData(editFormModel);
                }
            }
            catch (Exception e)
            {
                var error = new Error(MethodBase.GetCurrentMethod(), e);

                Logger.Log(error);

                requestResult.ReturnError(error);
            }

            return requestResult;
        }

        public static object Create(CreateFormModel createFormModel)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                if (createFormModel.FormInput.MaterialType == Define.New)
                {
                    requestResult.ReturnFailedMessage(string.Format("{0} {1}", Resources.Resource.Unsupported, Resources.Resource.MaterialType));
                }
                else
                {
                    using (CFContext context=new CFContext())
                    {
                        var exists = context.MSpecifications.FirstOrDefault(x => x.OrganizationId == new Guid(createFormModel.OrganizationId) && x.MaterialType == createFormModel.FormInput.MaterialType && x.Name == createFormModel.FormInput.Name);

                        if (exists == null)
                        {
                            Guid materialSpecificationId = Guid.NewGuid();

                            context.MSpecifications.Add(new CF.Models.Maintenance.MSpecification()
                            {
                                MSpecificationId = materialSpecificationId,
                                OrganizationId = new Guid(createFormModel.OrganizationId),
                                MaterialType = createFormModel.FormInput.MaterialType,
                                Name = createFormModel.FormInput.Name
                            });

                            context.MSOptions.AddRange(createFormModel.FormInput.MaterialSpecificationOptionModels.Select(x => new CF.Models.Maintenance.MSOption
                            {
                                MSOptionId = Guid.NewGuid(),
                                MSpecificationId = materialSpecificationId,
                                Name = x.Name,
                                Seq = x.Seq
                            }).ToList());

                            context.SaveChanges();

                            requestResult.ReturnData(materialSpecificationId, string.Format("{0} {1} {2}", Resources.Resource.Create, Resources.Resource.MaterialSpecification, Resources.Resource.Success));
                        }
                        else
                        {
                            requestResult.ReturnFailedMessage(string.Format("{0} {1}", Resources.Resource.MaterialSpecificationName, Resources.Resource.Exists));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                var error = new Error(MethodBase.GetCurrentMethod(), e);

                Logger.Log(error);

                requestResult.ReturnError(error);
            }

            return requestResult;
        }

        public static RequestResult GetCreateFormModel(string organizationId, string materialType)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                using (CFContext context=new CFContext())
                {
                    var createFormModel = new CreateFormModel()
                    {
                        OrganizationId =organizationId,
                        ParentOrganizationFullName = OrganizationDataAccessor.GetOrganizationFullName(new Guid(organizationId)),
                        MaterialTypeSelectItems = new List<SelectListItem>()
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
                            MaterialType = materialType
                        }
                    };

                    var upStreamOrganizationIds = OrganizationDataAccessor.GetUpStreamOrganizationIds(new Guid(organizationId), true);

                    createFormModel.MaterialTypeSelectItems.AddRange(context.MSpecifications .Where(x => upStreamOrganizationIds.Contains(x.OrganizationId)).Select(x => x.MaterialType).Distinct().OrderBy(x => x).Select(x => new SelectListItem
                    {
                        Value = x,
                        Text = x
                    }).ToList());

                    if (!string.IsNullOrEmpty(materialType) && createFormModel.MaterialTypeSelectItems.Any(x => x.Value == materialType))
                    {
                        createFormModel.MaterialTypeSelectItems.First(x => x.Value == materialType).Selected = true;
                    }

                    requestResult.ReturnData(createFormModel);
                }
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                requestResult.ReturnError(err);
            }

            return requestResult;
        }

        public static RequestResult GetDetailViewModel(string materialSpecificationId, Account account)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                using (CFContext context = new CFContext())
                {
                    var specification = context.MSpecifications.First(x => x.MSpecificationId == new Guid(materialSpecificationId));
                    requestResult.ReturnData(new DetailViewModel()
                    {
                        MaterialSpecificationId = specification.MSpecificationId.ToString(),
                        Name = specification.Name,
                        OrganizationId = specification.OrganizationId.ToString(),
                        ParentOrganizationFullName = OrganizationDataAccessor.GetOrganizationFullName(specification.OrganizationId),
                        MaterialType = specification.MaterialType,
                        Permission = account.OrganizationPermission(specification.OrganizationId),
                        MaterialSpecificationOptionNames = context.MSOptions.Where(x => x.MSpecificationId == specification.MSpecificationId).OrderBy(x => x.Seq).Select(x => x.Name).ToList()
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
                using (CFContext context = new CFContext())
                {
                    var downStreamOrganizationList = OrganizationDataAccessor.GetDownStreamOrganizationIds(new Guid(queryParameters.OrganizationId), true);

                    var query = context.MSpecifications.Where(x => downStreamOrganizationList.Contains(x.OrganizationId) && account.QueryableOrganizationIds.Contains(x.OrganizationId)).AsQueryable();

                    if (!string.IsNullOrEmpty(queryParameters.MaterialType))
                    {
                        query = query.Where(x => x.OrganizationId ==new Guid(queryParameters.OrganizationId) && x.MaterialType == queryParameters.MaterialType);
                    }

                    if (!string.IsNullOrEmpty(queryParameters.Keyword))
                    {
                        query = query.Where(x => x.Name.Contains(queryParameters.Keyword));
                    }

                    requestResult.ReturnData(new GridViewModel()
                    {
                        Permission = account.OrganizationPermission(new Guid(queryParameters.OrganizationId)),
                        OrganizationId = queryParameters.OrganizationId,
                        MaterialType = queryParameters.MaterialType,
                        OrganizationName = OrganizationDataAccessor.GetOrganizationName(new Guid(queryParameters.OrganizationId)),
                        FullOrganizationName = OrganizationDataAccessor.GetOrganizationFullName(new Guid(queryParameters.OrganizationId)),
                        Items = query.ToList().Select(x => new GridItem()
                        {
                            Permission = account.OrganizationPermission(x.OrganizationId),
                            MaterialSpecificationId = x.MSpecificationId.ToString(),
                            OrganizationName = OrganizationDataAccessor.GetOrganizationName(x.OrganizationId),
                            MaterialType = x.MaterialType,
                            Name = x.Name
                        }).OrderBy(x => x.OrganizationName).ThenBy(x => x.MaterialType).ThenBy(x => x.Name).ToList()
                    });
                }
            }
            catch (Exception e)
            {
                var error = new Error(MethodBase.GetCurrentMethod(), e);

                Logger.Log(error);

                requestResult.ReturnError(error);
            }

            return requestResult;
        }

        public static RequestResult GetRootTreeItem(List<Organization> organizationList, Guid rootOrganizationId, Account account)
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
                    { Define.EnumTreeAttribute.MaterialType, string.Empty },
                    { Define.EnumTreeAttribute.MaterialSpecificationId, string.Empty }
                };

                using (CFContext context = new CFContext())
                {
                    var organization = organizationList.First(x => x.OrganizationId == rootOrganizationId);

                    var treeItem = new TreeItem() { Title = organization.Name };

                    attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.Organization.ToString();
                    attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", organization.OId, organization.Name);
                    attributes[Define.EnumTreeAttribute.OrganizationId] = organization.OrganizationId.ToString();
                    attributes[Define.EnumTreeAttribute.MaterialType] = string.Empty;
                    attributes[Define.EnumTreeAttribute.MaterialSpecificationId] = string.Empty;

                    foreach (var attribute in attributes)
                    {
                        treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                    }

                    var haveDownStreamOrganization = organizationList.Any(x => x.ParentId == organization.OrganizationId && account.VisibleOrganizationIds.Contains(x.OrganizationId));

                    var haveMaterialSpecification = account.QueryableOrganizationIds.Contains(organization.OrganizationId) && context.MSpecifications.Any(x => x.OrganizationId == organization.OrganizationId);

                    if (haveDownStreamOrganization || haveMaterialSpecification)
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
            }

            return requestResult;
        }

        public static RequestResult GetTreeItems(List<Models.Shared.Organization> organizationList, Guid organizationId, string materialType, Account account)
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
                    { Define.EnumTreeAttribute.MaterialType, string.Empty },
                    { Define.EnumTreeAttribute.MaterialSpecificationId, string.Empty }
                };

                using (CFContext context = new CFContext())
                {
                    if (string.IsNullOrEmpty(materialType))
                    {
                        if (account.QueryableOrganizationIds.Contains(organizationId))
                        {
                            var materialTypes = context.MSpecifications.Where(x => x.OrganizationId == organizationId).Select(x => x.MaterialType).Distinct().OrderBy(x => x).ToList();

                            foreach (var type in materialTypes)
                            {
                                var treeItem = new TreeItem() { Title = type };

                                attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.MaterialType.ToString();
                                attributes[Define.EnumTreeAttribute.ToolTip] = type;
                                attributes[Define.EnumTreeAttribute.OrganizationId] = organizationId.ToString();
                                attributes[Define.EnumTreeAttribute.MaterialType] = type;
                                attributes[Define.EnumTreeAttribute.MaterialSpecificationId] = string.Empty;

                                foreach (var attribute in attributes)
                                {
                                    treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                                }

                                treeItem.State = "closed";

                                treeItems.Add(treeItem);
                            }
                        }

                        var organizations = organizationList.Where(x => x.ParentId == organizationId && account.VisibleOrganizationIds.Contains(x.OrganizationId)).OrderBy(x => x.OId).ToList();

                        foreach (var organization in organizations)
                        {
                            var treeItem = new TreeItem() { Title = organization.Name };

                            attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.Organization.ToString();
                            attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", organization.OId, organization.Name);
                            attributes[Define.EnumTreeAttribute.OrganizationId] = organization.OrganizationId.ToString();
                            attributes[Define.EnumTreeAttribute.MaterialType] = string.Empty;
                            attributes[Define.EnumTreeAttribute.MaterialSpecificationId] = string.Empty;

                            foreach (var attribute in attributes)
                            {
                                treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                            }

                            var haveDownStreamOrganization = organizationList.Any(x => x.ParentId == organization.OrganizationId && account.VisibleOrganizationIds.Contains(x.OrganizationId));

                            var haveMaterialSpecification = account.QueryableOrganizationIds.Contains(organization.OrganizationId) && context.MSpecifications.Any(x => x.OrganizationId == organization.OrganizationId);

                            if (haveDownStreamOrganization || haveMaterialSpecification)
                            {
                                treeItem.State = "closed";
                            }

                            treeItems.Add(treeItem);
                        }
                    }
                    else
                    {
                        var specifications = context.MSpecifications.Where(x => x.OrganizationId == organizationId && x.MaterialType == materialType).OrderBy(x => x.Name).ToList();

                        foreach (var specification in specifications)
                        {
                            var treeItem = new TreeItem() { Title = specification.Name };

                            attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.MaterialSpecification.ToString();
                            attributes[Define.EnumTreeAttribute.ToolTip] = specification.Name;
                            attributes[Define.EnumTreeAttribute.OrganizationId] = specification.OrganizationId.ToString();
                            attributes[Define.EnumTreeAttribute.MaterialType] = specification.MaterialType;
                            attributes[Define.EnumTreeAttribute.MaterialSpecificationId] = specification.MSpecificationId.ToString();

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
    }
}
