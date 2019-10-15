using System;

namespace Utility.Models
{
    public class PopulationLimit
    {
        public Guid OrganizationId { get; set; }
        public int NumberOfPeople { get; set; }
        public int NumberOfMobilePeople { get; set; }
    }
}
