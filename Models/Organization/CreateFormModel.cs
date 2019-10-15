using Models.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models.Organization
{
    public class CreateFormModel
    {
        public CreateFormModel()
        {
            FormInput = new FormInput();
            People = new List<PersonModel>();
            EditableOrganizations = new List<EditableOrganizationModel>();
            QueryableOrganizations = new List<QueryableOrganizationModel>();
        }

        public string AncestorOrganizationId { get; set; }

        public string ParentId { get; set; }

        [Display(Name = "ParentOrganization", ResourceType = typeof(Resources.Resource))]
        public string ParentOrganizationFullName { get; set; }

        public FormInput FormInput { get; set; }

        public List<PersonModel> People { get; set; }

        public List<EditableOrganizationModel> EditableOrganizations { get; set; }

        public List<QueryableOrganizationModel> QueryableOrganizations { get; set; }       
    }
}
