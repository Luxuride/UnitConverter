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
        // Core of converter
        // Does basic convert and conversion between units of different type
        public double Convert(string from, string to, double number){
            try
            {
                //Get Unit types
                string fromType = _config.First(x => x.Value.ContainsKey(from.Split("countper")[0])).Key;
                string fromTypePer = (from.Split("countper").Length == 2) ? _config.First(x => x.Value.ContainsKey(from.Split("countper")[1])).Key : "";

                string toType = _config.First(x => x.Value.ContainsKey(to.Split("countper")[0])).Key;
                string toTypePer = (to.Split("countper").Length == 2) ? _config.First(x => x.Value.ContainsKey(to.Split("countper")[1])).Key : "";

                //Advanced unit conversion
                if (_config.ContainsKey($"{fromType}countto{toType}") ||
                    _config.ContainsKey($"{toType}countto{fromType}"))
                {
                    //Get advanced convert unit type
                    string advancedType = _config.First(x =>
                        x.Key == $"{fromType}countto{toType}" || x.Key == $"{toType}countto{fromType}").Key;
                    //Get Units
                    string fromBasic = _config[advancedType].First(x => _config[fromType].ContainsKey(x.Key)).Key;
                    string toBasic = _config[advancedType].First(x => _config[toType].ContainsKey(x.Key)).Key;
                    return Convert(toBasic, to, Convert(from, fromBasic, number)) * _config[advancedType][toBasic] /
                           _config[advancedType][fromBasic];
                }
                //Convert from Unit to Unit/Unit or Unit/Unit to Unit
                else if (_config.ContainsKey($"{fromType}countto{toType}countper{toTypePer}") ||
                         _config.ContainsKey($"{toType}countto{fromType}countper{fromTypePer}"))
                {
                    string advancedType = _config.First(x =>
                        x.Key == $"{fromType}countto{toType}countper{toTypePer}" ||
                        x.Key == $"{toType}countto{fromType}countper{fromTypePer}").Key;
                    string fromBasic = _config[advancedType]
                        .First(x => _config[fromType].ContainsKey(x.Key.Split("countper")[0])).Key.Split("countper")[0];
                    string toBasic = _config[advancedType]
                        .First(x => _config[toType].ContainsKey(x.Key.Split("countper")[0])).Key.Split("countper")[0];
                    if (fromTypePer != "")
                    {
                        string fromBasicPer = _config[advancedType].First(x =>
                            x.Key.Split("countper").Length == 2 &&
                            _config[fromTypePer].ContainsKey(x.Key.Split("countper")[1])).Key.Split("countper")[1];
                        return Convert(toBasic, to,
                            Convert(from.Split("countper")[0], fromBasic, 1) /
                            Convert(from.Split("countper")[1], fromBasicPer, 1) * number);
                    }

                    if (toTypePer != "")
                    {
                        string toBasicPer = _config[advancedType].First(x =>
                            x.Key.Split("countper").Length == 2 &&
                            _config[toTypePer].ContainsKey(x.Key.Split("countper")[1])).Key.Split("countper")[1];
                        return Convert(from, fromBasic, 1) / Convert(to.Split("countper")[0], toBasic, 1) *
                               Convert(to.Split("countper")[1], toBasicPer, 1)  * number;
                    }
                }
                //Basic conversion
                else if (fromType == toType)
                {
                    return number / _config[toType][to] * _config[fromType][from];
                }
            } catch{}
            //If there is no conversion path return not convertable message
            throw new ArgumentException($"Can't convert {from} to {to}");
        }
        //Converts Unit/Unit to Unit/Unit
        public double Convert(string fromFirst, string toFirst, string fromSecond, string toSecond, double number) => number * Convert(fromFirst, toFirst, 1) / Convert(fromSecond, toSecond, 1);
        //Converts Unit to Unit/Unit or Unit/Unit to Unit
        public double Convert(string first, string second, string third, double number, int pos) => (pos == 0) ? Convert($"{first}countper{second}", third, number) : Convert(first, $"{second}countper{third}", number);
    }
}