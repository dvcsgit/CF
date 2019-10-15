using System;
using System.Collections.Generic;

namespace CF.Models.Maintenance
{
    public class EPart
    {
        public EPart()
        {
            //Materials = new HashSet<Material>();
            EPartMaterials = new HashSet<EPartMaterial>();
            CheckItems = new HashSet<CheckItem>();
        }

        public Guid EPartId { get; set; }

        public Guid EquipmentId { get; set; }
        public virtual Equipment Equipment { get; set; }

        public string Name { get; set; }

        //public virtual ICollection<Material> Materials { get; set; }
        public virtual ICollection<EPartMaterial> EPartMaterials { get; set; }

        public virtual ICollection<CheckItem> CheckItems { get; set; }
    }
}
