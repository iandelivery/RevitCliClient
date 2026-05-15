using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public static class ApplyViewTemplateHandler
    {
        public static async Task<int> HandleAsync(string[] args, Func<string, object?, Task<int>> sendCommand)
        {
            var templateId = ArgHelper.GetInt(args, "--template-id", "-ti");
            var templateName = ArgHelper.FindArg(args, "--template-name");
            var viewId = ArgHelper.GetInt(args, "--view-id", "-vi");
            var viewIds = ArgHelper.FindArg(args, "--view-ids");
            var viewType = ArgHelper.FindArg(args, "--view-type", "-t");
            var allViews = ArgHelper.HasFlag(args, "--all-views", "-a");

            if (templateId == null && templateName == null)
            {
                Console.WriteLine("Error: --template-id or --template-name is required");
                return 1;
            }

            if (viewId == null && viewIds == null && viewType == null && !allViews)
            {
                Console.WriteLine("Error: one of --view-id, --view-ids, --view-type, or --all-views is required");
                return 1;
            }

            var parameters = new Dictionary<string, object>();

            if (templateId != null)
                parameters["template_id"] = templateId.Value;

            if (templateName != null)
                parameters["template_name"] = templateName;

            if (viewId != null)
                parameters["view_id"] = viewId.Value;

            if (viewIds != null)
            {
                var parsedIds = ArgHelper.ParseIds(viewIds);
                if (parsedIds == null || parsedIds.Count == 0)
                {
                    Console.WriteLine("Error: --view-ids format invalid. Expected comma-separated integers");
                    return 1;
                }
                parameters["view_ids"] = parsedIds;
            }

            if (viewType != null)
                parameters["view_type"] = viewType;

            if (allViews)
                parameters["all_views"] = true;

            return await sendCommand("apply_view_template", parameters);
        }
    }
}
