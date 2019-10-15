using CF.Models.Maintenance;
using System.Data.Entity.ModelConfiguration;

namespace CF.Maps.Maintenance
{
    public class LineCheckpointCheckItemMap:EntityTypeConfiguration<LineCheckpointCheckItem>
    {
        public LineCheckpointCheckItemMap()
        {
            ToTable("Line_Checkpoint_CheckItems");
            HasKey(lcc => new { lcc.LineId, lcc.CheckpointId, lcc.CheckItemId });
        }
    }
}
