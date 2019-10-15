using System;

namespace CF.Models.Maintenance
{
    public class LineCheckpointEquipmentCheckItem
    {
        public Guid LineId { get; set; }
        public Line Line { get; set; }

        public Guid CheckpointId { get; set; }
        public Checkpoint Checkpoint { get; set; }

        public Guid EquipmentId { get; set; }
        public Equipment Equipment { get; set; }

        public Guid CheckItemId { get; set; }
        public CheckItem CheckItem { get; set; }
    }
}
