using System;
using System.Collections.Generic;

namespace CF.Models.Maintenance
{
    public class CheckItem
    {
        public CheckItem()
        {
            Checkpoints = new HashSet<Checkpoint>();
            Equipments = new HashSet<Equipment>();
            EParts = new HashSet<EPart>();
            FeelOptions = new HashSet<FeelOption>();
            AbnormalReasons = new HashSet<AbnormalReason>();
            LineCheckpointCheckItems = new HashSet<LineCheckpointCheckItem>();
            LineCheckpointEquipmentCheckItems = new HashSet<LineCheckpointEquipmentCheckItem>();
        }

        public Guid CheckItemId { get; set; }
        public string CIId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsFeelItem { get; set; }
        public bool IsAccumulation { get; set; }
        public bool IsInherit { get; set; }
        public Nullable<double> LowerLimit { get; set; }
        public Nullable<double> LowerAlertLimit { get; set; }
        public Nullable<double> UpperLimit { get; set; }
        public Nullable<double> UpperAlertLimit { get; set; }
        public Nullable<double> AccumulationBase { get; set; }
        public string Unit { get; set; }
        public string Remark { get; set; }
        public Nullable<int> TextValueType { get; set; }
        public DateTime LastModifyTime { get; set; }

        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; }

        public virtual ICollection<FeelOption> FeelOptions { get; set; }

        public virtual ICollection<AbnormalReason> AbnormalReasons { get; set; }

        public virtual ICollection<Checkpoint> Checkpoints { get; set; }
        public virtual ICollection<Equipment> Equipments { get; set; }
        public virtual ICollection<EPart> EParts { get; set; }

        public virtual ICollection<LineCheckpointCheckItem> LineCheckpointCheckItems { get; set; }
        public virtual ICollection<LineCheckpointEquipmentCheckItem> LineCheckpointEquipmentCheckItems { get; set; }
    }
}
