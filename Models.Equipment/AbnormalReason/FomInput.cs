using System.ComponentModel.DataAnnotations;

namespace Models.Maintenance.AbnormalReason
{
    public class FormInput
    {
        [Display(Name = "AbnormalReasonType", ResourceType = typeof(Resources.Resource))]
        public string Type { get; set; }

        [Display(Name = "ARId", ResourceType = typeof(Resources.Resource))]
        [Required(ErrorMessageResourceName = "ARIdRequired", ErrorMessageResourceType = typeof(Resources.Resource))]
        public string ARId { get; set; }

        [Display(Name = "AbnormalReasonName", ResourceType = typeof(Resources.Resource))]
        [Required(ErrorMessageResourceName = "AbnormalReasonNameRequired", ErrorMessageResourceType = typeof(Resources.Resource))]
        public string Name { get; set; }
    }
}
