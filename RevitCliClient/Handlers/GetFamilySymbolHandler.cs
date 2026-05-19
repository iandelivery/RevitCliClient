// SPDX-License-Identifier: MIT
using RevitCliClient.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public class GetFamilySymbolHandler : ICliCommand
    {
        public string CommandName => "get_family_symbol";
        public string Description => "Get specific FamilySymbol ID";
        public string Usage => "get_family_symbol --instance-id <id> | -f <name> -s <name> [-c <cat>]";
        public CommandCategory Category => CommandCategory.Query;
        public string[] Examples => new[] { "RevitCliClient.exe get_family_symbol -f \"M_Single-Flush\" -s \"0915 x 2134mm\"" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var instanceId = ArgHelper.GetInt(args, "--instance-id");
            var familyName = ArgHelper.FindArg(args, "--family", "-f");
            var symbolName = ArgHelper.FindArg(args, "--symbol", "-s");
            var category = ArgHelper.FindArg(args, "--category", "-c");

            if (instanceId is not null)
                return await sendCommand("get_family_symbol", new Dictionary<string, object> { ["instance_id"] = instanceId.Value });

            if (string.IsNullOrEmpty(familyName)) { Console.WriteLine("Error: --family is required (or use --instance-id)"); return 1; }
            if (string.IsNullOrEmpty(symbolName)) { Console.WriteLine("Error: --symbol is required"); return 1; }

            var parameters = new Dictionary<string, object> { ["family_name"] = familyName, ["symbol_name"] = symbolName };
            if (!string.IsNullOrEmpty(category)) parameters["category"] = category;
            return await sendCommand("get_family_symbol", parameters);
        }
    }
}
