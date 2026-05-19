// SPDX-License-Identifier: MIT
using RevitCliClient.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public class GetSymbolInstancesHandler : ICliCommand
    {
        public string CommandName => "get_symbol_instances";
        public string Description => "Get all instances of a FamilySymbol";
        public string Usage => "get_symbol_instances -si <id> [-vi <id>]";
        public CommandCategory Category => CommandCategory.Query;
        public string[] Examples => new[]
        {
            "RevitCliClient.exe get_symbol_instances -si 950367",
            "RevitCliClient.exe get_symbol_instances -si 950367 -vi 3001"
        };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var symbolId = ArgHelper.GetInt(args, "--symbol-id", "-si");
            if (symbolId is null) { Console.WriteLine("Error: --symbol-id is required"); return 1; }

            var viewId = ArgHelper.GetInt(args, "--view-id", "-vi");
            var parameters = new Dictionary<string, object> { ["symbol_id"] = symbolId.Value };
            if (viewId is not null) parameters["view_id"] = viewId.Value;
            return await sendCommand("get_symbol_instances", parameters);
        }
    }
}
