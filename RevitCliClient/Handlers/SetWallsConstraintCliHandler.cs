// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using RevitCliClient.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public class SetWallsConstraintCliHandler : ICliCommand
    {
        public string CommandName => "set_walls_constraint";
        public string Description => "Batch set wall constraints";
        public string Usage => "set_walls_constraint --top-level-id <id> | --base-level-id <id> [-fl <path> | -j <json>] [-c <cat>]";
        public CommandCategory Category => CommandCategory.Modify;
        public string[] Examples => new[] { "RevitCliClient.exe set_walls_constraint --top-level-id 196629 -c OST_Walls" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var jsonFile = ArgHelper.FindArg(args, "--file", "-fl");
            var jsonInline = ArgHelper.FindArg(args, "--json", "-j");
            var topLevelId = ArgHelper.GetInt(args, "--top-level-id");
            var baseLevelId = ArgHelper.GetInt(args, "--base-level-id");
            var category = ArgHelper.FindArg(args, "--category", "-c");

            if (topLevelId is null && baseLevelId is null) { Console.WriteLine("Error: at least one of --top-level-id or --base-level-id is required"); return 1; }

            var parameters = new Dictionary<string, object>();
            if (topLevelId is not null) parameters["top_level_id"] = topLevelId.Value;
            if (baseLevelId is not null) parameters["base_level_id"] = baseLevelId.Value;
            if (category is not null) parameters["category"] = category;

            if (jsonFile is not null || jsonInline is not null)
            {
                string jsonContent;
                if (jsonFile is not null)
                {
                    if (!File.Exists(jsonFile)) { Console.WriteLine($"Error: File not found: {jsonFile}"); return 1; }
                    jsonContent = File.ReadAllText(jsonFile);
                }
                else jsonContent = jsonInline!;

                try
                {
                    var wallsArray = JsonConvert.DeserializeObject<List<int>>(jsonContent);
                    if (wallsArray is null || wallsArray.Count == 0) { Console.WriteLine("Error: Invalid or empty walls array"); return 1; }
                    parameters["walls"] = wallsArray;
                }
                catch (Exception ex) { Console.WriteLine($"Error: Invalid JSON format - {ex.Message}"); return 1; }
            }

            return await sendCommand("set_walls_constraint", parameters);
        }
    }
}
