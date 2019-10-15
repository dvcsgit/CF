using System;
using System.Collections.Generic;

namespace CF.Models.Maintenance
{
    public class Solution
    {
        public Solution()
        {
            AbnormalReasons = new HashSet<AbnormalReason>();
        }

        public Guid SolutionId { get; set; }
        public string SId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public DateTime LastModifyTime { get; set; }

        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; }

        public virtual ICollection<AbnormalReason> AbnormalReasons { get; set; }
    }
}
