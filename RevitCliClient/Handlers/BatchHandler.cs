// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using RevitCliClient.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public class BatchHandler : ICliCommand
    {
        public string CommandName => "batch";
        public string Description => "Execute multiple commands in a TransactionGroup";
        public string Usage => "batch -j <json> | -fl <path> [-n <name>] [--no-rollback] [--no-assimilate]";
        public CommandCategory Category => CommandCategory.Modify;
        public string[] Examples => new[]
        {
            "RevitCliClient.exe batch -j \"[{\\\"command\\\":\\\"create_wall\\\",\\\"parameters\\\":{\\\"level_id\\\":3001,\\\"start_x\\\":0,\\\"start_y\\\":0,\\\"end_x\\\":5000,\\\"end_y\\\":0}},{\\\"command\\\":\\\"set_param\\\",\\\"parameters\\\":{\\\"element_id\\\":\\\"$0\\\",\\\"param_name\\\":\\\"Comments\\\",\\\"param_value\\\":\\\"New wall\\\"}}]\""
        };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var jsonFile = ArgHelper.FindArg(args, "--file", "-fl");
            var jsonStr = ArgHelper.FindArg(args, "--json", "-j");
            var name = ArgHelper.FindArg(args, "--name", "-n");
            var noRollback = ArgHelper.HasFlag(args, "--no-rollback");
            var noAssimilate = ArgHelper.HasFlag(args, "--no-assimilate");

            string? operationsJson = null;
            if (!string.IsNullOrEmpty(jsonFile))
            {
                if (!File.Exists(jsonFile)) { Console.WriteLine($"Error: File not found: {jsonFile}"); return 1; }
                operationsJson = File.ReadAllText(jsonFile);
            }
            else if (!string.IsNullOrEmpty(jsonStr)) operationsJson = jsonStr;
            else { Console.WriteLine("Error: --json or --file is required"); return 1; }

            object? parsedOperations;
            try { parsedOperations = JsonConvert.DeserializeObject(operationsJson); }
            catch (Exception ex) { Console.WriteLine($"Error: Invalid JSON: {ex.Message}"); return 1; }

            var isSimpleArray = parsedOperations is Newtonsoft.Json.Linq.JArray;
            object parameters;

            if (isSimpleArray)
            {
                var dict = new Dictionary<string, object> { ["operations"] = parsedOperations! };
                if (!string.IsNullOrEmpty(name)) dict["name"] = name;
                if (noRollback) dict["rollback_on_error"] = false;
                if (noAssimilate) dict["assimilate"] = false;
                parameters = dict;
            }
            else parameters = parsedOperations!;

            return await sendCommand("batch", parameters);
        }
    }
}
