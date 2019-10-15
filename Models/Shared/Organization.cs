using System;

namespace Models.Shared
{
    public class Organization
    {
        public Guid OrganizationId { get; set; }
        public Guid ParentId { get; set; }
        public string OId { get; set; }
        public string Name { get; set; }
    }
}
