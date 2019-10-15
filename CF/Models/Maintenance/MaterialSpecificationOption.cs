using System;

namespace CF.Models.Maintenance
{
    public class MaterialSpecificationOption
    {
        public Guid MaterialId { get; set; }
        public Material Material { get; set; }

        public Guid MSpecificationId { get; set; }
        public MSpecification MSpecification { get; set; }

        public Guid MSOptionId { get; set; }
        public MSOption MSOption { get; set; }

        public string Value { get; set; }

        public int Seq { get; set; }
    }
}
