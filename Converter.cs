using System.Linq;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace UnitConverter
{
    public class Converter
    {
        private Dictionary<string, Dictionary<string, double>> _config {get;set;}
        public Converter(string config) {
            _config = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, double>>>(config);
        }
        public double ConvertSimple(string from, string to, double number){
            string fromType = _config.Where(x => x.Value.ContainsKey(from)).First().Key;
            string toType = _config.Where(x => x.Value.ContainsKey(to)).First().Key;
            if(fromType == toType) {
                return number / _config[toType][to] * _config[fromType][from];
            } 
            else if (fromType == "metric" && toType == "imperial") {
                return ConvertSimple("inch", to, ConvertSimple(from,"meter", number) / 0.0254);
            } 
            else if (fromType == "imperial" && toType == "metric") {
                return ConvertSimple("meter", to, ConvertSimple(from, "inch", number) * 0.0254);
            }
            throw new ArgumentException($"Can't convert {from} to {to}");
        }
        public double ConvertAdvanced(string fromDistance, string toDistance, string fromTime, string toTime, double number) => number * ConvertSimple(fromDistance, toDistance, 1) / ConvertSimple(fromTime, toTime, 1);
    }
}