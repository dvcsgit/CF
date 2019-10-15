using CF.Models.Maintenance;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CF.Maps.Maintenance
{
    public class MSOptionMap:EntityTypeConfiguration<MSOption>
    {
        public MSOptionMap()
        {
            Property(mso=>mso.MSOptionId)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            //HasMany(mso => mso.MaterialSpecificationOptionValues)
            //    .WithRequired(msov => msov.MaterialSpecificationOptionMiddle)
            //    .HasForeignKey(mso => mso.MaterialSpecificationOptionMiddleId);

            //HasRequired(mso => mso.)
            //    .WithMany(ms => ms.MaterialSpecificationOptions)
            //    .WillCascadeOnDelete(false);
        }
    }
}
