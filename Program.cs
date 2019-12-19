using System.Collections.Generic;
using System;
using System.IO;
using System.Threading.Tasks;

namespace UnitConverter
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Input templates:\nSimple convert - number fromUnit toUnit\nAdvanced convert - number fromUnit1/fromUnit2 toUnit1/toUnit2");
            var userInput = "";
            // When you create converter you send him config
            Converter converter = new Converter(await File.ReadAllTextAsync("./config.json"));
            while((userInput = Console.ReadLine().ToLower()) != "exit") {
                var values = userInput.Split(' ');
                try {
                    if(double.TryParse(values[0], out double number)) {
                        // Convert advanced value
                        if(values[1].Split('/').Length == 2 && values[2].Split('/').Length == 2) {
                            var values1 = values[1].Split('/');
                            var values2 = values[2].Split('/');
                            Console.WriteLine($"{values[0]} {values[1]} -> {converter.Convert(values1[0], values2[0], values1[1], values2[1], number)} {values[2]}");
                        } 
                        // Convert simple value
                        else {
                            Console.WriteLine($"{values[0]} {values[1]} -> {converter.Convert(values[1], values[2], number)} {values[2]}");
                        }
                    }
                } catch(Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
