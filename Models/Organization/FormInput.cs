using System.ComponentModel.DataAnnotations;

namespace Models.Organization
{
    public class FormInput
    {
        [Display(Name = "OId", ResourceType = typeof(Resources.Resource))]
        [Required(ErrorMessageResourceName = "OIdRequired", ErrorMessageResourceType = typeof(Resources.Resource))]
        public string OId { get; set; }

        [Display(Name = "OrganizationName", ResourceType = typeof(Resources.Resource))]
        [Required(ErrorMessageResourceName = "OrganizationNameRequired", ErrorMessageResourceType = typeof(Resources.Resource))]
        public string Name { get; set; }

        [Display(Name = "Manager", ResourceType = typeof(Resources.Resource))]
        public string Managers { get; set; }
    }
}
