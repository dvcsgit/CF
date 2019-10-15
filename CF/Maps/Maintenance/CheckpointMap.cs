using CF.Models.Maintenance;
using System.Data.Entity.ModelConfiguration;

namespace CF.Maps.Maintenance
{
    public class CheckpointMap:EntityTypeConfiguration<Checkpoint>
    {
        public CheckpointMap()
        {
            HasRequired(c => c.Organization)
                .WithMany(o => o.Checkpoints)
                .HasForeignKey(c => c.OrganizationId).WillCascadeOnDelete(false);

            HasMany(c => c.CheckItems)
                .WithMany(ci => ci.Checkpoints)
                .Map(x =>
                {
                    x.ToTable("Checkpoint_CheckItems");
                    x.MapLeftKey("CheckpointId");
                    x.MapRightKey("CheckItemId");
                });
        }
    }
}
