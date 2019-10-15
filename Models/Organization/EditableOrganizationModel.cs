using System;

namespace Models.Organization
{
    public class EditableOrganizationModel
    {
        public bool CanDelete { get; set; }

        public Guid OrganizationId { get; set; }

        public string FullName { get; set; }
    }
}
