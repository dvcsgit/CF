using CF.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CF.Maps
{
    public class WebPermissionNameMap: EntityTypeConfiguration<WebPermissionName>
    {
        public WebPermissionNameMap()
        {
            //Property(wpd=>wpd.WebPermissionNameId)
            //    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            HasKey(wpn => new { wpn.WebPermissionId, wpn.Language });
        }
    }
}
