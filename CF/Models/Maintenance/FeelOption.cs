using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CF.Models.Maintenance
{
    public class FeelOption
    {
        public Guid FeelOptionId { get; set; }

        public string Name { get; set; }

        public bool IsAbnormal { get; set; }

        public int Seq { get; set; }

        public Guid CheckItemId { get; set; }
        public CheckItem CheckItem { get; set; }
    }
}
