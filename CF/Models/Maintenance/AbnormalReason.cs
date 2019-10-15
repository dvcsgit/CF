using System;
using System.Collections.Generic;

namespace CF.Models.Maintenance
{
    public class AbnormalReason
    {
        public AbnormalReason()
        {
            Solutions = new HashSet<Solution>();
            CheckItems = new HashSet<CheckItem>();
        }

        public Guid AbnormalReasonId { get; set; }
        public string ARId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public DateTime LastModifyTime { get; set; }

        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; }

        public virtual ICollection<Solution> Solutions { get; set; }

        public virtual ICollection<CheckItem> CheckItems { get; set; }
    }
}
