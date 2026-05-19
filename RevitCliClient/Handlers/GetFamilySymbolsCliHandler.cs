// SPDX-License-Identifier: MIT
using RevitCliClient.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public class GetFamilySymbolsCliHandler : ICliCommand
    {
        public string CommandName => "family_symbols";
        public string Description => "List FamilySymbols";
        public string Usage => "family_symbols [-f <name>] [-c <cat>]";
        public CommandCategory Category => CommandCategory.Query;
        public string[] Examples => new[] { "RevitCliClient.exe family_symbols -c OST_Doors" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var familyName = ArgHelper.FindArg(args, "--family", "-f");
            var category = ArgHelper.FindArg(args, "--category", "-c");
            var parameters = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(familyName)) parameters["family_name"] = familyName;
            if (!string.IsNullOrEmpty(category)) parameters["category"] = category;
            return await sendCommand("get_family_symbols", parameters.Count > 0 ? parameters : null);
        }
    }
}
