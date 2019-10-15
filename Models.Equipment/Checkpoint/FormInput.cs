using System.ComponentModel.DataAnnotations;

namespace Models.Maintenance.Checkpoint
{
    public class FormInput
    {
        [Display(Name = "CId", ResourceType = typeof(Resources.Resource))]
        [Required(ErrorMessageResourceName = "CIdRequired", ErrorMessageResourceType = typeof(Resources.Resource))]
        public string CId { get; set; }

        [Display(Name = "CheckpointName", ResourceType = typeof(Resources.Resource))]
        [Required(ErrorMessageResourceName = "CheckpointNameRequired", ErrorMessageResourceType = typeof(Resources.Resource))]
        public string Name { get; set; }

        [Display(Name = "TagId", ResourceType = typeof(Resources.Resource))]
        public string TagId { get; set; }

        [Display(Name = "IsFeelItemDefaultNormal", ResourceType = typeof(Resources.Resource))]
        public bool IsFeelItemDefaultNormal { get; set; }

        [Display(Name = "Remark", ResourceType = typeof(Resources.Resource))]
        public string Remark { get; set; }
    }
}
