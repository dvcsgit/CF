using CF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace Models.Authentication
{
    public class Account
    {
        public Account()
        {
            RoleIds = new List<string>();
            MenuItems = new List<MenuItem>();
            WebPermissionFunctions = new List<WebPermissionWebFunctionModel>();
            OrganizationPermissions = new List<OrganizationPermission>();
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public Guid RootOrganizationId { get; set; }
        public Guid OrganizationId { get; set; }
        public string Photo { get; set; }
        public List<string> RoleIds { get; set; }
        public List<MenuItem> MenuItems { get; set; }
        public List<WebPermissionWebFunctionModel> WebPermissionFunctions { get; set; }
        //public List<WebPermission> WebPermissions { get; set; }
        public List<OrganizationPermission> OrganizationPermissions { get; set; }

        public List<Guid> VisibleOrganizationIds
        {
            get
            {
                return OrganizationPermissions.Select(x => x.OrganizationId).ToList();
            }
        }

        public List<Guid> QueryableOrganizationIds
        {
            get
            {
                return OrganizationPermissions.Where(x => x.Permission == Define.EnumOrganizationPermission.Queryable || x.Permission == Define.EnumOrganizationPermission.Editable).Select(x => x.OrganizationId).ToList();
            }
        }

        public List<Guid> EditableOrganizationIds
        {
            get
            {
                return OrganizationPermissions.Where(x => x.Permission == Define.EnumOrganizationPermission.Editable).Select(x => x.OrganizationId).ToList();
            }
        }

        public Define.EnumOrganizationPermission OrganizationPermission(Guid OrganizationId)
        {
            if (EditableOrganizationIds.Contains(OrganizationId))
            {
                return Define.EnumOrganizationPermission.Editable;
            }
            else if (QueryableOrganizationIds.Contains(OrganizationId))
            {
                return Define.EnumOrganizationPermission.Queryable;
            }
            else if (VisibleOrganizationIds.Contains(OrganizationId))
            {
                return Define.EnumOrganizationPermission.Visible;
            }
            else
            {
                return Define.EnumOrganizationPermission.None;
            }
        }
    }
}
