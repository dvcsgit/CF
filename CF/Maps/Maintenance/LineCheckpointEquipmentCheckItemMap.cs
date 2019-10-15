using CF.Models.Maintenance;
using System.Data.Entity.ModelConfiguration;

namespace CF.Maps.Maintenance
{
    public class LineCheckpointEquipmentCheckItemMap:EntityTypeConfiguration<LineCheckpointEquipmentCheckItem>
    {
        public LineCheckpointEquipmentCheckItemMap()
        {
            ToTable("Line_Checkpoint_Equipment_CheckItems");
            HasKey(lcec => new { lcec.LineId, lcec.CheckpointId, lcec.EquipmentId, lcec.CheckItemId });
        }
    }
}
