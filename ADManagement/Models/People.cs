using System;
using System.Collections.Generic;
using System.Web;
using System.Xml.Serialization;

namespace ADManagement.Models
{
    public class People
    {
        public string SAMAccountName { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public string Title { get; set; }
        public string Office { get; set; }
        public string Telephone { get; set; }
        public string Department { get; set; }
        public bool HasPhoto { get; set; }
        
        [XmlIgnore]
        public string DistinguishedName { get; set; }
    }
}