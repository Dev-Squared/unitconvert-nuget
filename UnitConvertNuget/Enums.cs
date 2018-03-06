using System;
using System.Collections.Generic;
using System.Text;

namespace UnitConvert
{
    public enum ComparisonType
    {
        Equals,
        NotEquals,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual
    }

    public enum ConversionResult
    {
        Ok,
        Error
    }
}
