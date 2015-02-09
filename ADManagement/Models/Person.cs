using System;
using System.Collections.Generic;
using System.Web;
using System.Xml.Serialization;

namespace ADManagement.Models
{
    public class Person
    {
        public string SAMAccountName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public string Title { get; set; }
        public string Office { get; set; }
        public string Telephone { get; set; }
        
        public int DirectsCount { get; set; }
        public string Department { get; set; }
        public bool DeleteThumbnailPhoto { get; set; }
        public string HasPhoto { get; set; }
        [XmlIgnore]
        public string DistinguishedName { get; set; }
        
        [XmlIgnore]
        public string ManagerDistinguishedName { get; set; }
        public string ManagerName { get; set; }
        public string ManagerSamAccountName { get; set; }

        [XmlIgnore]
        public List<Person> Directs { get; set; }

        [XmlIgnore]
        public List<string> NewDirects { get; set; }

        [XmlIgnore]
        public List<string> MemberOf { get; set; }

        [XmlIgnore]
        public string ThumbnailPhoto { get; set; }
    }
}