using CF.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CF.Maps
{
    public class WebPermissionMap: EntityTypeConfiguration<WebPermission>
    {
        public WebPermissionMap()
        {
            //Property(w=>w.WebPermissionId)
            //    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            //HasMany(p => p.WebFunctions)
            //    .WithMany(f => f.WebPermissions)
            //    .Map(m =>
            //    {
            //        m.ToTable("WebPermissionWebFunctions");                    
            //        m.MapLeftKey("WebPermissionId");
            //        m.MapRightKey("WebFunctionId");
            //    });
        }
    }
}
