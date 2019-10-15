using CF.Models.Maintenance;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CF.Maps.Maintenance
{
    public class ESpecificationMap:EntityTypeConfiguration<ESpecification>
    {
        public ESpecificationMap()
        {
            Property(es=>es.ESpecificationId)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}
