using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public static class GetViewsHandler
    {
        public static async Task<int> HandleAsync(string[] args, Func<string, object?, Task<int>> sendCommand)
        {
            var viewType = ArgHelper.FindArg(args, "--type", "-t");
            var templateName = ArgHelper.FindArg(args, "--template");
            var isTemplate = ArgHelper.HasFlag(args, "--templates-only");

            var parameters = new Dictionary<string, object>();

            if (viewType != null)
                parameters["type"] = viewType;

            if (templateName != null)
                parameters["template"] = templateName;

            if (isTemplate)
                parameters["is_template"] = true;

            return await sendCommand("get_views", parameters.Count > 0 ? parameters : null);
        }
    }
}
