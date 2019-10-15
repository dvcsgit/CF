using CF.Models.Maintenance;
using System.Data.Entity.ModelConfiguration;

namespace CF.Maps.Maintenance
{
    public class JobMap:EntityTypeConfiguration<Job>
    {
        public JobMap()
        {
            HasMany(j => j.People)
                .WithMany(p => p.Jobs)
                .Map(x =>
                {
                    x.ToTable("JobPeople");
                    x.MapLeftKey("JobId");
                    x.MapRightKey("PersonId");
                });
        }
    }
}
