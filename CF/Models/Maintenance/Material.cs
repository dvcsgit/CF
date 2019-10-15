using System;
using System.Collections.Generic;

namespace CF.Models.Maintenance
{
    public class Material
    {
        public Material()
        {
            //MaterialSpecifications = new HashSet<MaterialSpecification>();
            //Equipments = new HashSet<Equipment>();
            //EquipmentParts = new HashSet<EquipmentPart>();
            EquipmentPartMaterials = new HashSet<EPartMaterial>();
            MaterialSpecificationOptions = new HashSet<MaterialSpecificationOption>();
        }

        public Guid MaterialId { get; set; }
        public string MId { get; set; }
        public string Name { get; set; }
        public string MaterialType { get; set; }
        public int Quantity { get; set; }
        //public virtual ICollection<MaterialSpecification> MaterialSpecifications { get; set; }
        public virtual ICollection<MaterialSpecificationOption> MaterialSpecificationOptions { get; set; }

        public Guid OrganizationId { get; set; }
        public virtual Organization Organization { get; set; }

        //public ICollection<Equipment> Equipments { get; set; }
        public ICollection<EquipmentMaterial> EquipmentMaterials { get; set; }
        //public ICollection<EquipmentPart> EquipmentParts { get; set; }
        public ICollection<EPartMaterial> EquipmentPartMaterials { get; set; }
    }
}
