using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Organization
{
    public class EditFormModel
    {
        public string OrganizationId { get; set; }

        public string AncestorOrganizationId { get; set; }

        [Display(Name = "ParentOrganization", ResourceType = typeof(Resources.Resource))]
        public string ParentOrganizationFullName { get; set; }

        public List<string> ManagerList { get; set; }

        public string Managers
        {
            get
            {
                if (ManagerList != null && ManagerList.Count > 0)
                {
                    var sb = new StringBuilder();

                    foreach (var manager in ManagerList)
                    {
                        sb.Append(manager);
                        sb.Append(",");
                    }

                    sb.Remove(sb.Length - 1, 1);

                    return sb.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public FormInput FormInput { get; set; }

        public List<EditableOrganizationModel> EditableOrganizations { get; set; }

        public List<QueryableOrganizationModel> QueryableOrganizations { get; set; }

        public EditFormModel()
        {
            FormInput = new FormInput();           
            ManagerList = new List<string>();
            EditableOrganizations = new List<EditableOrganizationModel>();
            QueryableOrganizations = new List<QueryableOrganizationModel>();
        }
    }
}
