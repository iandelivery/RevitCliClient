// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using RevitCliClient.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public class CreateWallsHandler : ICliCommand
    {
        public string CommandName => "create_walls";
        public string Description => "Batch create walls";
        public string Usage => "create_walls -fl <path> | -j <json>";
        public CommandCategory Category => CommandCategory.Create;
        public string[] Examples => new[]
        {
            "RevitCliClient.exe create_walls -j \"[{\\\"start_x\\\":0,\\\"start_y\\\":0,\\\"end_x\\\":5000,\\\"end_y\\\":0,\\\"level_id\\\":3001}]\""
        };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
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
                if (wallsArray is null || wallsArray.Count == 0)
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
