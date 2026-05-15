using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public static class ZoomToFitHandler
    {
        public static async Task<int> HandleAsync(string[] args, Func<string, object?, Task<int>> sendCommand)
        {
            var elementId = ArgHelper.GetInt(args, "--element-id", "-e");
            var elementIds = ArgHelper.FindArg(args, "--element-ids");

            if (elementId == null && elementIds == null)
            {
                return await sendCommand("zoom_to_fit", null);
            }

            if (elementId != null)
            {
                var parameters = new Dictionary<string, object>
                {
                    ["element_id"] = elementId.Value
                };

                return await sendCommand("zoom_to_fit", parameters);
            }

            if (elementIds != null)
            {
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

                return await sendCommand("zoom_to_fit", parameters);
            }

            return 1;
        }
    }
}
