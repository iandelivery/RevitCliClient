using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public static class GetFamilySymbolsCliHandler
    {
        public static async Task<int> HandleAsync(string[] args, Func<string, object?, Task<int>> sendCommand)
        {
            var familyName = ArgHelper.FindArg(args, "--family", "-f");
            var category = ArgHelper.FindArg(args, "--category", "-c");

            var parameters = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(familyName))
                parameters["family_name"] = familyName;

            if (!string.IsNullOrEmpty(category))
                parameters["category"] = category;

            return await sendCommand("get_family_symbols", parameters.Count > 0 ? parameters : null);
        }
    }
}
