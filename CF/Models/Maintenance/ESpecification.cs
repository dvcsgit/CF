using System;
using System.Collections.Generic;

namespace CF.Models.Maintenance
{
    public class ESpecification
    {
        public ESpecification()
        {
            //Equipments = new HashSet<Equipment>();
            ESOptions = new HashSet<ESOption>();
            EquipmentSpecificationOptions = new HashSet<EquipmentSpecificationOption>();
        }

        public Guid ESpecificationId { get; set; }
        public string EquipmentType { get; set; }
        public string Name { get; set; }
        public ICollection<ESOption> ESOptions { get; set; }

        public Guid OrganizationId { get; set; }
        public virtual Organization Organization { get; set; }

        //public virtual ICollection<Equipment> Equipments { get; set; }
        public virtual ICollection<EquipmentSpecificationOption> EquipmentSpecificationOptions { get; set; }
    }
}
