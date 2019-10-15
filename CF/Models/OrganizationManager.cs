using System;

namespace CF.Models
{
    public class OrganizationManager
    {
        public Guid OrganizationId { get; set; }
        public string ManagerId { get; set; }
        public virtual Organization ManagerOf { get; set; }
    }
}
