using CF.Models.Maintenance;
using System;
using System.Collections.Generic;

namespace CF.Models
{
    public class Person
    {
        public Person()
        {
            Roles = new HashSet<Role>();
            Lines = new HashSet<Line>();
        }
        public Guid PersonId { get; set; }
        public string LoginId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        //public string UID { get; set; }
        public bool IsMobilePerson { get; set; }
        public DateTime? LastModifyTime { get; set; }
        //public string ManagerId { get; set; }

        public virtual ICollection<Role> Roles { get; set; }//collection navigation property
        
        public Guid OrganizationId { get; set; }//Foreign key
        public virtual Organization Organization { get; set; }//reference navigation property

        public virtual ICollection<Line> Lines { get; set; }

        public virtual ICollection<Job> Jobs { get; set; }
    }
}
