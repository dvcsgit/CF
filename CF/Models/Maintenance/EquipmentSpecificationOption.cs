using System;

namespace CF.Models.Maintenance
{
    public class EquipmentSpecificationOption
    {
        public Guid EquipmentId { get; set; }
        public Equipment Equipment { get; set; }

        public Guid ESpecificationId { get; set; }
        public ESpecification ESpecification { get; set; }

        public Guid ESOptionId { get; set; }
        public ESOption ESOption { get; set; }

        public string Value { get; set; }

        public int Seq { get; set; }
    }
}
