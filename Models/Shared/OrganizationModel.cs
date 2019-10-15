using System;

namespace Models.Shared
{
    public class OrganizationModel
    {
        public Guid AncestorOrganizationId { get; set; }
        public Guid OrganizationId { get; set; }
        public string OId { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string ManagerId { get; set; }
    }
}