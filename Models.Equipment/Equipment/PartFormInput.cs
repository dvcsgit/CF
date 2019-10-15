using System.ComponentModel.DataAnnotations;

namespace Models.Maintenance.Equipment
{
    public class PartFormInput
    {
        [Display(Name = "PartName", ResourceType = typeof(Resources.Resource))]
        [Required(ErrorMessageResourceName = "PartNameRequired", ErrorMessageResourceType = typeof(Resources.Resource))]
        public string Name { get; set; }
    }
}
