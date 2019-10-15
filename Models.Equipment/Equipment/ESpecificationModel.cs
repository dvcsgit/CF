using System.Collections.Generic;
using System.Linq;

namespace Models.Maintenance.Equipment
{
    public class ESpecificationModel
    {
        public string ESpecificationId { get; set; }

        public string Name { get; set; }

        public List<ESOptionModel> ESOptionModels { get; set; }

        public string ESOptionId { get; set; }

        public string ESOptionValue
        {
            get
            {
                var option = ESOptionModels.FirstOrDefault(x => x.ESOptionId == ESOptionId);

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

        public ESpecificationModel()
        {
            ESOptionModels = new List<ESOptionModel>();
        }
    }
}
