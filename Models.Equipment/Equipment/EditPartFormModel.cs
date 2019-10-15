namespace Models.Maintenance.Equipment
{
    public class EditPartFormModel
    {
        public string EPartId { get; set; }

        public PartFormInput PartFormInput { get; set; }

        public EditPartFormModel()
        {
            PartFormInput = new PartFormInput();
        }
    }
}
