using System;
using System.Collections.Generic;
using System.Text;

namespace UnitConvert.Models
{
    public class MeasurementInfo
    {
        public string Unit { get; set; }
        public string Display { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
        public List<string> Variants { get; set; }
        public List<string> ConvertableTo { get; set; }
    }
}
