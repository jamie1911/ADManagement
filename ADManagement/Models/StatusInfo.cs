using System;
using System.Collections.Generic;
using System.Web;

namespace ADManagement.Models
{
    public class StatusInfo
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public List<StatusDetails> StatusDetail { get; set; }
    }
}