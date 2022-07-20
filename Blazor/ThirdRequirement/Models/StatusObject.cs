using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThirdRequirement.Models
{
    public class StatusObject
    {
        public string Status { get; set; }
        public int Progress { get; set; }
        public string Result { get; set; }
        public string Error { get; set; }
    }
}
