using CF;
using Models.Authentication;
using Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Utility;
using Utility.Models;
using Models.Maintenance.Material;
using System.Web.Mvc;

namespace DataAccessor.Maintenance
{
    public class MaterialDataAccessor
    {
        public static RequestResult GetTreeItems(List<Models.Shared.Organization> organizations,Guid organizationId,string equipmentType,Account account)
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
                    {Define.EnumTreeAttribute.EquipmentType,string.Empty },
                    {Define.EnumTreeAttribute.MaterialId,string.Empty }
                };

                using(CFContext context=new CFContext())
                {
                    if (string.IsNullOrEmpty(equipmentType))
                    {
                        if (account.QueryableOrganizationIds.Contains(organizationId))
                        {
                            var materialTypes = context.Materials.Where(x => x.OrganizationId == organizationId).Select(x => x.MaterialType).Distinct().OrderBy(x => x).ToList();
                            foreach(var type in materialTypes)
                            {
                                var treeItem = new TreeItem() { Title = type };

                                attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.MaterialType.ToString();
                                attributes[Define.EnumTreeAttribute.ToolTip] = type;
                                attributes[Define.EnumTreeAttribute.OrganizationId] = organizationId.ToString();
                                attributes[Define.EnumTreeAttribute.MaterialType] = type;
                                attributes[Define.EnumTreeAttribute.MaterialId] = string.Empty;

                                foreach (var attribute in attributes)
                                {
                                    treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                                }

                                treeItem.State = "closed";

                                treeItems.Add(treeItem);
                            }
                        }

                        var newOrganizations = organizations.Where(x => x.ParentId == organizationId && account.VisibleOrganizationIds.Contains(x.OrganizationId)).OrderBy(x => x.OId).ToList();

                        foreach (var organization in newOrganizations)
                        {
                            var treeItem = new TreeItem() { Title = organization.Name };

                            attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.Organization.ToString();
                            attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", organization.OId, organization.Name);
                            attributes[Define.EnumTreeAttribute.OrganizationId] = organization.OrganizationId.ToString();
                            attributes[Define.EnumTreeAttribute.MaterialType] = string.Empty;
                            attributes[Define.EnumTreeAttribute.MaterialId] = string.Empty;

                            foreach (var attribute in attributes)
                            {
                                treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                            }

                            var haveDownStreamOrganization = organizations.Any(x => x.ParentId == organization.OrganizationId && account.VisibleOrganizationIds.Contains(x.OrganizationId));

                            var haveMaterial = account.QueryableOrganizationIds.Contains(organization.OrganizationId) && context.Materials.Any(x => x.OrganizationId == organization.OrganizationId);

                            if (haveDownStreamOrganization || haveMaterial)
                            {
                                treeItem.State = "closed";
                            }

                            treeItems.Add(treeItem);
                        }
                    }
                    else
                    {
                        var materials = context.Materials.Where(x => x.OrganizationId == organizationId && x.MaterialType == equipmentType).OrderBy(x => x.MId).ToList();

                        foreach (var material in materials)
                        {
                            var treeItem = new TreeItem() { Title = material.Name };

                            attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.Material.ToString();
                            attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", material.MId, material.Name);
                            attributes[Define.EnumTreeAttribute.OrganizationId] = material.OrganizationId.ToString();
                            attributes[Define.EnumTreeAttribute.EquipmentType] = material.MaterialType;
                            attributes[Define.EnumTreeAttribute.MaterialId] = material.MaterialId.ToString();

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

        public static RequestResult GetTreeItems(List<Models.Shared.Organization> organizationList, Guid refOrganizationId, Guid organizationId, string materialType)
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
                    { Define.EnumTreeAttribute.MaterialId, string.Empty }
                };

                using (CFContext context=new CFContext())
                {
                    if (string.IsNullOrEmpty(materialType))
                    {
                        var materialTypes = context.Materials.Where(x => x.OrganizationId == organizationId).Select(x => x.MaterialType).Distinct().OrderBy(x => x).ToList();

                        foreach (var type in materialTypes)
                        {
                            var treeItem = new TreeItem() { Title = type };

                            attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.EquipmentType.ToString();
                            attributes[Define.EnumTreeAttribute.ToolTip] = type;
                            attributes[Define.EnumTreeAttribute.OrganizationId] = organizationId.ToString();
                            attributes[Define.EnumTreeAttribute.EquipmentType] = type;
                            attributes[Define.EnumTreeAttribute.MaterialId] = string.Empty;

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
                            var downStreamOrganizationIds = OrganizationDataAccessor.GetDownStreamOrganizationIds(organization.OrganizationId, true);

                            if (context.Materials.Any(x => downStreamOrganizationIds.Contains(x.OrganizationId) && availableOrganizationIds.Contains(x.OrganizationId)))
                            {
                                var treeItem = new TreeItem() { Title = organization.Name };

                                attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.Organization.ToString();
                                attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", organization.OId, organization.Name);
                                attributes[Define.EnumTreeAttribute.OrganizationId] = organization.OrganizationId.ToString();
                                attributes[Define.EnumTreeAttribute.EquipmentType] = string.Empty;
                                attributes[Define.EnumTreeAttribute.MaterialId] = string.Empty;

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
                        var materialList = context.Materials.Where(x => x.OrganizationId == organizationId && x.MaterialType == materialType).OrderBy(x => x.MId).ToList();

                        foreach (var material in materialList)
                        {
                            var treeItem = new TreeItem() { Title = material.Name };

                            attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.Material.ToString();
                            attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", material.MId, material.Name);
                            attributes[Define.EnumTreeAttribute.OrganizationId] = material.OrganizationId.ToString();
                            attributes[Define.EnumTreeAttribute.MaterialType] = material.MaterialType;
                            attributes[Define.EnumTreeAttribute.MaterialId] = material.MaterialId.ToString();

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

        public static RequestResult GetEditFormModel(Guid materialId)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                using (CFContext context=new CFContext())
                {
                    var material = context.Materials.First(x => x.MaterialId == materialId);

                    var model = new EditFormModel()
                    {
                        MaterialId = material.MaterialId.ToString(),
                        OrganizationId = material.OrganizationId.ToString(),
                        ParentOrganizationFullName = OrganizationDataAccessor.GetOrganizationFullName(material.OrganizationId),
                        MaterialTypeSelectItems = new List<SelectListItem>()
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
                            MaterialType = material.MaterialType,
                            MId = material.MId,
                            MaterialName = material.Name,
                            Quantity = material.Quantity
                        },
                        MaterialSpecificationModels = (from x in context.MaterialSpecificationOptions
                                    join s in context.MSpecifications
                                    on x.MSpecificationId equals s.MSpecificationId
                                    where x.MaterialId == material.MaterialId
                                    select new MaterialSpecificationModel
                                    {
                                        MaterialSpecificationId = s.MSpecificationId.ToString(),
                                        Name = s.Name,
                                        MaterialSpecificationOptionId = x.MSOptionId.ToString(),
                                        Value = x.Value,
                                        Seq = x.Seq,
                                        MaterialSpecificationOptionModels = context.MSOptions.Where(o => o.MSpecificationId == s.MSpecificationId).Select(o => new MaterialSpecificationOptionModel
                                        {
                                            MaterialSpecificationOptionId = o.MSOptionId.ToString(),
                                            Name = o.Name,
                                            Seq = o.Seq,
                                            MaterialSpecificationId = o.MSpecificationId.ToString()
                                        }).OrderBy(o => o.Name).ToList()
                                    }).OrderBy(x => x.Seq).ToList()
                    };

                    var upStreamOrganizationIds = OrganizationDataAccessor.GetUpStreamOrganizationIds(new Guid(model.OrganizationId), true);

                    model.MaterialTypeSelectItems.AddRange(context.Materials.Where(x => upStreamOrganizationIds.Contains(x.OrganizationId)).Select(x => x.MaterialType).Distinct().OrderBy(x => x).Select(x => new SelectListItem
                    {
                        Value = x,
                        Text = x
                    }).ToList());

                    if (!string.IsNullOrEmpty(model.FormInput.MaterialType) && model.MaterialTypeSelectItems.Any(x => x.Value == model.FormInput.MaterialType))
                    {
                        model.MaterialTypeSelectItems.First(x => x.Value == model.FormInput.MaterialType).Selected = true;
                    }

                    requestResult.ReturnData(model);
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

        public static RequestResult GetCopyFormModel(Guid materialId)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                using (CFContext context=new CFContext())
                {
                    var material = context.Materials.First(x => x.MaterialId == materialId);

                    var createFormModel = new CreateFormModel()
                    {
                        OrganizationId = material.OrganizationId.ToString(),
                        ParentOrganizationFullName = OrganizationDataAccessor.GetOrganizationFullName(material.OrganizationId),
                        MaterialTypeSelectItems = new List<SelectListItem>()
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
                            MaterialType = material.MaterialType
                        },
                        MaterialSpecificationModels = (from x in context.MaterialSpecificationOptions
                                    join s in context.MSpecifications
                                    on x.MSpecificationId equals s.MSpecificationId
                                    where x.MaterialId == material.MaterialId
                                    select new MaterialSpecificationModel
                                    {
                                        MaterialSpecificationId = s.MSpecificationId.ToString(),
                                        Name = s.Name,
                                        MaterialSpecificationOptionId = x.MSOptionId.ToString(),
                                        Value = x.Value,
                                        Seq = x.Seq,
                                        MaterialSpecificationOptionModels = context.MSOptions.Where(o => o.MSpecificationId == s.MSpecificationId).Select(o => new MaterialSpecificationOptionModel
                                        {
                                            MaterialSpecificationId = o.MSpecificationId.ToString(),
                                            Seq = o.Seq,
                                            Name = o.Name,
                                            MaterialSpecificationOptionId = o.MSOptionId.ToString()
                                        }).OrderBy(o => o.Seq).ToList()
                                    }).OrderBy(x => x.Seq).ToList()
                    };

                    var upStreamOrganizationIds = OrganizationDataAccessor.GetUpStreamOrganizationIds(material.OrganizationId, true);

                    createFormModel.MaterialTypeSelectItems.AddRange(context.Materials.Where(x => upStreamOrganizationIds.Contains(x.OrganizationId)).Select(x => x.MaterialType).Distinct().OrderBy(x => x).Select(x => new SelectListItem
                    {
                        Value = x,
                        Text = x
                    }).ToList());

                    createFormModel.MaterialTypeSelectItems.First(x => x.Value == material.MaterialType).Selected = true;

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
                    DeleteHelper.DeleteMaterial(context, selectedList);

                    context.SaveChanges();
                }

                requestResult.ReturnSuccessMessage(string.Format("{0} {1} {2}", Resources.Resource.Delete, Resources.Resource.Material, Resources.Resource.Success));
            }
            catch (Exception e)
            {
                var error = new Error(MethodBase.GetCurrentMethod(), e);

                Logger.Log(error);

                requestResult.ReturnError(error);
            }

            return requestResult;
        }

        public static RequestResult UploadPhoto(string uniqueID, string extension)
        {
            RequestResult result = new RequestResult();

            try
            {
                using (CFContext context=new CFContext())
                {
                    var material = context.Materials.First(x => x.MaterialId == new Guid(uniqueID));

                    //material.Extension = Extension;

                    //db.SaveChanges();

                    result.Success();
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

        public static object DeletePhoto(string uniqueID)
        {
            RequestResult result = new RequestResult();

            try
            {
                using (CFContext context = new CFContext())
                {
                    var material = context.Materials.First(x => x.MaterialId == new Guid(uniqueID));

                    //material.Extension = string.Empty;

                    //db.SaveChanges();

                    result.Success();
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

        public static RequestResult AddMaterialSpecification(List<MaterialSpecificationModel> materialSpecificationModels, List<string> selectedList, string refOrganizationId)
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
                        var materialType = temp[1];
                        var specUniqueID = temp[2];

                        if (!string.IsNullOrEmpty(specUniqueID))
                        {
                            if (!materialSpecificationModels.Any(x => x.MaterialSpecificationId == specUniqueID))
                            {
                                var spec = context.MSpecifications.First(x => x.MSpecificationId == new Guid(specUniqueID));

                                materialSpecificationModels.Add(new MaterialSpecificationModel()
                                {
                                    MaterialSpecificationId = spec.MSpecificationId.ToString(),
                                    Name = spec.Name,
                                    Seq = materialSpecificationModels.Count + 1,
                                    MaterialSpecificationOptionModels = context.MSOptions.Where(x => x.MSpecificationId == spec.MSpecificationId).Select(x => new MaterialSpecificationOptionModel
                                    {
                                        MaterialSpecificationOptionId = x.MSOptionId.ToString(),
                                        Name = x.Name,
                                        Seq = x.Seq,
                                        MaterialSpecificationId = x.MSpecificationId.ToString()
                                    }).OrderBy(x => x.Name).ToList()
                                });
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(materialType))
                            {
                                var specList = context.MSpecifications.Where(x => x.OrganizationId == new Guid(organizationId) && x.MaterialType == materialType).ToList();

                                foreach (var spec in specList)
                                {
                                    if (!materialSpecificationModels.Any(x => x.MaterialSpecificationId == spec.MSpecificationId.ToString()))
                                    {
                                        materialSpecificationModels.Add(new MaterialSpecificationModel()
                                        {
                                            MaterialSpecificationId = spec.MSpecificationId.ToString(),
                                            Name = spec.Name,
                                            Seq = materialSpecificationModels.Count + 1,
                                            MaterialSpecificationOptionModels = context.MSOptions.Where(x => x.MSpecificationId == spec.MSpecificationId).Select(x => new MaterialSpecificationOptionModel
                                            {
                                                MaterialSpecificationOptionId = x.MSpecificationId.ToString(),
                                                Name = x.Name,
                                                Seq = x.Seq,
                                                MaterialSpecificationId = x.MSpecificationId.ToString()
                                            }).OrderBy(x => x.Name).ToList()
                                        });
                                    }
                                }
                            }
                            else
                            {
                                var availableOrganizationIds = OrganizationDataAccessor.GetUpStreamOrganizationIds(new Guid(refOrganizationId), true);

                                var downStreamOrganizationIds = OrganizationDataAccessor.GetDownStreamOrganizationIds(new Guid(organizationId), true);

                                foreach (var downStreamOrganization in downStreamOrganizationIds)
                                {
                                    if (availableOrganizationIds.Any(x => x == downStreamOrganization))
                                    {
                                        var specList = context.MSpecifications.Where(x => x.OrganizationId == downStreamOrganization).ToList();

                                        foreach (var spec in specList)
                                        {
                                            if (!materialSpecificationModels.Any(x => x.MaterialSpecificationId == spec.MSpecificationId.ToString()))
                                            {
                                                materialSpecificationModels.Add(new MaterialSpecificationModel()
                                                {
                                                    MaterialSpecificationId = spec.MSpecificationId.ToString(),
                                                    Name = spec.Name,
                                                    Seq = materialSpecificationModels.Count + 1,
                                                    MaterialSpecificationOptionModels = context.MSOptions.Where(x => x.MSpecificationId == spec.MSpecificationId).Select(x => new MaterialSpecificationOptionModel
                                                    {
                                                        MaterialSpecificationOptionId = x.MSOptionId.ToString(),
                                                        Name = x.Name,
                                                        Seq = x.Seq,
                                                        MaterialSpecificationId = x.MSpecificationId.ToString()
                                                    }).OrderBy(x => x.Name).ToList()
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                materialSpecificationModels = materialSpecificationModels.OrderBy(x => x.Name).ToList();

                result.ReturnData(materialSpecificationModels);
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

                        var material = context.Materials.First(x => x.MaterialId == new Guid(editFormModel.MaterialId));

                        var exists =context.Materials.FirstOrDefault(x => x.MaterialId != material.MaterialId && x.OrganizationId == material.OrganizationId && x.MaterialType == editFormModel.FormInput.MaterialType && x.MId == editFormModel.FormInput.MId);

                        if (exists == null)
                        {
#if !DEBUG
                    using (TransactionScope trans = new TransactionScope())
                    {
#endif
                            #region Materials
                            material.MaterialType = editFormModel.FormInput.MaterialType;
                            material.MId = editFormModel.FormInput.MId;
                            material.Name = editFormModel.FormInput.MaterialName;
                            //material.Quantity = Model.FormInput.Quantity.HasValue ? Model.FormInput.Quantity.Value : 0;

                            context.SaveChanges();
                            #endregion

                            #region MaterialSpecificationOptionValue
                            #region Delete
                            context.MaterialSpecificationOptions.RemoveRange(context.MaterialSpecificationOptions.Where(x => x.MaterialId == new Guid(editFormModel.MaterialId)).ToList());

                            context.SaveChanges();
                            #endregion

                            #region Insert
                            context.MaterialSpecificationOptions.AddRange(editFormModel.MaterialSpecificationModels.Select(x => new CF.Models.Maintenance.MaterialSpecificationOption
                            {
                                MaterialId = new Guid(editFormModel.MaterialId),
                                MSpecificationId = new Guid(x.MaterialSpecificationId),
                                MSOptionId = new Guid(x.MaterialSpecificationOptionId),
                                Value = x.Value,
                                Seq = x.Seq
                            }).ToList());

                            context.SaveChanges();
                            #endregion
                            #endregion

                            requestResult.ReturnSuccessMessage(string.Format("{0} {1} {2}", Resources.Resource.Edit, Resources.Resource.Material, Resources.Resource.Success));
#if !DEBUG
                        trans.Complete();
                    }
#endif
                        }
                        else
                        {
                            requestResult.ReturnFailedMessage(string.Format("{0} {1}", Resources.Resource.MId, Resources.Resource.Exists));
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

        public static RequestResult SavePageState(List<MaterialSpecificationModel> materialSpecificationModels, List<string> pageStateList)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                int seq = 1;

                foreach (string pageState in pageStateList)
                {
                    string[] temp = pageState.Split(Define.Seperators, StringSplitOptions.None);

                    string materialSpecificationId = temp[0];
                    string materialSpecificationOptionId = temp[1];
                    string value = temp[2];

                    var specification = materialSpecificationModels.First(x => x.MaterialSpecificationId == materialSpecificationId);

                    specification.MaterialSpecificationOptionId = materialSpecificationOptionId;
                    specification.Value = value;
                    specification.Seq = seq;

                    seq++;
                }

                materialSpecificationModels = materialSpecificationModels.OrderBy(x => x.Seq).ToList();

                requestResult.ReturnData(materialSpecificationModels);
            }
            catch (Exception e)
            {
                var error = new Error(MethodBase.GetCurrentMethod(), e);

                Logger.Log(error);

                requestResult.ReturnError(error);
            }

            return requestResult;
        }

        public static RequestResult Create(CreateFormModel createFormModel)
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
                        var exists = context.Materials.FirstOrDefault(x => x.OrganizationId == new Guid(createFormModel.OrganizationId) && x.MaterialType == createFormModel.FormInput.MaterialType && x.MId == createFormModel.FormInput.MId);

                        if (exists == null)
                        {
                            Guid materialId = Guid.NewGuid();

                            context.Materials.Add(new CF.Models.Maintenance.Material()
                            {
                                MaterialId = materialId,
                                OrganizationId = new Guid(createFormModel.OrganizationId),
                                MaterialType = createFormModel.FormInput.MaterialType,
                                MId = createFormModel.FormInput.MId,
                                Name = createFormModel.FormInput.MaterialName,
                                Quantity = createFormModel.FormInput.Quantity ?? 0
                            });

                            context.MaterialSpecificationOptions.AddRange(createFormModel.MaterialSpecificationModels.Select(x => new CF.Models.Maintenance.MaterialSpecificationOption
                            {
                                MaterialId = materialId,
                                MSpecificationId = new Guid(x.MaterialSpecificationId),
                                MSOptionId = new Guid(x.MaterialSpecificationOptionId),
                                Value = x.Value,
                                Seq = x.Seq
                            }).ToList());

                            context.SaveChanges();

                            requestResult.ReturnData(materialId, string.Format("{0} {1} {2}", Resources.Resource.Create, Resources.Resource.Material, Resources.Resource.Success));
                        }
                        else
                        {
                            requestResult.ReturnFailedMessage(string.Format("{0} {1}", Resources.Resource.MId, Resources.Resource.Exists));
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
                using (CFContext context = new CFContext())
                {
                    var createFormModel = new CreateFormModel()
                    {
                        OrganizationId = organizationId,
                        ParentOrganizationFullName = OrganizationDataAccessor.GetOrganizationFullName(new Guid(organizationId)),
                        MaterialTypeSelectItems = new List<SelectListItem>()
                        {
                            Define.DefaultSelectListItem(Resources.Resource.Select),
                            new SelectListItem()
                            {
                                Text=Resources.Resource.Create+"...",
                                Value=Define.New
                            }
                        },
                        FormInput = new FormInput()
                        {
                            MaterialType = materialType
                        }
                    };

                    var upStreamOrganizationIds = OrganizationDataAccessor.GetUpStreamOrganizationIds(new Guid(organizationId), true);

                    createFormModel.MaterialTypeSelectItems.AddRange(context.Materials.Where(x => upStreamOrganizationIds.Contains(x.OrganizationId)).Select(x => x.MaterialType).Distinct().OrderBy(x => x).Select(x => new SelectListItem
                    {
                        Text = x,
                        Value = x
                    }).ToList());

                    if (!string.IsNullOrEmpty(materialType) && createFormModel.MaterialTypeSelectItems.Any(x => x.Value == materialType))
                    {
                        createFormModel.MaterialTypeSelectItems.First(x => x.Value == materialType).Selected = true;
                    }

                    requestResult.ReturnData(createFormModel);
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
                    var downStreamOrganizationIds = OrganizationDataAccessor.GetDownStreamOrganizationIds(new Guid(queryParameters.OrganizationId), true);

                    var query = context.Materials.Where(m => downStreamOrganizationIds.Contains(m.OrganizationId) && account.QueryableOrganizationIds.Contains(m.OrganizationId)).AsQueryable();
                    if (!string.IsNullOrEmpty(queryParameters.MaterialType))
                    {
                        query = query.Where(x => x.OrganizationId == new Guid(queryParameters.OrganizationId) && x.MaterialType == queryParameters.MaterialType);
                    }
                    if (!string.IsNullOrEmpty(queryParameters.Keyword))
                    {
                        query = query.Where(x => x.MId.Contains(queryParameters.Keyword) || x.Name.Contains(queryParameters.Keyword));
                    }

                    requestResult.ReturnData(new GridViewModel()
                    {
                        OrganizationId = queryParameters.OrganizationId,
                        OrganizationName = OrganizationDataAccessor.GetOrganizationName(new Guid(queryParameters.OrganizationId)),
                        FullOrganizationName = OrganizationDataAccessor.GetOrganizationFullName(new Guid(queryParameters.OrganizationId)),
                        Permission = account.OrganizationPermission(new Guid(queryParameters.OrganizationId)),
                        MaterialType = queryParameters.MaterialType,
                        Items = query.ToList().Select(x => new GridItem()
                        {
                            MaterialId = x.MaterialId.ToString(),
                            MId = x.MId,
                            Name = x.Name,
                            MaterialType = x.MaterialType,
                            Quantity = x.Quantity,
                            Permission = account.OrganizationPermission(x.OrganizationId),
                            OrganizationName = OrganizationDataAccessor.GetOrganizationName(x.OrganizationId),
                        }).OrderBy(x => x.OrganizationName).ThenBy(x => x.MaterialType).ThenBy(x => x.MId).ToList()
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

        public static RequestResult GetDetailViewModel(string materialId, Account account)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                using (CFContext context = new CFContext())
                {
                    var material = context.Materials.Include("MaterialSpecificationOptions").Where(m => m.MaterialId == new Guid(materialId)).FirstOrDefault();
                    var materialSpecificationIds = material.MaterialSpecificationOptions.Select(ms=>ms.MSpecificationId);

                    var detailViewModel = new DetailViewModel()
                    {
                        Permission = account.OrganizationPermission(material.OrganizationId),
                        MaterialId = material.MaterialId.ToString(),
                        OrganizationId = material.OrganizationId.ToString(),
                        ParentOrganizationName = OrganizationDataAccessor.GetOrganizationFullName(material.OrganizationId),
                        MaterialType = material.MaterialType,
                        MId = material.MId,
                        MaterialName = material.Name,
                        Quantity = material.Quantity,
                        MaterialSpecificationModels = context.MSpecifications.Where(ms => materialSpecificationIds.Contains(ms.MSpecificationId)).Select(ms => new MaterialSpecificationModel
                        {
                            MaterialSpecificationId = ms.MSpecificationId.ToString(),
                            Name = ms.Name,
                            MaterialSpecificationOptionId = context.MaterialSpecificationOptions.Where(x => x.MSpecificationId == ms.MSpecificationId).Select(x => x.MSOptionId).FirstOrDefault().ToString(),
                            Value = context.MaterialSpecificationOptions.Where(x => x.MSpecificationId == ms.MSpecificationId).Select(x => x.Value).FirstOrDefault().ToString(),
                            Seq = context.MaterialSpecificationOptions.Where(x => x.MSpecificationId == ms.MSpecificationId).Select(x => x.Seq).FirstOrDefault(),
                            MaterialSpecificationOptionModels = context.MSOptions.Where(x => x.MSpecificationId == ms.MSpecificationId).Select(mso => new MaterialSpecificationOptionModel
                            {
                                MaterialSpecificationOptionId = mso.MSOptionId.ToString(),
                                MaterialSpecificationId = mso.MSpecificationId.ToString(),
                                Name = mso.Name,
                                Seq = mso.Seq
                            }).OrderBy(mso => mso.Seq).ToList()
                        }).OrderBy(ms => ms.Seq).ToList()
                    };

                    //
                    requestResult.ReturnData(detailViewModel);
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

        public static RequestResult GetRootTreeItem(List<Organization> organizationList, Guid rootOrganizationId, Account account)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                var treeItemList = new List<TreeItem>();

                var attributes = new Dictionary<Define.EnumTreeAttribute, string>()
                {
                    { Define.EnumTreeAttribute.NodeType, string.Empty },
                    { Define.EnumTreeAttribute.ToolTip, string.Empty },
                    { Define.EnumTreeAttribute.OrganizationId, string.Empty },
                    { Define.EnumTreeAttribute.EquipmentType, string.Empty },
                    { Define.EnumTreeAttribute.MaterialId, string.Empty }
                };

                using (CFContext context=new CFContext())
                {
                    var organization = organizationList.First(x => x.OrganizationId == rootOrganizationId);

                    var treeItem = new TreeItem() { Title = organization.Name };

                    attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.Organization.ToString();
                    attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", organization.OId, organization.Name);
                    attributes[Define.EnumTreeAttribute.OrganizationId] = organization.OrganizationId.ToString();
                    attributes[Define.EnumTreeAttribute.EquipmentType] = string.Empty;
                    attributes[Define.EnumTreeAttribute.MaterialId] = string.Empty;

                    foreach (var attribute in attributes)
                    {
                        treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                    }

                    var haveDownStreamOrganization = organizationList.Any(x => x.ParentId == organization.OrganizationId && account.VisibleOrganizationIds.Contains(x.OrganizationId));

                    var haveMaterial = account.QueryableOrganizationIds.Contains(organization.OrganizationId) && context.Materials.Any(x => x.OrganizationId == organization.OrganizationId);

                    if (haveDownStreamOrganization || haveMaterial)
                    {
                        treeItem.State = "closed";
                    }

                    treeItemList.Add(treeItem);
                }

                requestResult.ReturnData(treeItemList);
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
