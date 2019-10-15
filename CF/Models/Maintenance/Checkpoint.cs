using System;
using System.Collections.Generic;

namespace CF.Models.Maintenance
{
    public class Checkpoint
    {
        public Checkpoint()
        {
            CheckItems = new HashSet<CheckItem>();
            //Lines = new HashSet<Line>();
            Equipments = new HashSet<Equipment>();
            LineCheckpointCheckItems = new HashSet<LineCheckpointCheckItem>();
            LineCheckpointEquipmentCheckItems = new HashSet<LineCheckpointEquipmentCheckItem>();
        }

        public Guid CheckpointId { get; set; }
        public string CId { get; set; }
        public string Name { get; set; }
        public bool IsFeelItemDefaultNormal { get; set; }
        public string TagId { get; set; }
        public string Remark { get; set; }
        public DateTime LastModifyTime { get; set; }

        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; }

        public virtual ICollection<CheckItem> CheckItems { get; set; }

        //public virtual ICollection<Line> Lines { get; set; }
        public virtual ICollection<Equipment> Equipments { get; set; }

        public virtual ICollection<LineCheckpointCheckItem> LineCheckpointCheckItems { get; set; }
        public virtual ICollection<LineCheckpointEquipmentCheckItem> LineCheckpointEquipmentCheckItems { get; set; }
    }
}
