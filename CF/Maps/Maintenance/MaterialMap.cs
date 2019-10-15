using CF.Models.Maintenance;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CF.Maps
{
    public class MaterialMap : EntityTypeConfiguration<Material>
    {
        public MaterialMap()
        {
            Property(m => m.MaterialId)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            //HasMany(m => m.MaterialSpecifications)
            //    .WithMany(s => s.Materials)
            //    .Map(mms =>
            //    {
            //        mms.ToTable("Material_MaterialSpecifications");
            //        mms.MapLeftKey("MaterialId");
            //        mms.MapRightKey("MaterialSpecificationId");
            //    });

            HasRequired(m => m.Organization)
                .WithMany(o => o.Materials)
                .HasForeignKey(m => m.OrganizationId);

            //HasMany(m => m.MaterialSpecificationOptionValues)
            //    .WithRequired(msov => msov.MaterialMiddle)
            //    .HasForeignKey(m => m.MaterialMiddleId);
        }
    }
}
