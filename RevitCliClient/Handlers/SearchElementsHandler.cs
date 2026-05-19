// SPDX-License-Identifier: MIT
using RevitCliClient.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public class SearchElementsHandler : ICliCommand
    {
        public string CommandName => "search_elements";
        public string Description => "Search elements by parameter";
        public string Usage => "search_elements -c <cat> --param-name <name> [--param-value <val>] [--param-operator <eq|neq|gt|lt|contains|empty|notempty>] [--limit <n>]";
        public CommandCategory Category => CommandCategory.Query;
        public string[] Examples => new[] { "RevitCliClient.exe search_elements -c OST_Walls --param-name \"Comments\" --param-value \"demolish\"" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var category = ArgHelper.FindArg(args, "--category", "-c");
            var paramName = ArgHelper.FindArg(args, "--param-name");
            var paramValue = ArgHelper.FindArg(args, "--param-value");
            var paramOperator = ArgHelper.FindArg(args, "--param-operator");
            var limit = ArgHelper.GetInt(args, "--limit");

            if (category is null) { Console.WriteLine("Error: --category is required"); return 1; }
            if (paramName is null) { Console.WriteLine("Error: --param-name is required"); return 1; }
            if (paramValue is null && paramOperator != "empty") { Console.WriteLine("Error: --param-value is required (unless --param-operator is 'empty')"); return 1; }

            var parameters = new Dictionary<string, object> { ["category"] = category, ["param_name"] = paramName };
            if (paramValue is not null) parameters["param_value"] = paramValue;
            if (paramOperator is not null) parameters["param_operator"] = paramOperator;
            if (limit is not null) parameters["limit"] = limit.Value;
            return await sendCommand("search_elements", parameters);
        }
    }
}
