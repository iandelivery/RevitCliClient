using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public static class SetActiveViewCliHandler
    {
        public static async Task<int> HandleAsync(string[] args, Func<string, object?, Task<int>> sendCommand)
        {
            var viewId = ArgHelper.GetInt(args, "--view-id", "-vi");
            var viewName = ArgHelper.FindArg(args, "--view-name", "-vn");

            if (viewId == null && viewName == null)
            {
                Console.WriteLine("Error: either --view-id or --view-name is required");
                return 1;
            }

            var parameters = new Dictionary<string, object>();

            if (viewId != null)
                parameters["view_id"] = viewId.Value;

            if (viewName != null)
                parameters["view_name"] = viewName;

            return await sendCommand("set_active_view", parameters);
        }
    }
}
