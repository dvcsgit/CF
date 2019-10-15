using CF.Models.Maintenance;
using System.Data.Entity.ModelConfiguration;

namespace CF.Maps.Maintenance
{
    public class EquipmentMaterialMap:EntityTypeConfiguration<EquipmentMaterial>
    {
        public EquipmentMaterialMap()
        {
            HasKey(em => new { em.EquipmentId, em.MaterialId });
            ToTable("Equipment_Materials");
        }
    }
}
