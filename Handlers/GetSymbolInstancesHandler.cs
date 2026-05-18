using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public static class GetSymbolInstancesHandler
    {
        public static async Task<int> HandleAsync(string[] args, Func<string, object?, Task<int>> sendCommand)
        {
            var symbolId = ArgHelper.GetInt(args, "--symbol-id", "-si");
            if (symbolId == null)
            {
                Console.WriteLine("Error: --symbol-id is required");
                return 1;
            }

            var viewId = ArgHelper.GetInt(args, "--view-id", "-vi");

            var parameters = new Dictionary<string, object>
            {
                ["symbol_id"] = symbolId.Value
            };

            if (viewId != null)
                parameters["view_id"] = viewId.Value;

            return await sendCommand("get_symbol_instances", parameters);
        }
    }
}
