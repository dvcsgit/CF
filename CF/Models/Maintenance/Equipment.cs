using System;
using System.Collections.Generic;

namespace CF.Models.Maintenance
{
    public class Equipment
    {
        public Equipment()
        {
            //Materials = new HashSet<Material>();
            //EquipmentSpecifications = new HashSet<EquipmentSpecification>();
            EquipmentMaterials = new HashSet<EquipmentMaterial>();
            EParts = new HashSet<EPart>();
            EquipmentSpecificationOptions = new HashSet<EquipmentSpecificationOption>();
            //Checkpoints = new HashSet<Checkpoint>();
            LineCheckpointEquipmentCheckItems = new HashSet<LineCheckpointEquipmentCheckItem>();
            CheckItems = new HashSet<CheckItem>();
        }

        public Guid EquipmentId { get; set; }       
        public string EId { get; set; }
        public string Name { get; set; }
        public bool IsFeelItemDefaultNormal { get; set; }

        //public virtual ICollection<Material> Materials { get; set; }
        public ICollection<EquipmentMaterial> EquipmentMaterials { get; set; }

        public Guid AffiliationOrganizationId { get; set; }
        public virtual Organization AffiliationOrganization { get; set; }

        public Guid MaintenanceOrganizationId { get; set; }
        public virtual Organization MaintenanceOrganization { get; set; }
        
        public DateTime LastModifyTime { get; set; }

        //public ICollection<EquipmentSpecification> EquipmentSpecifications { get; set; }

        public virtual ICollection<EquipmentSpecificationOption> EquipmentSpecificationOptions { get; set; }

        public virtual ICollection<EPart> EParts { get; set; }

        //public virtual ICollection<Checkpoint> Checkpoints { get; set; }

        public virtual ICollection<CheckItem> CheckItems { get; set; }

        public virtual ICollection<LineCheckpointEquipmentCheckItem> LineCheckpointEquipmentCheckItems { get; set; }
    }
}
