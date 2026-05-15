using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public static class SetWallsConstraintCliHandler
    {
        public static async Task<int> HandleAsync(string[] args, Func<string, object?, Task<int>> sendCommand)
        {
            var jsonFile = ArgHelper.FindArg(args, "--file", "-fl");
            var jsonInline = ArgHelper.FindArg(args, "--json", "-j");
            var topLevelId = ArgHelper.GetInt(args, "--top-level-id");
            var baseLevelId = ArgHelper.GetInt(args, "--base-level-id");
            var category = ArgHelper.FindArg(args, "--category", "-c");

            if (topLevelId == null && baseLevelId == null)
            {
                Console.WriteLine("Error: at least one of --top-level-id or --base-level-id is required");
                return 1;
            }

            var parameters = new Dictionary<string, object>();

            if (topLevelId != null)
                parameters["top_level_id"] = topLevelId.Value;

            if (baseLevelId != null)
                parameters["base_level_id"] = baseLevelId.Value;

            if (category != null)
                parameters["category"] = category;

            if (jsonFile != null || jsonInline != null)
            {
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
                else
                {
                    jsonContent = jsonInline!;
                }

                try
                {
                    var wallsArray = JsonConvert.DeserializeObject<List<int>>(jsonContent);
                    if (wallsArray == null || wallsArray.Count == 0)
                    {
                        Console.WriteLine("Error: Invalid or empty walls array");
                        return 1;
                    }

                    parameters["walls"] = wallsArray;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: Invalid JSON format - {ex.Message}");
                    return 1;
                }
            }

            return await sendCommand("set_walls_constraint", parameters);
        }
    }
}
