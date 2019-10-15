using CF.Models.Maintenance;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CF.Maps.Maintenance
{
    public class MSpecificationMap:EntityTypeConfiguration<MSpecification>
    {
        public MSpecificationMap()
        {
            Property(ms=>ms.MSpecificationId)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            HasRequired(ms => ms.Organization)
                .WithMany(o => o.MSpecifications)
                .HasForeignKey(ms => ms.OrganizationId).WillCascadeOnDelete(false);

            //HasMany(ms => ms.MaterialSpecificationOptionValues)
            //    .WithRequired(msov => msov.MaterialSpecificationMiddle)
            //    .HasForeignKey(ms => ms.MaterialSpecificationMiddleId);
        }
    }
}
