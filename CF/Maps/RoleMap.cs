using CF.Models;
using System.Data.Entity.ModelConfiguration;

namespace CF.Maps
{
    public class RoleMap: EntityTypeConfiguration<Role>
    {
        public RoleMap()
        {
            Property(r => r.Name)
                .IsRequired();

            //Property(r => r.RoleId)
            //    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}
