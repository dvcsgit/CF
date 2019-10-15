using System;

namespace CF.Models.Maintenance
{
    public class EquipmentMaterial
    {
        public Guid EquipmentId { get; set; }
        public virtual Equipment Equipment { get; set; }

        public Guid MaterialId { get; set; }
        public virtual Material Material { get; set; }

        public int Quantity { get; set; }
    }
}
