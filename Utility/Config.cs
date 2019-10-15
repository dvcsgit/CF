using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Utility.Models;

namespace Utility
{
    public class Config
    {
        #region Global
        private static XElement Root
        {
            get
            {
                return XDocument.Load(Define.ConfigFile).Root;
            }
        }

        public static Dictionary<string,string> SystemName
        {
            get
            {
                var element = Root.Element("SystemName");
                return new Dictionary<string, string>()
                {
                    {"zh-cn",element.Attribute("zh-cn").Value },
                    {"us-en",element.Attribute("us-en").Value }
                };
            }
        }

        public static string LogFolder
        {
            get
            {
                return Root.Element("Folder").Element("Log").Value;
            }
        }
        #endregion

        #region LDAP
        public static bool HaveLDAPSettings
        {
            get
            {
                return Root.Element("LDAP") != null;
            }
        }

        public static string LDAP_Domain
        {
            get
            {
                return Root.Element("LDAP").Attribute("Domain").Value;
            }
        }

        public static string LDAP_LoginId
        {
            get
            {
                return Root.Element("LDAP").Attribute("LoginId").Value;
            }
        }

        public static string LDAP_Password
        {
            get
            {
                return Root.Element("LDAP").Attribute("Password").Value;
            }
        }
        #endregion

        #region UserPhoto
        public static string UserPhotoFolderPath
        {
            get
            {
                return Root.Element("Folder").Element("UserPhoto").Value;
            }
        }

        public static string UserPhotoVirtualPath
        {
            get
            {
                return Root.Element("Floder").Element("UserPhoto").Attribute("VirtualPath").Value;
            }
        }
        #endregion

        public static List<PopulationLimit> PopulationLimits
        {
            get
            {
                var populationLimits = new List<PopulationLimit>();

                try
                {
                    var elements = Root.Element("PopulationLimits").Elements("PopulationLimit").ToList();

                    foreach(var element in elements)
                    {
                        populationLimits.Add(new PopulationLimit()
                        {
                            OrganizationId = new Guid(element.Attribute("OrganizationId").Value),
                            NumberOfPeople = int.Parse(element.Attribute("NumberOfPeople").Value),
                            NumberOfMobilePeople = int.Parse(element.Attribute("NumberOfMobilePeople").Value)
                        });
                    }
                }
                catch
                {
                    populationLimits = new List<PopulationLimit>();
                }

                return populationLimits;
            }
        }

        public static string MaintenancePhotoFolderPath
        {
            get
            {
                return Root.Element("Folder").Element("Maintenance").Element("Photo").Value;
            }
        }
    }
}
