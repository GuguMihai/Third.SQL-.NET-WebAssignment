using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThirdRequirement.Models
{
    public class DataModel
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public DateTime ComputationStartDatetime { get; set; }
        public int ComputationRequiredTime { get; set; }
        public string Result { get; set; }
    }
}
