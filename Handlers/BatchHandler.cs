using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public static class BatchHandler
    {
        public static async Task<int> HandleAsync(string[] args, Func<string, object?, Task<int>> sendCommand)
        {
            var jsonFile = ArgHelper.FindArg(args, "--file", "-fl");
            var jsonStr = ArgHelper.FindArg(args, "--json", "-j");
            var name = ArgHelper.FindArg(args, "--name", "-n");
            var noRollback = ArgHelper.HasFlag(args, "--no-rollback");
            var noAssimilate = ArgHelper.HasFlag(args, "--no-assimilate");

            string? operationsJson = null;

            if (!string.IsNullOrEmpty(jsonFile))
            {
                if (!File.Exists(jsonFile))
                {
                    Console.WriteLine($"Error: File not found: {jsonFile}");
                    return 1;
                }
                operationsJson = File.ReadAllText(jsonFile);
            }
            else if (!string.IsNullOrEmpty(jsonStr))
            {
                operationsJson = jsonStr;
            }
            else
            {
                Console.WriteLine("Error: --json or --file is required");
                return 1;
            }

            object? parsedOperations;
            try
            {
                parsedOperations = Newtonsoft.Json.JsonConvert.DeserializeObject(operationsJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: Invalid JSON: {ex.Message}");
                return 1;
            }

            var isSimpleArray = parsedOperations is Newtonsoft.Json.Linq.JArray;

            object parameters;

            if (isSimpleArray)
            {
                var dict = new Dictionary<string, object>
                {
                    ["operations"] = parsedOperations!
                };
                if (!string.IsNullOrEmpty(name))
                    dict["name"] = name;
                if (noRollback)
                    dict["rollback_on_error"] = false;
                if (noAssimilate)
                    dict["assimilate"] = false;
                parameters = dict;
            }
            else
            {
                parameters = parsedOperations!;
            }

            return await sendCommand("batch", parameters);
        }
    }
}
