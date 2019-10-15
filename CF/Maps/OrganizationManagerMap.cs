using CF.Models;
using System.Data.Entity.ModelConfiguration;

namespace CF.Maps
{
    public class OrganizationManagerMap:EntityTypeConfiguration<OrganizationManager>
    {
        public OrganizationManagerMap()
        {
            HasKey(om => om.OrganizationId);
            //One to one relationship.Set the field "OrganizationId" to be the pirmary key and foreign key.
            HasRequired(om => om.ManagerOf).WithOptional(o => o.Manager);            
        }
    }
}
