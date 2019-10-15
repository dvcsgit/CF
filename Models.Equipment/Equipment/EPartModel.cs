using System.Collections.Generic;

namespace Models.Maintenance.Equipment
{
    public class EPartModel
    {
        public string EPartId { get; set; }

        public string Name { get; set; }

        public List<MaterialModel> MaterialModels { get; set; }

        public EPartModel()
        {
            MaterialModels = new List<MaterialModel>();
        }
    }
}
