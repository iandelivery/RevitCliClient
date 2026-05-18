using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public static class HideElementsHandler
    {
        public static async Task<int> HandleAsync(string[] args, Func<string, object?, Task<int>> sendCommand, string command)
        {
            var idsStr = ArgHelper.FindArg(args, "--element-ids", "-e");
            if (idsStr == null)
            {
                Console.WriteLine("Error: --element-ids is required");
                return 1;
            }

            var ids = ArgHelper.ParseIdsToArray(idsStr);
            if (ids == null || ids.Length == 0)
            {
                Console.WriteLine("Error: Invalid element IDs format. Use comma-separated integers.");
                return 1;
            }

            var viewId = ArgHelper.GetInt(args, "--view-id", "-vi");

            var parameters = new Dictionary<string, object>
            {
                ["element_ids"] = ids
            };

            if (viewId != null)
                parameters["view_id"] = viewId.Value;

            return await sendCommand(command, parameters);
        }
    }
}
