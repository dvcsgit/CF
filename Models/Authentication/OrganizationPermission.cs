using System;
using Utility;

namespace Models.Authentication
{
    public class OrganizationPermission
    {
        public Guid OrganizationId { get; set; }
        public Define.EnumOrganizationPermission Permission { get; set; }
    }
}
