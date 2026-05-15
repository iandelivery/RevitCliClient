using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public static class GetFamilySymbolHandler
    {
        public static async Task<int> HandleAsync(string[] args, Func<string, object?, Task<int>> sendCommand)
        {
            var instanceId = ArgHelper.GetInt(args, "--instance-id");
            var familyName = ArgHelper.FindArg(args, "--family", "-f");
            var symbolName = ArgHelper.FindArg(args, "--symbol", "-s");
            var category = ArgHelper.FindArg(args, "--category", "-c");

            if (instanceId != null)
            {
                var parameters = new Dictionary<string, object>
                {
                    ["instance_id"] = instanceId.Value
                };

                return await sendCommand("get_family_symbol", parameters);
            }

            if (string.IsNullOrEmpty(familyName))
            {
                Console.WriteLine("Error: --family is required (or use --instance-id)");
                return 1;
            }

            if (string.IsNullOrEmpty(symbolName))
            {
                Console.WriteLine("Error: --symbol is required");
                return 1;
            }

            var parametersByName = new Dictionary<string, object>
            {
                ["family_name"] = familyName,
                ["symbol_name"] = symbolName
            };

            if (!string.IsNullOrEmpty(category))
            {
                parametersByName["category"] = category;
            }

            return await sendCommand("get_family_symbol", parametersByName);
        }
    }
}
