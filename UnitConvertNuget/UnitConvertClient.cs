using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnitConvert.Models;

namespace UnitConvert
{
    public class UnitConvertClient
    {
        private string apiKey;
        private HttpClient httpClient;

        /// <summary>
        /// Construct a unitconvert client
        /// </summary>
        /// <param name="apiKey">Your unitconvert api key</param>
        public UnitConvertClient(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentException("You must supply an api key", "apiKey");
            }

            this.apiKey = apiKey;
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("api-key", apiKey);
        }

        /// <summary>
        /// Gets the information for a supplied measurement string
        /// </summary>
        /// <param name="measurement">A plain text measurement, e.g. "10mg" or "24.5 pounds"</param>
        /// <returns>Measurement definition data if supplied measurement is valid. Throws exception if invalid</returns>
        public async Task<MeasurementInfo> GetMeasurementInfoAsync(string measurement)
        {
            if (string.IsNullOrEmpty(measurement))
            {
                throw new ArgumentException("You must supply a measurement", "measurement");
            }

            HttpResponseMessage response = await httpClient.GetAsync("https://unitconvertapi.azurewebsites.net/api/v1/Measurements/Info?measurement=" + measurement);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<MeasurementInfo>(await response.Content.ReadAsStringAsync());
            }
            else
            {
                RequestErrorObject error = JsonConvert.DeserializeObject<RequestErrorObject>(await response.Content.ReadAsStringAsync());
                throw new Exception(error.Error);
            }
        }

        /// <summary>
        /// Converts one measurement to another type of measurement
        /// </summary>
        /// <param name="fromMeasurement">A plain text measurement, e.g. "10mg" or "24.5 pounds"</param>
        /// <param name="toUnit">The measurement unit to convert to, e.g. "grams" or "ounces"</param>
        /// <returns>Measurement definition data if supplied measurement is valid. Throws exception if invalid</returns>
        public async Task<MeasurementInfo> ConvertMeasurementAsync(string fromMeasurement, string toUnit)
        {
            if (string.IsNullOrEmpty(fromMeasurement))
            {
                throw new ArgumentException("You must supply a measurement to convert from", "fromMeasurement");
            }
            if (string.IsNullOrEmpty(toUnit))
            {
                throw new ArgumentException("You must supply a unit to convert to", "toUnit");
            }

            HttpResponseMessage response = await httpClient.GetAsync("https://unitconvertapi.azurewebsites.net/api/v1/Measurements/Convert?from=" + fromMeasurement + "&to=" + toUnit);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<MeasurementInfo>(await response.Content.ReadAsStringAsync());
            }
            else
            {
                RequestErrorObject error = JsonConvert.DeserializeObject<RequestErrorObject>(await response.Content.ReadAsStringAsync());
                throw new Exception(error.Error);
            }
        }

        /// <summary>
        /// Compares two supplied measurements, if they are comparable with one another
        /// </summary>
        /// <param name="firstMeasurement">The first measurement to compare</param>
        /// <param name="conversionType">The type of comparison to perform</param>
        /// <param name="secondMeasurement">The second measurement to compare</param>
        /// <returns>True or false. Throws exception if supplied measurement data is invalid</returns>
        public async Task<bool> CompareMeasurementsAsync(string firstMeasurement, ComparisonType conversionType, string secondMeasurement)
        {
            if (string.IsNullOrEmpty(firstMeasurement))
            {
                throw new ArgumentException("You must supply a first measurement", "firstMeasurement");
            }
            if (string.IsNullOrEmpty(secondMeasurement))
            {
                throw new ArgumentException("You must supply a measurement to convert to", "secondMeasurement");
            }

            string comparer = "";
            switch (conversionType)
            {
                case ComparisonType.Equals:
                    comparer = "==";
                    break;
                case ComparisonType.GreaterThan:
                    comparer = ">";
                    break;
                case ComparisonType.GreaterThanOrEqual:
                    comparer = ">=";
                    break;
                case ComparisonType.LessThan:
                    comparer = "<";
                    break;
                case ComparisonType.LessThanOrEqual:
                    comparer = "<=";
                    break;
                case ComparisonType.NotEquals:
                    comparer = "!=";
                    break;
                default:
                    throw new ArgumentException("Invalid conversion type specified", "conversionType");
            }


            // first if there are no units, just compare the numbers
            bool canDoNumericComparison = false;
            bool numericCompareTrue = attemptNumericComparison(firstMeasurement, comparer, secondMeasurement, out canDoNumericComparison);
            if (canDoNumericComparison)
            {
                return numericCompareTrue;
            }


            HttpResponseMessage response = await httpClient.GetAsync("https://unitconvertapi.azurewebsites.net/api/v1/Measurements/Compare?first=" + firstMeasurement + "&comparer=" + comparer + "&second=" + secondMeasurement);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<ComparisonResult>(await response.Content.ReadAsStringAsync()).Result;
            }
            else
            {
                RequestErrorObject error = JsonConvert.DeserializeObject<RequestErrorObject>(await response.Content.ReadAsStringAsync());
                throw new Exception(error.Error);
            }
        }



        private bool attemptNumericComparison(string first, string comparer, string second, out bool canDoNumericComparison)
        {
            canDoNumericComparison = false;

            bool compareOutput = false;

            decimal firstNumber = 0;
            decimal secondNumber = 0;

            bool firstNumeric = decimal.TryParse(first, out firstNumber);
            bool secondNumeric = decimal.TryParse(second, out secondNumber);

            if (firstNumeric && secondNumeric)
            {
                canDoNumericComparison = true;
                switch (comparer)
                {
                    case "==":
                        if (firstNumber == secondNumber)
                        {
                            compareOutput = true;
                        }
                        break;
                    case ">":
                        if (firstNumber > secondNumber)
                        {
                            compareOutput = true;
                        }
                        break;
                    case ">=":
                        if (firstNumber >= secondNumber)
                        {
                            compareOutput = true;
                        }
                        break;
                    case "<":
                        if (firstNumber < secondNumber)
                        {
                            compareOutput = true;
                        }
                        break;
                    case "<=":
                        if (firstNumber <= secondNumber)
                        {
                            compareOutput = true;
                        }
                        break;
                    case "!=":
                        if (firstNumber != secondNumber)
                        {
                            compareOutput = true;
                        }
                        break;
                }
            }

            return compareOutput;
        }
    }
}
