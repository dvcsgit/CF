using CF.Models.Maintenance;
using System.Data.Entity.ModelConfiguration;

namespace CF.Maps.Maintenance
{
    public class LineMap:EntityTypeConfiguration<Line>
    {
        public LineMap()
        {
            HasRequired(l => l.Organization)
                .WithMany(o => o.Lines)
                .HasForeignKey(l => l.OrganizationId).WillCascadeOnDelete(false);
        }
    }
}
