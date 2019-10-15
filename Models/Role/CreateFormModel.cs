using System;
using System.Collections.Generic;

namespace Models.Role
{
    public class CreateFormModel
    {
        public string AncestorOrganizationId { get; set; }                

        public FormInput FormInput { get; set; }

        public WebPermissionFunctionModel WebPermissionFunction { get; set; }

        public List<PersonModel> People { get; set; }

        public CreateFormModel()
        {
            FormInput = new FormInput();
            WebPermissionFunction = new WebPermissionFunctionModel();
            People = new List<PersonModel>();           
        }
    }
}
