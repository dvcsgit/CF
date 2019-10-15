using System;
using System.Collections.Generic;

namespace CF.Models.Maintenance
{
    public class ESOption
    {
        public ESOption()
        {
            EquipmentSpecificationOptions = new HashSet<EquipmentSpecificationOption>();
        }

        public Guid ESOptionId { get; set; }
        public string Name { get; set; }
        public int Seq { get; set; }

        public Guid ESpecificationId { get; set; }
        public virtual ESpecification ESpecification { get; set; }

        public virtual ICollection<EquipmentSpecificationOption> EquipmentSpecificationOptions { get; set; }
    }
}
