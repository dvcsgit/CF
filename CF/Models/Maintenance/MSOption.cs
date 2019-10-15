using System;
using System.Collections.Generic;

namespace CF.Models.Maintenance
{
    public class MSOption
    {
        public MSOption()
        {
            MaterialSpecificationOptions = new HashSet<MaterialSpecificationOption>();
        }

        public Guid MSOptionId { get; set; }
        public string Name { get; set; }

        public Guid MSpecificationId { get; set; }
        public virtual MSpecification MSpecification { get; set; }

        public virtual ICollection<MaterialSpecificationOption> MaterialSpecificationOptions { get; set; }

        public int Seq { get; set; }
    }
}
