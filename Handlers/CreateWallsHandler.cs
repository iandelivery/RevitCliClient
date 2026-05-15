using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public static class CreateWallsHandler
    {
        public static async Task<int> HandleAsync(string[] args, Func<string, object?, Task<int>> sendCommand)
        {
            var jsonFile = ArgHelper.FindArg(args, "--file", "-fl");
            var jsonInline = ArgHelper.FindArg(args, "--json", "-j");

            string jsonContent;
            if (jsonFile != null)
            {
                if (!File.Exists(jsonFile))
                {
                    Console.WriteLine($"Error: File not found: {jsonFile}");
                    return 1;
                }
                jsonContent = File.ReadAllText(jsonFile);
            }
            else if (jsonInline != null)
            {
                jsonContent = jsonInline;
            }
            else
            {
                Console.WriteLine("Error: --file or --json is required");
                return 1;
            }

            try
            {
                var wallsArray = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonContent);
                if (wallsArray == null || wallsArray.Count == 0)
                {
                    Console.WriteLine("Error: Invalid or empty walls array");
                    return 1;
                }

                return await sendCommand("create_walls", new { walls = wallsArray });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: Invalid JSON format - {ex.Message}");
                return 1;
            }
        }
    }
}
