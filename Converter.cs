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
        public double Convert(string from, string to, double number){
            try {
                string fromType = _config.Where(x => x.Value.ContainsKey(from)).First().Key;
                string toType = _config.Where(x => x.Value.ContainsKey(to)).First().Key;
                if (_config.ContainsKey($"{fromType}to{toType}") || _config.ContainsKey($"{toType}to{fromType}")){
                    string advancedType = _config.Where(x => x.Key == $"{fromType}to{toType}" || x.Key == $"{toType}to{fromType}").First().Key;
                    string fromBasic = _config[advancedType].Where(x => _config[fromType].ContainsKey(x.Key)).First().Key;
                    string toBasic = _config[advancedType].Where(x => _config[toType].ContainsKey(x.Key)).First().Key;
                    return Convert(toBasic, to, Convert(from, fromBasic, number)) * _config[advancedType][toBasic] / _config[advancedType][fromBasic];
                }
                else if(fromType == toType) {
                    return number / _config[toType][to] * _config[fromType][from];
                }
            } finally {
                throw new ArgumentException($"Can't convert {from} to {to}");
            }
        }
        public double Convert(string fromFirst, string toFirst, string fromSecond, string toSecond, double number) => number * Convert(fromFirst, toFirst, 1) / Convert(fromSecond, toSecond, 1);
        public double Convert(string first, string second, string third, double number, int pos) => (pos == 0) ? Convert($"{first}per{second}", third, number) : Convert(first, $"{second}per{third}", number);
    }
}