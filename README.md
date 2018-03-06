# UnitConvert
This is a .Net Standard library for unitconvert.io - a simple, innovative api that makes it easy to convert any plain text measurement to another compatible measurement, get information about the measurement, or compare with another measurement.  

## Usage:  
1. Get an api key from https://unitconvert.io. 

2. Instantiate a UnitConvertClient: 
```
UnitConvertClient client = new UnitConvertClient("API_KEY");
```  
  
3. Call any of the following operations from the client:
* Info: `var data = await client.GetMeasurementInfoAsync("10.352 inches");`
* Conversion: `var data = await client.ConvertMeasurementAsync("12g", "mg");`
* Comparison: `bool result = await c.CompareMeasurementsAsync("11 inches", ComparisonType.GreaterThan, "4 cm");`  
  
If an invalid measurement is passed to any of the above operations, an exception will be thrown with an error message. 
