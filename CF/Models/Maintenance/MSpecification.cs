using System;
using System.Collections.Generic;

namespace CF.Models.Maintenance
{
    public class MSpecification
    {
        public MSpecification()
        {
            MSOptions = new HashSet<MSOption>();
            //Materials = new HashSet<Material>();
            MaterialSpecificationOptions = new HashSet<MaterialSpecificationOption>();
        }

        public Guid MSpecificationId { get; set; }
        public string MaterialType { get; set; }
        public string Name { get; set; }
        public virtual ICollection<MSOption> MSOptions { get; set; }
        public virtual ICollection<MaterialSpecificationOption> MaterialSpecificationOptions { get; set; }

        public Guid OrganizationId { get; set; }
        public virtual Organization Organization { get; set; }

        //public virtual ICollection<Material> Materials { get; set; }
    }
}
