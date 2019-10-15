using CF.Models.Maintenance;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CF.Maps.Maintenance
{
    public class MaterialSpecificationOptionMap:EntityTypeConfiguration<MaterialSpecificationOption>
    {
        public MaterialSpecificationOptionMap()
        {
            //Property(x=>x.MaterialSpecificationOptionValueId)
            //.HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            HasKey(msov => new {msov.MaterialId,msov.MSpecificationId, msov.MSOptionId });

            ToTable("Material_Specification_Options");

            HasRequired(mso => mso.MSpecification)
                .WithMany(ms => ms.MaterialSpecificationOptions)
                .HasForeignKey(mso => mso.MSpecificationId).WillCascadeOnDelete(false);
        }
    }
}
