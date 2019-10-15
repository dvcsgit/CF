using System;
using System.IO;
using System.Web.Mvc;

namespace Utility
{
    public class Define
    {
        public const string New = "New";
        public const string Other = "Other";

        #region Log
        private static string LogFileName
        {
            get
            {
                return DateTimeHelper.DateTimeToDateString(DateTime.Today) + ".log";
            }
        }

        public static string LogFile
        {
            get
            {
                return Path.Combine(Config.LogFolder, LogFileName);
            }
        }
        #endregion

        #region DateTime Format
        public const char DateTimeFormat_DateSeperator = '-';
        public const char DateTimeFormat_TimeSeperator = ':';
        public const string DateTimeFormat_DateString = "yyyyMMdd";
        public const string DateTimeFormat_TimeString = "HHmmss";

        public static string DateTimeFormat_DateStringWithSeperator
        {
            get
            {
                return string.Format("yyyy{0}MM{0}dd", DateTimeFormat_DateSeperator);
            }
        }

        public static string DateTimeFormat_TimeStringWithSeperator
        {
            get
            {
                return string.Format("HH{0}mm{0}ss", DateTimeFormat_TimeSeperator);
            }
        }
        #endregion

        #region
        private const string ConfigFileName = "Utility.Config.xml";
        public static string ConfigFile
        {
            get
            {
                string exePath = System.AppDomain.CurrentDomain.BaseDirectory;
                string filePath = Path.Combine(exePath, ConfigFileName);
                if (File.Exists(filePath))
                {
                    return filePath;
                }
                else
                {
                    filePath = Path.Combine(exePath, "bin", ConfigFileName);
                    if (File.Exists(filePath))
                    {
                        return filePath;
                    }
                    else
                    {
                        return ConfigFileName;
                    }
                }
            }
        }
        #endregion

        #region Enum
        public enum EnumOrganizationPermission
        {
            None,
            Visible,
            Queryable,
            Editable
        }

        public enum EnumFormAction
        {
            Create,
            Edit
        }

        public enum EnumExcelVersion
        {
            _2003,
            _2007
        }

        public enum EnumTreeAttribute
        {
            NodeType,
            ToolTip,
            OrganizationPermission,
            OrganizationId,
            PersonId,
            AbnormalType,
            AbnormalReasonId,
            CheckType,
            CheckItemId,
            EquipmentId,
            CheckpointId,
            RouteId,
            JobId,
            EquipmentSpecificationId,
            EquipmentType,
            MaterialSpecificationId,
            MaterialType,
            MaterialId,
            SolutionType,
            SolutionId,
            PartId,
            MaintenanceType,
            StandardId,
            FolderId,
            FileId,
            EmgContactId,
            FlowId,
            IsNew,
            IsError,
            TruckId,
            RepairFormTypeId,
            RepairFormColumnId,                       
            RepairFormSubjectId,
            ManagerID,
            Manager,
            StationId,
            IslandId,
            PortId,
            LoginId
        }

        public enum EnumTreeNodeType
        {
            Organization,
            Person,
            AbnormalType,
            AbnormalReason,
            CheckType,
            CheckItem,
            Equipment,
            Checkpoint,
            Route,
            Job,
            EquipmentType,
            EquipmentSpecification,
            MaterialType,
            MaterialSpecification,
            Material,
            SolutionType,
            Solution,
            EquipmentPart,
            MaintenanceType,
            Standard,
            Folder,
            File,
            EmgContact,
            Flow,
            Truck,
            RepairFormType,
            RepairFormColumn,
            Pipeline,
            PipelineSpec,
            SpecType,
            PipePoint,
            PipePointType,
            Shift,
            ConstructionRoot,
            InspectionRoot,
            PipelineAbnormalRoot,
            PipePointRoot,
            Construction,
            Inspection,
            PipelineAbnormal,
            RepairFormSubject,
            Station,
            Island,
            Port
        }

        public enum EnumMoveDirection
        {
            Up,
            Down
        }
        #endregion

        #region Seperator
        public const string Seperator = "[|]";

        public static string[] Seperators
        {
            get
            {
                return new string[] { Seperator };
            }
        }
        #endregion

        public static SelectListItem DefaultSelectListItem(string DefaultOption)
        {
            return new SelectListItem()
            {
                Selected = true,
                Text = "= " + DefaultOption + " =",
                Value = ""
            };
        }

        public static SelectListItem SelectListItem(string DefaultOption)
        {
            return new SelectListItem()
            {
                Selected = true,
                Text = DefaultOption,
                Value = DefaultOption
            };
        }
    }
}
