using System.ComponentModel.DataAnnotations;

namespace Models.Maintenance.Material
{
    public class FormInput
    {
        [Display(Name ="MaterialName",ResourceType =typeof(Resources.Resource))]
        public string MaterialType { get; set; }

        [Display(Name = "MId", ResourceType = typeof(Resources.Resource))]
        [Required(ErrorMessageResourceName ="MIdRequired",ErrorMessageResourceType =typeof(Resources.Resource))]
        public string MId { get; set; }

        [Display(Name = "MaterialName", ResourceType = typeof(Resources.Resource))]
        [Required(ErrorMessageResourceName = "MaterialNameRequired", ErrorMessageResourceType = typeof(Resources.Resource))]
        public string MaterialName { get; set; }

        [Display(Name ="Quantity",ResourceType =typeof(Resources.Resource))]
        public int? Quantity { get; set; }
    }
}
