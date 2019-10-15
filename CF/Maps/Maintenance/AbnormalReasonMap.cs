using CF.Models.Maintenance;
using System.Data.Entity.ModelConfiguration;

namespace CF.Maps.Maintenance
{
    public class AbnormalReasonMap:EntityTypeConfiguration<AbnormalReason>
    {
        public AbnormalReasonMap()
        {
            HasRequired(ar => ar.Organization)
                .WithMany(o => o.AbnormalReasons)
                .HasForeignKey(ar => ar.OrganizationId).WillCascadeOnDelete(false);

            HasMany(ar => ar.Solutions)
                .WithMany(s => s.AbnormalReasons)
                .Map(x =>
                {
                    x.ToTable("AbnormalReason_Solutions");
                    x.MapLeftKey("AbnormalReasonId");
                    x.MapRightKey("SolutionId");
                });
        }
    }
}
