namespace Models.Maintenance.Equipment
{
    public class CreatePartFormModel
    {
        public PartFormInput FormInput { get; set; }

        public CreatePartFormModel()
        {
            FormInput = new PartFormInput();
        }
    }
}
