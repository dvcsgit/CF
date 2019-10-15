namespace Models.Maintenance.Material
{
    public class EquipmentModel
    {
        public string EId { get; set; }
        public string EquipmentName { get; set; }
        public string EquipmentPartName { get; set; }
        public string OrganizationName { get; set; }
        public string Equipment
        {
            get
            {
                if (!string.IsNullOrEmpty(EId))
                {
                    if (string.IsNullOrEmpty(EquipmentPartName))
                    {
                        return string.Format("{0}/{1}", EId, EquipmentName);
                    }
                    else
                    {
                        return string.Format("{0}/{1}-{2}", EId, EquipmentName, EquipmentPartName);
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public int Quantity { get; set; }
    }
}
