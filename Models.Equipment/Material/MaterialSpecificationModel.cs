using System.Collections.Generic;
using System.Linq;

namespace Models.Maintenance.Material
{
    public class MaterialSpecificationModel
    {
        public string MaterialSpecificationId { get; set; }
        public string Name { get; set; }
        public List<MaterialSpecificationOptionModel> MaterialSpecificationOptionModels { get; set; }
        public string MaterialSpecificationOptionId { get; set; }
        public string MaterialSpecificationOptionValue
        {
            get
            {
                var option = MaterialSpecificationOptionModels.FirstOrDefault(x => x.MaterialSpecificationOptionId == MaterialSpecificationOptionId);
                if (option != null)
                {
                    return option.Name;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string Value { get; set; }
        public int Seq { get; set; }
        public MaterialSpecificationModel()
        {
            MaterialSpecificationOptionModels = new List<MaterialSpecificationOptionModel>();
        }
    }
}
