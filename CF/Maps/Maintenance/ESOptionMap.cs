using CF.Models.Maintenance;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CF.Maps.Maintenance
{
    public class ESOptionMap:EntityTypeConfiguration<ESOption>
    {
        public ESOptionMap()
        {
            Property(eso=>eso.ESOptionId)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}
