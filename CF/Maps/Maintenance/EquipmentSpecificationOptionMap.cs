using CF.Models.Maintenance;
using System.Data.Entity.ModelConfiguration;

namespace CF.Maps.Maintenance
{
    public class EquipmentSpecificationOptionMap : EntityTypeConfiguration<EquipmentSpecificationOption>
    {
        public EquipmentSpecificationOptionMap()
        {
            HasKey(x => new { x.EquipmentId, x.ESpecificationId, x.ESOptionId });

            HasRequired(eso => eso.ESOption)
                .WithMany(o => o.EquipmentSpecificationOptions)
                .HasForeignKey(eso => eso.ESOptionId).WillCascadeOnDelete(false);

            ToTable("Equipment_Specification_Options");
        }
    }
}
