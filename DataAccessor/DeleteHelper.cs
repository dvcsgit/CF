using CF;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessor
{
    public class DeleteHelper
    {
        public static void DeleteOrganization(CFContext context, List<Guid> organizationIds)
        {
            foreach (var organizationId in organizationIds)
            {
                context.Organizations.Remove(context.Organizations.First(o => o.OrganizationId == organizationId));
                context.OrganizationManagers.RemoveRange(context.OrganizationManagers.Where(om => om.OrganizationId == organizationId));
                context.EditableOrganizations.RemoveRange(context.EditableOrganizations.Where(eo => eo.OrganizationId == organizationId));
                context.QueryableOrganizations.RemoveRange(context.QueryableOrganizations.Where(qo => qo.OrganizationId == organizationId));
            }

        }

        public static void DeleteMaterialSpecificaton(CFContext context, List<string> selectedList)
        {
            foreach(var materialSpecificationId in selectedList)
            {
                context.MSpecifications.Remove(context.MSpecifications.First(x => x.MSpecificationId == new Guid(materialSpecificationId)));
                //context.MaterialSpecificationOptionValues.RemoveRange(context.MaterialSpecificationOptionValues.Where(x => x.MaterialSpecificationId == new Guid(materialSpecificationId)).ToList());
            }
        }

        public static void DeleteMaterial(CFContext context, List<string> selectedList)
        {
            foreach(var materialId in selectedList)
            {
                context.Materials.Remove(context.Materials.First(x => x.MaterialId == new Guid(materialId)));
                context.MaterialSpecificationOptions.RemoveRange(context.MaterialSpecificationOptions.Where(x => x.MaterialId == new Guid(materialId)).ToList());
            }
        }

        public static void DeleteEquipmentSpecification(CFContext context, List<string> selectedList)
        {
            foreach(var equipmentSpecificationId in selectedList)
            {
                context.ESpecifications.Remove(context.ESpecifications.First(x => x.ESpecificationId == new Guid(equipmentSpecificationId)));

                context.EquipmentSpecificationOptions.RemoveRange(context.EquipmentSpecificationOptions.Where(x => x.ESpecificationId == new Guid(equipmentSpecificationId)).ToList());

                context.EquipmentSpecificationOptions.RemoveRange(context.EquipmentSpecificationOptions.Where(x => x.ESpecificationId == new Guid(equipmentSpecificationId)).ToList());
            }
        }

        public static void DeleteEquipment(CFContext context, List<string> selectedList)
        {
            foreach (var equipmentId in selectedList)
            {
                //Equipment
                context.Equipments.Remove(context.Equipments.First(x => x.EquipmentId == new Guid(equipmentId)));

                ////EquipmentMaterial
                //context.EquipmentMaterials.RemoveRange(context.EquipmentMaterials.Where(x => x.EquipmentId == new Guid(equipmentId)).ToList());

                ////EquipmentPart
                //context.EParts.RemoveRange(context.EParts.Where(x => x.EquipmentId == new Guid(equipmentId)).ToList());

                ////EquipmentSpecValue
                //context.EquipmentSpecificationOptions.RemoveRange(context.EquipmentSpecificationOptions.Where(x => x.EquipmentId == new Guid(equipmentId)).ToList());

                ////EquipmentCheckItem
                //context.EquipmentCheckItem.RemoveRange(context.EquipmentCheckItem.Where(x => x.EquipmentUniqueID == uniqueID).ToList());

                ////EquipmentStandard
                //context.EquipmentStandard.RemoveRange(context.EquipmentStandard.Where(x => x.EquipmentUniqueID == uniqueID).ToList());

                ////JobEquipment
                //context.JobEquipment.RemoveRange(context.JobEquipment.Where(x => x.EquipmentUniqueID == uniqueID).ToList());

                ////JobEquipmentCheckItem
                //context.JobEquipmentCheckItem.RemoveRange(context.JobEquipmentCheckItem.Where(x => x.EquipmentUniqueID == uniqueID).ToList());



                ////MJobEquipment
                //context.MJobEquipment.RemoveRange(context.MJobEquipment.Where(x => x.EquipmentUniqueID == uniqueID).ToList());

                ////MJobEquipmentStandard
                //context.MJobEquipmentStandard.RemoveRange(context.MJobEquipmentStandard.Where(x => x.EquipmentUniqueID == uniqueID).ToList());

                ////RouteEquipment
                //context.RouteEquipment.RemoveRange(context.RouteEquipment.Where(x => x.EquipmentUniqueID == uniqueID).ToList());

                ////RouteEquipmentCheckItem
                //context.RouteEquipmentCheckItem.RemoveRange(context.RouteEquipmentCheckItem.Where(x => x.EquipmentUniqueID == uniqueID).ToList());

                //Folder(context, context.Folder.Where(x => x.EquipmentUniqueID == uniqueID).Select(x => x.UniqueID).ToList());

                //File(context, context.File.Where(x => x.EquipmentUniqueID == uniqueID).Select(x => x.UniqueID).ToList());
            }
        }

        public static void DeleteSolutions(CFContext context, List<string> selectedList)
        {
            foreach(var solutionId in selectedList)
            {
                context.Solutions.Remove(context.Solutions.First(x => x.SolutionId == new Guid(solutionId)));
            }
        }

        public static void DeleteAbnormalReason(CFContext context, List<string> selectedList)
        {
            foreach(var abnormalReasonId in selectedList)
            {
                context.AbnormalReasons.Remove(context.AbnormalReasons.First(x => x.AbnormalReasonId == new Guid(abnormalReasonId)));
            }
        }

        public static void DeleteCheckItem(CFContext context, List<string> selectedList)
        {
            foreach(var checkItemId in selectedList)
            {
                context.CheckItems.Remove(context.CheckItems.First(x => x.CheckItemId == new Guid(checkItemId)));
            }
        }

        public static void DeleteCheckpoints(CFContext context, List<string> selectedList)
        {
            foreach(var checkpointId in selectedList)
            {
                context.Checkpoints.Remove(context.Checkpoints.First(x => x.CheckpointId == new Guid(checkpointId)));
            }
        }
    }
}
