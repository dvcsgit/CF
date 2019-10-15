using CF.Models.Maintenance;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CF.Maps.Maintenance
{
    public class EquipmentMap : EntityTypeConfiguration<Equipment>
    {
        public EquipmentMap()
        {
            Property(e=>e.EquipmentId)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            HasOptional(e => e.AffiliationOrganization).WithMany(o => o.AffiliationOrganizationFor);//Cannot use HasRequired().
            HasOptional(e => e.MaintenanceOrganization).WithMany(o => o.MaintenanceOrganizationFor);

            //HasMany(e => e.EquipmentSpecifications)
            //    .WithMany(s => s.Equipments)
            //    .Map(ees =>
            //    {
            //        ees.ToTable("Equipment_EquipmentSpecifications");
            //        ees.MapLeftKey("EquipmentId");
            //        ees.MapRightKey("EquipmentSpecificationId");
            //    });

            //HasMany(e => e.Materials)
            //    .WithMany(m => m.Equipments)
            //    .Map(em =>
            //    {
            //        em.ToTable("Equipment_Materials");
            //        em.MapLeftKey("EquipmentId");
            //        em.MapRightKey("MaterialId");
            //    });

            HasMany(e => e.CheckItems)
                .WithMany(c => c.Equipments)
                .Map(x =>
                {
                    x.ToTable("Equipment_CheckItems");
                    x.MapLeftKey("EquipmentId");
                    x.MapRightKey("CheckItemId");
                });
        }
    }
}
