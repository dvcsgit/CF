using System;
using System.Collections.Generic;

namespace CF.Models.Maintenance
{
    public class Line
    {
        public Line()
        {
            //Checkpoints = new HashSet<Checkpoint>();
            LineCheckpointCheckItems = new HashSet<LineCheckpointCheckItem>();
            LineCheckpointEquipmentCheckItems = new HashSet<LineCheckpointEquipmentCheckItem>();
        }

        public Guid LineId { get; set; }
        public string LId { get; set; }
        public string Name { get; set; }
        public DateTime LastModifyTime { get; set; }

        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; }

        public Guid ManagerId { get; set; }
        public Person Manager { get; set; }

        //public virtual ICollection<Checkpoint> Checkpoints { get; set; }
        public virtual ICollection<LineCheckpointCheckItem> LineCheckpointCheckItems { get; set; }
        public virtual ICollection<LineCheckpointEquipmentCheckItem> LineCheckpointEquipmentCheckItems { get; set; }
        
    }
}
