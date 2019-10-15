namespace Models.Shared
{
    public class PersonModel
    {
        public string OrganizationId { get; set; }

        public string OrganizationName { get; set; }

        public string ID { get; set; }

        public string Name { get; set; }

        public string Person
        {
            get
            {
                if (!string.IsNullOrEmpty(Name))
                {
                    return string.Format("{0}/{1}", ID, Name);
                }
                else
                {
                    return ID;
                }
            }
        }

        public string Email { get; set; }

        public string ManagerId { get; set; }

        public override bool Equals(object Object)
        {
            var key = Object as PersonModel;

            return Equals(key);
        }

        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }

        public bool Equals(PersonModel personModel)
        {
            if (personModel == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(personModel.ID))
            {
                return false;
            }

            return this.ID.Equals(personModel.ID);
        }
    }
}
