using CF.Models.Maintenance;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CF.Maps.Maintenance
{
    public class CheckItemMap:EntityTypeConfiguration<CheckItem>
    {
        public CheckItemMap()
        {
            HasMany(ci => ci.AbnormalReasons)
                .WithMany(ar => ar.CheckItems)
                .Map(x =>
                {
                    x.ToTable("CheckItem_AbnormalReasons");
                    x.MapLeftKey("CheckItemId");
                    x.MapRightKey("AbnormalReasonId");
                });
        }
    }
}
