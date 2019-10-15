using CF.Models.Maintenance;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CF.Maps.Maintenance
{
    public class EPartMap:EntityTypeConfiguration<EPart>
    {
        public EPartMap()
        {
            Property(ep=>ep.EPartId)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            //HasMany(p => p.Materials)
            //    .WithMany(m => m.EquipmentParts)
            //    .Map(epm =>
            //    {
            //        epm.ToTable("EquipmentPart_Materials");
            //        epm.MapLeftKey("EquipmentPartId");
            //        epm.MapRightKey("MaterialId");
            //    });

            HasMany(ep => ep.CheckItems)
                .WithMany(ci => ci.EParts)
                .Map(x =>
                {
                    x.ToTable("EPart_CheckItems");
                    x.MapLeftKey("EPartId");
                    x.MapRightKey("CheckItemId");
                });
        }
    }
}
