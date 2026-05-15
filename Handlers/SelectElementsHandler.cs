using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public static class SelectElementsHandler
    {
        public static async Task<int> HandleAsync(string[] args, Func<string, object?, Task<int>> sendCommand)
        {
            var elementIds = ArgHelper.FindArg(args, "--element-ids", "-e");

            if (elementIds == null)
            {
                return await sendCommand("select_elements", null);
            }

            var ids = ArgHelper.ParseIdsToArray(elementIds);
            if (ids == null)
            {
                Console.WriteLine("Error: --element-ids format invalid. Expected comma-separated integers");
                return 1;
            }

            var parameters = new Dictionary<string, object>
            {
                ["element_ids"] = ids
            };

            return await sendCommand("select_elements", parameters);
        }
    }
}
