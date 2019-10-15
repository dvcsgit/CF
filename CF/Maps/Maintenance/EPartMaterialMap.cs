using CF.Models.Maintenance;
using System.Data.Entity.ModelConfiguration;

namespace CF.Maps.Maintenance
{
    public class EPartMaterialMap:EntityTypeConfiguration<EPartMaterial>
    {
        public EPartMaterialMap()
        {
            HasKey(epm => new { epm.EPartId, epm.MaterialId });
            ToTable("EPart_Materials");
        }
    }
}
