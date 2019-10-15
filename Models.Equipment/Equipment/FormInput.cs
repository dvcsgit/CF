using System.ComponentModel.DataAnnotations;

namespace Models.Maintenance.Equipment
{
    public class FormInput
    {
        [Display(Name = "EId", ResourceType = typeof(Resources.Resource))]
        [Required(ErrorMessageResourceName = "EIdRequired", ErrorMessageResourceType = typeof(Resources.Resource))]
        public string EId { get; set; }

        [Display(Name = "EquipmentName", ResourceType = typeof(Resources.Resource))]
        [Required(ErrorMessageResourceName = "EquipmentNameRequired", ErrorMessageResourceType = typeof(Resources.Resource))]
        public string Name { get; set; }

        [Display(Name = "MaintenanceOrganization", ResourceType = typeof(Resources.Resource))]
        public string MaintenanceOrganizationId { get; set; }
    }
}
