using System;
using System.Collections.Generic;

namespace CF.Models.Maintenance
{
    public class Job
    {
        public Job()
        {
            People = new HashSet<Person>();
        }

        public Guid JobId { get; set; }
        public string Name { get; set; }
        public bool IsCheckBySeq { get; set; }
        public bool IsShowPrevRecord { get; set; }
        public bool IsNeedVerify { get; set; }
        public string CycleMode { get; set; }
        public int CycleCount { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TimeMode { get; set; }
        public string BeginTime { get; set; }
        public string EndTime { get; set; }
        public string Remark { get; set; }
        public DateTime LastModifyTime { get; set; }

        public Guid LineId { get; set; }
        public Line Line { get; set; }

        public virtual ICollection<Person> People { get; set; }
    }
}
