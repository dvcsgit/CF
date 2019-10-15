using System;

namespace CF.Models.Maintenance
{
    public class EPartMaterial
    {
        public Guid EPartId { get; set; }
        public virtual EPart EPart { get; set; }

        public Guid MaterialId { get; set; }
        public virtual Material Material { get; set; }

        public int Quantity { get; set; }
    }
}
