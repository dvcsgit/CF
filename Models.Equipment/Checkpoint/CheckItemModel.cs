namespace Models.Maintenance.Checkpoint
{
    public class CheckItemModel
    {
        public bool IsInherit { get; set; }

        public string CheckItemId { get; set; }

        public string Type { get; set; }
        public string CIId { get; set; }
        public string Name { get; set; }

        public bool IsFeelItem { get; set; }
        public bool IsAccumulation { get; set; }

        public double? OriUpperLimit { get; set; }
        public double? OriUpperAlertLimit { get; set; }
        public double? OriLowerAlertLimit { get; set; }
        public double? OriLowerLimit { get; set; }
        public double? OriAccumulationBase { get; set; }

        public string OriUnit { get; set; }
        public string OriRemark { get; set; }

        public double? UpperLimit { get; set; }
        public double? UpperAlertLimit { get; set; }
        public double? LowerAlertLimit { get; set; }
        public double? LowerLimit { get; set; }
        public double? AccumulationBase { get; set; }

        public string Unit { get; set; }
        public string Remark { get; set; }

        public CheckItemModel()
        {
            IsInherit = true;
        }
    }
}
