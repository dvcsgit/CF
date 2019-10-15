using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CF;
using CF.Models.Maintenance;
using Models.Authentication;
using Models.Maintenance.Equipment;
using Models.Shared;
using Utility;
using Utility.Models;

namespace DataAccessor.Maintenance
{
    public class EquipmentDataAccessor
    {
        public static RequestResult GetTreeItems(List<Organization> organizationList, Guid organizationId, Account account)
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
                    { Define.EnumTreeAttribute.EquipmentId, string.Empty }
                };

                using (CFContext context=new CFContext())
                {
                    if (account.QueryableOrganizationIds.Contains(organizationId))
                    {
                        var equipmentList = context.Equipments.Where(x => x.AffiliationOrganizationId == organizationId).OrderBy(x => x.EId).ToList();

                        foreach (var equipment in equipmentList)
                        {
                            var treeItem = new TreeItem() { Title = equipment.Name };

                            attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.Equipment.ToString();
                            attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", equipment.EId, equipment.Name);
                            attributes[Define.EnumTreeAttribute.OrganizationId] = organizationId.ToString();
                            attributes[Define.EnumTreeAttribute.EquipmentId] = equipment.EquipmentId.ToString();

                            foreach (var attribute in attributes)
                            {
                                treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                            }

                            treeItemList.Add(treeItem);
                        }
                    }

                    var organizations = organizationList.Where(x => x.ParentId == organizationId && account.VisibleOrganizationIds.Contains(x.OrganizationId)).OrderBy(x => x.OId).ToList();

                    foreach (var organization in organizations)
                    {
                        var treeItem = new TreeItem() { Title = organization.Name };

                        attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.Organization.ToString();
                        attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", organization.OId, organization.Name);
                        attributes[Define.EnumTreeAttribute.OrganizationId] = organization.OrganizationId.ToString();
                        attributes[Define.EnumTreeAttribute.EquipmentId] = string.Empty;

                        foreach (var attribute in attributes)
                        {
                            treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                        }

                        var haveDownStreamOrganization = organizationList.Any(x => x.ParentId == organization.OrganizationId && account.VisibleOrganizationIds.Contains(x.OrganizationId));

                        var haveEquipment = account.QueryableOrganizationIds.Contains(organization.OrganizationId) && context.Equipments.Any(x => x.AffiliationOrganizationId == organization.OrganizationId);

                        if (haveDownStreamOrganization || haveEquipment)
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

        public static RequestResult Create(CreateFormModel createFormModel)
        {
            RequestResult result = new RequestResult();

            try
            {
                using (CFContext context=new CFContext())
                {
                    var exists = context.Equipments.FirstOrDefault(x => x.AffiliationOrganizationId == new Guid(createFormModel.OrganizationId) && x.EId == createFormModel.FormInput.EId);

                    if (exists == null)
                    {
                        context.Equipments.Add(new Equipment()
                        {
                            EquipmentId = new Guid(createFormModel.EquipmentId),                            
                            EId = createFormModel.FormInput.EId,
                            Name = createFormModel.FormInput.Name,
                            IsFeelItemDefaultNormal = false,
                            EquipmentSpecificationOptions=createFormModel.ESpecificationModels.Select(s=>new EquipmentSpecificationOption()
                            {
                                EquipmentId=new Guid(createFormModel.EquipmentId),
                                ESpecificationId=new Guid(s.ESpecificationId),
                                ESOptionId=new Guid(s.ESOptionId)
                            }).ToList(),
                            EquipmentMaterials=createFormModel.MaterialModels.Select(m=>new EquipmentMaterial()
                            {
                                EquipmentId=new Guid(createFormModel.EquipmentId),
                                MaterialId=new Guid(m.MaterialId),
                                Quantity=m.Quantity
                            }).ToList(),
                            AffiliationOrganizationId = new Guid(createFormModel.OrganizationId),
                            MaintenanceOrganizationId = new Guid(createFormModel.FormInput.MaintenanceOrganizationId),
                            EParts=createFormModel.EPartModels.Select(x=>new EPart()
                            {
                                EPartId=new Guid(x.EPartId),
                                Name=x.Name,
                                EPartMaterials=x.MaterialModels.Select(m=>new EPartMaterial()
                                {
                                    EPartId=new Guid(x.EPartId),
                                    MaterialId=new Guid(m.MaterialId),
                                    Quantity=m.Quantity
                                }).ToList()
                            }).ToList(),
                            LastModifyTime = DateTime.Now
                        });                                                

                        context.SaveChanges();

                        result.ReturnSuccessMessage(string.Format("{0} {1} {2}", Resources.Resource.Create, Resources.Resource.Equipment, Resources.Resource.Success));
                    }
                    else
                    {
                        result.ReturnFailedMessage(string.Format("{0} {1}", Resources.Resource.EId, Resources.Resource.Exists));
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

        public static RequestResult GetCopyFormModel(Guid equipmentId)
        {
            RequestResult result = new RequestResult();

            try
            {
                using (CFContext context=new CFContext())
                {
                    var equipment = context.Equipments.First(x => x.EquipmentId == equipmentId);

                    var organization = OrganizationDataAccessor.GetOrganization(equipment.AffiliationOrganizationId);

                    var maintenanceOrganization = OrganizationDataAccessor.GetOrganization(equipment.MaintenanceOrganizationId);

                    result.ReturnData(new CreateFormModel()
                    {
                        EquipmentId = Guid.NewGuid().ToString(),
                        AncestorOrganizationId = organization.AncestorOrganizationId.ToString(),
                        OrganizationId = equipment.AffiliationOrganizationId.ToString(),
                        ParentOrganizationFullName = organization.FullName,
                        MaintenanceOrganizationId = maintenanceOrganization != null ? maintenanceOrganization.OId : string.Empty,
                        MaintenanceOrganizationName = maintenanceOrganization != null ? maintenanceOrganization.Name : string.Empty,
                        ESpecificationModels = (from x in context.EquipmentSpecificationOptions
                                    join s in context.ESpecifications
                                    on x.ESpecificationId equals s.ESpecificationId
                                    where x.EquipmentId == equipment.EquipmentId
                                    select new ESpecificationModel
                                    {
                                        ESpecificationId = s.ESpecificationId.ToString(),
                                        Name = s.Name,
                                        ESOptionId = x.ESOptionId.ToString(),
                                        Value = x.Value,
                                        Seq = x.Seq,
                                        ESOptionModels = context.ESOptions.Where(o => o.ESpecificationId == s.ESpecificationId).Select(o => new Models.Maintenance.Equipment.ESOptionModel
                                        {
                                            ESOptionId = o.ESOptionId.ToString(),
                                            Name = o.Name,
                                            ESpecificationId = o.ESpecificationId.ToString(),
                                            Seq = o.Seq
                                        }).OrderBy(o => o.Seq).ToList()
                                    }).OrderBy(x => x.Seq).ToList(),
                        MaterialModels = (from x in context.EquipmentMaterials
                                        join m in context.Materials
                                        on x.MaterialId equals m.MaterialId
                                        where x.EquipmentId == equipment.EquipmentId
                                        select new MaterialModel
                                        {
                                            MaterialId = m.MaterialId.ToString(),
                                            MId = m.MId,
                                            Name = m.Name,
                                            Quantity = x.Quantity
                                        }).OrderBy(x => x.MId).ToList(),
                        EPartModels = (from p in context.EParts
                                    where p.EquipmentId == equipment.EquipmentId
                                    select new EPartModel
                                    {
                                        EPartId = Guid.NewGuid().ToString(),
                                        Name = p.Name,
                                        MaterialModels = (from x in context.EPartMaterials
                                                        join m in context.Materials
                                                        on x.MaterialId equals m.MaterialId
                                                        where x.EPartId == p.EPartId
                                                        select new MaterialModel
                                                        {
                                                            MaterialId = m.MaterialId.ToString(),
                                                            MId = m.MId,
                                                            Name = m.Name,
                                                            Quantity = x.Quantity
                                                        }).OrderBy(x => x.MId).ToList()
                                    }).OrderBy(x => x.Name).ToList(),
                        FormInput = new FormInput()
                        {
                            Name = equipment.Name,
                            MaintenanceOrganizationId = equipment.MaintenanceOrganizationId.ToString()
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

        public static RequestResult GetEditFormModel(string equipmentId)
        {
            RequestResult result = new RequestResult();

            try
            {
                using (CFContext context=new CFContext())
                {
                    var equipment = context.Equipments.First(x => x.EquipmentId == new Guid(equipmentId));

                    var organization = OrganizationDataAccessor.GetOrganization(equipment.AffiliationOrganizationId);

                    var maintenanceOrganization = OrganizationDataAccessor.GetOrganization(equipment.MaintenanceOrganizationId);

                    result.ReturnData(new EditFormModel()
                    {
                        EquipmentId = equipment.EquipmentId.ToString(),
                        AncestorOrganizationId = organization.AncestorOrganizationId.ToString(),
                        OrganizationId = equipment.AffiliationOrganizationId.ToString(),
                        ParentOrganizationFullName = organization.FullName,
                        MaintenanceOId = maintenanceOrganization != null ? maintenanceOrganization.OId : string.Empty,
                        MaintenanceOrganizationName = maintenanceOrganization != null ? maintenanceOrganization.Name : string.Empty,
                        ESpecificationModels = (from x in context.EquipmentSpecificationOptions
                                    join s in context.ESpecifications
                                    on x.ESpecificationId equals s.ESpecificationId
                                    where x.EquipmentId == equipment.EquipmentId
                                    select new ESpecificationModel
                                    {
                                        ESpecificationId = s.ESpecificationId.ToString(),
                                        Name = s.Name,
                                        ESOptionId = x.ESOptionId.ToString(),
                                        Value = x.Value,
                                        Seq = x.Seq,
                                        ESOptionModels = context.ESOptions.Where(o => o.ESpecificationId == s.ESpecificationId).Select(o => new Models.Maintenance.Equipment.ESOptionModel
                                        {
                                            ESOptionId = o.ESOptionId.ToString(),
                                            Name = o.Name,
                                            ESpecificationId = o.ESpecificationId.ToString(),
                                            Seq = o.Seq
                                        }).OrderBy(o => o.Seq).ToList()
                                    }).OrderBy(x => x.Seq).ToList(),
                        MaterialModels = (from x in context.EquipmentMaterials
                                        join m in context.Materials
                                        on x.MaterialId equals m.MaterialId
                                        where x.EquipmentId == equipment.EquipmentId
                                        select new MaterialModel
                                        {
                                            MaterialId = m.MaterialId.ToString(),
                                            MId = m.MId,
                                            Name = m.Name,
                                            Quantity = x.Quantity
                                        }).OrderBy(x => x.MId).ToList(),
                        EPartModels = (from p in context.EParts
                                    where p.EquipmentId == equipment.EquipmentId
                                    select new EPartModel
                                    {
                                        EPartId = p.EPartId.ToString(),
                                        Name = p.Name,
                                        MaterialModels = (from x in context.EquipmentMaterials
                                                        join m in context.Materials
                                                        on x.MaterialId equals m.MaterialId
                                                        where x.EquipmentId == equipment.EquipmentId
                                                        select new MaterialModel
                                                        {
                                                            MaterialId = m.MaterialId.ToString(),
                                                            MId = m.MId,
                                                            Name = m.Name,
                                                            Quantity = x.Quantity
                                                        }).OrderBy(x => x.MId).ToList()
                                    }).OrderBy(x => x.Name).ToList(),
                        FormInput = new FormInput()
                        {
                            EId = equipment.EId,
                            Name = equipment.Name,
                            MaintenanceOrganizationId = equipment.MaintenanceOrganizationId.ToString()
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

        public static RequestResult Delete(List<string> selectedList)
        {
            RequestResult result = new RequestResult();

            try
            {
                using (CFContext context=new CFContext())
                {
                    DeleteHelper.DeleteEquipment(context, selectedList);

                    context.SaveChanges();
                }

                result.ReturnSuccessMessage(string.Format("{0} {1} {2}", Resources.Resource.Delete, Resources.Resource.Equipment, Resources.Resource.Success));
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
                using (CFContext context=new CFContext())
                {
                    var equipment = context.Equipments.First(x => x.EquipmentId == new Guid(editFormModel.EquipmentId));

                    var exists = context.Equipments.FirstOrDefault(x => x.AffiliationOrganizationId == equipment.AffiliationOrganizationId && x.EquipmentId != equipment.EquipmentId && x.EId == editFormModel.FormInput.EId);

                    if (exists == null)
                    {
#if !DEBUG
                    using (TransactionScope trans = new TransactionScope())
                    {
#endif
                        #region Equipment
                        equipment.EId = editFormModel.FormInput.EId;
                        equipment.Name = editFormModel.FormInput.Name;
                        equipment.MaintenanceOrganizationId = new Guid(editFormModel.FormInput.MaintenanceOrganizationId);
                        equipment.LastModifyTime = DateTime.Now;

                        context.SaveChanges();
                        #endregion

                        #region EquipmentSpecificationOptions
                        #region Delete
                        context.EquipmentSpecificationOptions.RemoveRange(context.EquipmentSpecificationOptions.Where(x => x.EquipmentId == equipment.EquipmentId).ToList());

                        context.SaveChanges();
                        #endregion

                        #region Insert
                        context.EquipmentSpecificationOptions.AddRange(editFormModel.ESpecificationModels.Select(x => new EquipmentSpecificationOption
                        {
                            EquipmentId = equipment.EquipmentId,
                            ESpecificationId = new Guid(x.ESpecificationId),
                            ESOptionId= new Guid(x.ESOptionId),
                            Value = x.Value,
                            Seq = x.Seq
                        }).ToList());

                        context.SaveChanges();
                        #endregion
                        #endregion

                        #region EquipmentPart, EquipmentMaterial
                        #region Delete
                        context.EParts.RemoveRange(context.EParts.Where(x => x.EquipmentId == equipment.EquipmentId).ToList());
                        context.EquipmentMaterials.RemoveRange(context.EquipmentMaterials.Where(x => x.EquipmentId == equipment.EquipmentId).ToList());

                        context.SaveChanges();
                        #endregion

                        #region Insert
                        context.EquipmentMaterials.AddRange(editFormModel.MaterialModels.Select(x => new EquipmentMaterial
                        {
                            EquipmentId = equipment.EquipmentId,                            
                            MaterialId = new Guid(x.MaterialId),
                            Quantity = x.Quantity
                        }).ToList());

                        foreach (var part in editFormModel.EPartModels)
                        {
                            context.EParts.Add(new EPart()
                            {
                                EPartId = new Guid(part.EPartId),
                                EquipmentId = equipment.EquipmentId,
                                Name = part.Name
                            });

                            context.EPartMaterials.AddRange(part.MaterialModels.Select(x => new EPartMaterial
                            {
                                EPartId = new Guid(part.EPartId),                               
                                MaterialId = new Guid(x.MaterialId),
                                Quantity = x.Quantity
                            }).ToList());
                        }

                        context.SaveChanges();
                        #endregion
                        #endregion

                        //#region EquipmentCheckItem, EquipmentStandard
                        //var partList = editFormModel.EPartModels.Select(x => x.EPartId).ToList();

                        //context.EquipmentCheckItem.RemoveRange(context.EquipmentCheckItem.Where(x => x.EquipmentUniqueID == equipment.EquipmentId && x.PartUniqueID != "*" && !partList.Contains(x.PartUniqueID)).ToList());
                        //context.EquipmentStandard.RemoveRange(context.EquipmentStandard.Where(x => x.EquipmentUniqueID == equipment.EquipmentId && x.PartUniqueID != "*" && !partList.Contains(x.PartUniqueID)).ToList());

                        //context.SaveChanges();
                        //#endregion
#if !DEBUG
                        trans.Complete();
                    }
#endif
                        result.ReturnSuccessMessage(string.Format("{0} {1} {2}", Resources.Resource.Edit, Resources.Resource.Equipment, Resources.Resource.Success));
                    }
                    else
                    {
                        result.ReturnFailedMessage(string.Format("{0} {1}", Resources.Resource.EId, Resources.Resource.Exists));
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

        public static RequestResult SavePageState(List<ESpecificationModel> eSpecificationModels, List<string> specPageStateList)
        {
            RequestResult result = new RequestResult();

            try
            {
                int seq = 1;

                foreach (string pageState in specPageStateList)
                {
                    string[] temp = pageState.Split(Define.Seperators, StringSplitOptions.None);

                    string specUniqueID = temp[0];
                    string optionUniqueID = temp[1];
                    string value = temp[2];

                    var spec = eSpecificationModels.First(x => x.ESpecificationId == specUniqueID);

                    spec.ESOptionId = optionUniqueID;
                    spec.Value = value;
                    spec.Seq = seq;

                    seq++;
                }

                eSpecificationModels = eSpecificationModels.OrderBy(x => x.Seq).ToList();

                result.ReturnData(eSpecificationModels);
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return result;
        }

        public static RequestResult SavePageState(List<MaterialModel> materialModels, List<string> pageStateList)
        {
            RequestResult result = new RequestResult();

            try
            {
                foreach (string pageState in pageStateList)
                {
                    string[] temp = pageState.Split(Define.Seperators, StringSplitOptions.None);

                    string materialId = temp[0];
                    string qty = temp[1];

                    var material = materialModels.First(x => x.MaterialId == materialId);

                    if (!string.IsNullOrEmpty(qty))
                    {
                        int q;

                        if (int.TryParse(qty, out q))
                        {
                            material.Quantity = q;
                        }
                        else
                        {
                            material.Quantity = 0;
                        }
                    }
                    else
                    {
                        material.Quantity = 0;
                    }
                }

                result.ReturnData(materialModels);
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return result;
        }

        public static RequestResult SavePageState(List<EPartModel> ePartModels, List<string> pageStateList)
        {
            RequestResult result = new RequestResult();

            try
            {
                foreach (string pageState in pageStateList)
                {
                    string[] temp = pageState.Split(Define.Seperators, StringSplitOptions.None);

                    string ePartId = temp[0];
                    string materialId = temp[1];
                    string qty = temp[2];

                    var material = ePartModels.First(x => x.EPartId == ePartId).MaterialModels.First(x => x.MaterialId == materialId);

                    if (!string.IsNullOrEmpty(qty))
                    {
                        int q;

                        if (int.TryParse(qty, out q))
                        {
                            material.Quantity = q;
                        }
                        else
                        {
                            material.Quantity = 0;
                        }
                    }
                    else
                    {
                        material.Quantity = 0;
                    }
                }

                result.ReturnData(ePartModels);
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return result;
        }

        public static RequestResult GetDetailViewModel(string equipmentId, Account account)
        {
            RequestResult result = new RequestResult();

            try
            {
                using (CFContext context=new CFContext())
                {
                    var equipment = context.Equipments.Include("EquipmentMaterials").First(x => x.EquipmentId == new Guid(equipmentId));
                    var equipmentParts = equipment.EParts.ToList();

                    result.ReturnData(new DetailViewModel()
                    {
                        EquipmentId = equipment.EquipmentId.ToString(),
                        Permission = account.OrganizationPermission(equipment.AffiliationOrganizationId),
                        OrganizationId = equipment.AffiliationOrganizationId.ToString(),
                        ParentOrganizationFullName = OrganizationDataAccessor.GetOrganizationFullName(equipment.AffiliationOrganizationId),
                        EId = equipment.EId,
                        Name = equipment.Name,
                        MaintenanceOrganizationFullName = OrganizationDataAccessor.GetOrganizationFullName(equipment.MaintenanceOrganizationId),
                        ESpecificationModels = (from x in context.EquipmentSpecificationOptions
                                    join s in context.ESpecifications
                                    on x.ESpecificationId equals s.ESpecificationId
                                    where x.EquipmentId == equipment.EquipmentId
                                    select new ESpecificationModel
                                    {
                                        ESpecificationId = s.ESpecificationId.ToString(),
                                        Name = s.Name,
                                        ESOptionId = x.ESOptionId.ToString(),
                                        Value = x.Value,
                                        Seq = x.Seq,
                                        ESOptionModels = context.ESOptions.Where(o => o.ESpecificationId == s.ESpecificationId).Select(o => new Models.Maintenance.Equipment.ESOptionModel
                                        {
                                            Seq = o.Seq,
                                            ESpecificationId = o.ESpecificationId.ToString(),
                                            Name = o.Name, 
                                            ESOptionId = o.ESOptionId.ToString()
                                        }).OrderBy(o => o.Seq).ToList()
                                    }).OrderBy(x => x.Seq).ToList(),
                        MaterialModels = (from x in equipment.EquipmentMaterials
                                        join m in context.Materials
                                        on x.MaterialId equals m.MaterialId
                                        //where x.EquipmentId == equipment.UniqueID && x.PartUniqueID == "*"
                                        select new MaterialModel
                                        {
                                            MaterialId = m.MaterialId.ToString(),
                                            MId = m.MId,
                                            Name = m.Name,
                                            Quantity = x.Quantity
                                        }).OrderBy(x => x.MId).ToList(),
                        EPartModels = (from p in context.EParts
                                    where p.EquipmentId == equipment.EquipmentId
                                    select new EPartModel
                                    {
                                        EPartId = p.EPartId.ToString(),
                                        Name = p.Name,
                                        MaterialModels = (from epm in p.EPartMaterials
                                                                       join m in context.Materials
                                                                       on epm.MaterialId equals m.MaterialId
                                                                       select new MaterialModel
                                                                       {
                                                                           MaterialId=m.MaterialId.ToString(),
                                                                           MId=m.MId,
                                                                           Name=m.Name,
                                                                           Quantity=epm.Quantity
                                                                       }).OrderBy(x=>x.MId).ToList()                                                                                                             
                                    }).OrderBy(x => x.Name).ToList()
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

                    var query = context.Equipments.Where(x => downStreamOrganizationIds.Contains(x.AffiliationOrganizationId) && account.QueryableOrganizationIds.Contains(x.AffiliationOrganizationId)).AsQueryable();

                    if (!string.IsNullOrEmpty(queryParameters.Keyword))
                    {
                        query = query.Where(x => x.EId.Contains(queryParameters.Keyword) || x.Name.Contains(queryParameters.Keyword));
                    }

                    var organization = OrganizationDataAccessor.GetOrganization(new Guid(queryParameters.OrganizationId));

                    var model = new GridViewModel()
                    {
                        OrganizationId = queryParameters.OrganizationId,
                        Permission = account.OrganizationPermission(new Guid(queryParameters.OrganizationId)),
                        OrganizationName = organization.Name,
                        FullOrganizationName = organization.FullName,
                        Items = query.ToList().Select(x => new GridItem()
                        {
                            EquipmentId = x.EquipmentId.ToString(),
                            Permission = account.OrganizationPermission(x.AffiliationOrganizationId),
                            OrganizationName = OrganizationDataAccessor.GetOrganizationName(x.AffiliationOrganizationId),
                            MaintenanceOrganization = OrganizationDataAccessor.GetOrganizationName(x.MaintenanceOrganizationId),
                            EId = x.EId,
                            Name = x.Name
                        }).OrderBy(x => x.OrganizationName).ThenBy(x => x.EId).ToList()
                    };

                    var upStreamList = OrganizationDataAccessor.GetUpStreamOrganizationIds(new Guid(queryParameters.OrganizationId), false);
                    var downStreamList = OrganizationDataAccessor.GetDownStreamOrganizationIds(new Guid(queryParameters.OrganizationId), false);

                    foreach (var upStream in upStreamList)
                    {
                        if (account.EditableOrganizationIds.Any(x => x == upStream))
                        {
                            model.MoveToTargets.Add(new MoveToTarget()
                            {
                                Id = upStream,
                                Name = OrganizationDataAccessor.GetOrganizationFullName(upStream),
                                Direction = Define.EnumMoveDirection.Up
                            });
                        }
                    }

                    foreach (var downStream in downStreamList)
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

        public static RequestResult GetRootTreeItems(List<Organization> organizationList, Guid rootOrganizationId, Account account)
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
                    { Define.EnumTreeAttribute.EquipmentId, string.Empty }
                };

                using (CFContext context=new CFContext())
                {
                    var organization = organizationList.First(x => x.OrganizationId == rootOrganizationId);

                    var treeItem = new TreeItem() { Title = organization.Name };

                    attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.Organization.ToString();
                    attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", organization.OId, organization.Name);
                    attributes[Define.EnumTreeAttribute.OrganizationId] = organization.OrganizationId.ToString();
                    attributes[Define.EnumTreeAttribute.EquipmentId] = string.Empty;

                    foreach (var attribute in attributes)
                    {
                        treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                    }

                    var haveDownStreamOrganization = organizationList.Any(x => x.ParentId == organization.OrganizationId && account.VisibleOrganizationIds.Contains(x.OrganizationId));

                    var haveEquipment = account.QueryableOrganizationIds.Contains(organization.OrganizationId) && context.Equipments.Any(x => x.AffiliationOrganizationId == organization.OrganizationId);

                    if (haveDownStreamOrganization || haveEquipment)
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

        public static RequestResult AddMaterial(List<MaterialModel> materialModels, List<string> selectedList, string refOrganizationId)
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
                        var materialId = temp[2];

                        if (!string.IsNullOrEmpty(materialId))
                        {
                            if (!materialModels.Any(x => x.MaterialId == materialId))
                            {
                                var material = context.Materials.First(x => x.MaterialId == new Guid(materialId));

                                materialModels.Add(new MaterialModel()
                                {
                                    MaterialId = material.MaterialId.ToString(),
                                    MId = material.MId,
                                    Name = material.Name,
                                    Quantity = 0
                                });
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(materialType))
                            {
                                var materialList = context.Materials.Where(x => x.OrganizationId == new Guid(organizationId) && x.MaterialType == materialType).ToList();

                                foreach (var material in materialList)
                                {
                                    if (!materialModels.Any(x => x.MaterialId == material.MaterialId.ToString()))
                                    {
                                        materialModels.Add(new MaterialModel()
                                        {
                                            MaterialId = material.MaterialId.ToString(),
                                            MId = material.MId,
                                            Name = material.Name,
                                            Quantity = 0
                                        });
                                    }
                                }
                            }
                            else
                            {
                                var availableOrganizationList = OrganizationDataAccessor.GetUpStreamOrganizationIds(new Guid(refOrganizationId), true);

                                var organizationList = OrganizationDataAccessor.GetDownStreamOrganizationIds(new Guid(organizationId), true);

                                foreach (var organization in organizationList)
                                {
                                    if (availableOrganizationList.Any(x => x == organization))
                                    {
                                        var materialList = context.Materials.Where(x => x.OrganizationId == organization).ToList();

                                        foreach (var material in materialList)
                                        {
                                            if (!materialModels.Any(x => x.MaterialId == material.MaterialId.ToString()))
                                            {
                                                materialModels.Add(new MaterialModel()
                                                {
                                                    MaterialId = material.MaterialId.ToString(),
                                                    MId = material.MId,
                                                    Name = material.Name,
                                                    Quantity = 0
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                result.ReturnData(materialModels.OrderBy(x => x.MId).ToList());
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return result;
        }

        public static RequestResult AddSpecification(List<ESpecificationModel> eSpecificationModels, List<string> selectedList, string refOrganizationId)
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
                        var equipmentType = temp[1];
                        var eSpecificationId = temp[2];

                        if (!string.IsNullOrEmpty(eSpecificationId))
                        {
                            if (!eSpecificationModels.Any(x => x.ESpecificationId == eSpecificationId))
                            {
                                var spec = context.ESpecifications.First(x => x.ESpecificationId == new Guid(eSpecificationId));

                                eSpecificationModels.Add(new ESpecificationModel()
                                {
                                    ESpecificationId = spec.ESpecificationId.ToString(),
                                    Name = spec.Name,
                                    Seq = eSpecificationModels.Count + 1,
                                    ESOptionModels = context.ESOptions.Where(o => o.ESpecificationId == spec.ESpecificationId).Select(o => new Models.Maintenance.Equipment.ESOptionModel
                                    {
                                        ESOptionId = o.ESOptionId.ToString(),
                                        Name = o.Name,
                                        ESpecificationId = o.ESpecificationId.ToString(),
                                        Seq = o.Seq
                                    }).OrderBy(o => o.Seq).ToList()
                                });
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(equipmentType))
                            {
                                var specList = context.ESpecifications.Where(x => x.OrganizationId == new Guid(organizationId) && x.EquipmentType == equipmentType).ToList();

                                foreach (var spec in specList)
                                {
                                    if (!eSpecificationModels.Any(x => x.ESpecificationId == spec.ESpecificationId.ToString()))
                                    {
                                        eSpecificationModels.Add(new ESpecificationModel()
                                        {
                                            ESpecificationId = spec.ESpecificationId.ToString(),
                                            Name = spec.Name,
                                            Seq = eSpecificationModels.Count + 1,
                                            ESOptionModels = context.ESOptions.Where(o => o.ESpecificationId == spec.ESpecificationId).Select(o => new Models.Maintenance.Equipment.ESOptionModel
                                            {
                                                ESOptionId = o.ESOptionId.ToString(),
                                                Name = o.Name,
                                                ESpecificationId = o.ESpecificationId.ToString(),
                                                Seq = o.Seq
                                            }).OrderBy(o => o.Seq).ToList()
                                        });
                                    }
                                }
                            }
                            else
                            {
                                var availableOrganizationList = OrganizationDataAccessor.GetUpStreamOrganizationIds(new Guid(refOrganizationId), true);

                                var downStreamOrganizationList = OrganizationDataAccessor.GetDownStreamOrganizationIds(new Guid(organizationId), true);

                                foreach (var organization in downStreamOrganizationList)
                                {
                                    if (availableOrganizationList.Any(x => x == organization))
                                    {
                                        var specList = context.ESpecifications.Where(x => x.OrganizationId == organization).ToList();

                                        foreach (var spec in specList)
                                        {
                                            if (!eSpecificationModels.Any(x => x.ESpecificationId == spec.ESpecificationId.ToString()))
                                            {
                                                eSpecificationModels.Add(new ESpecificationModel()
                                                {
                                                    ESpecificationId = spec.ESpecificationId.ToString(),
                                                    Name = spec.Name,
                                                    Seq = eSpecificationModels.Count + 1,
                                                    ESOptionModels = context.ESOptions.Where(o => o.ESpecificationId == spec.ESpecificationId).Select(o => new Models.Maintenance.Equipment.ESOptionModel
                                                    {
                                                        ESOptionId = o.ESOptionId.ToString(),
                                                        Name = o.Name,
                                                        ESpecificationId = o.ESpecificationId.ToString(),
                                                        Seq = o.Seq
                                                    }).OrderBy(o => o.Seq).ToList()
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                eSpecificationModels = eSpecificationModels.OrderBy(x => x.Seq).ToList();

                result.ReturnData(eSpecificationModels);
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
