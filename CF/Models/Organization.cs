using CF.Models.Maintenance;
using System;
using System.Collections.Generic;

namespace CF.Models
{
    public class Organization
    {
        public Organization()
        {
            People =new HashSet<Person>();
            AffiliationOrganizationFor = new HashSet<Equipment>();
            MaintenanceOrganizationFor = new HashSet<Equipment>();
            Materials = new HashSet<Material>();
            MSpecifications = new HashSet<MSpecification>();
            ESpecifications = new HashSet<ESpecification>();
            //MaterialSpecificationOptionValues = new HashSet<MaterialSpecificationOptionValue>();
            Checkpoints = new HashSet<Checkpoint>();
            CheckItems = new HashSet<CheckItem>();
            AbnormalReasons = new HashSet<AbnormalReason>();
            Lines = new HashSet<Line>();
        }
        
        public Guid OrganizationId { get; set; }
        public Guid ParentId { get; set; }
        public string OId { get; set; }
        public string Name { get; set; }

        public OrganizationManager Manager { get; set; }//One to one relationships

        public virtual ICollection<Person> People { get; set; }

        public virtual ICollection<Equipment> AffiliationOrganizationFor { get; set; }
        public virtual ICollection<Equipment> MaintenanceOrganizationFor { get; set; }

        public virtual ICollection<Material> Materials { get; set; }
        public virtual ICollection<MSpecification> MSpecifications { get; set; }
        public virtual ICollection<ESpecification> ESpecifications { get; set; }
        //public virtual ICollection<MaterialSpecificationOptionValue> MaterialSpecificationOptionValues { get; set; }

        public virtual ICollection<Checkpoint> Checkpoints { get; set; }
        public virtual ICollection<CheckItem> CheckItems { get; set; }
        public virtual ICollection<AbnormalReason> AbnormalReasons { get; set; }
        public virtual ICollection<Line> Lines { get; set; }
    }
}
