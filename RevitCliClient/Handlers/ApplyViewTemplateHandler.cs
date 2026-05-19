// SPDX-License-Identifier: MIT
using RevitCliClient.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public class ApplyViewTemplateHandler : ICliCommand
    {
        public string CommandName => "apply_view_template";
        public string Description => "Apply view template";
        public string Usage => "apply_view_template -ti <id> | --template-name <name> -vi <id> | --view-ids <id1,id2,...> | -t <type> | -a";
        public CommandCategory Category => CommandCategory.Modify;
        public string[] Examples => new[] { "RevitCliClient.exe apply_view_template -ti 3002 -vi 3001" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var templateId = ArgHelper.GetInt(args, "--template-id", "-ti");
            var templateName = ArgHelper.FindArg(args, "--template-name");
            var viewId = ArgHelper.GetInt(args, "--view-id", "-vi");
            var viewIds = ArgHelper.FindArg(args, "--view-ids");
            var viewType = ArgHelper.FindArg(args, "--view-type", "-t");
            var allViews = ArgHelper.HasFlag(args, "--all-views", "-a");

            if (templateId is null && templateName is null) { Console.WriteLine("Error: --template-id or --template-name is required"); return 1; }
            if (viewId is null && viewIds is null && viewType is null && !allViews) { Console.WriteLine("Error: one of --view-id, --view-ids, --view-type, or --all-views is required"); return 1; }

            var parameters = new Dictionary<string, object>();
            if (templateId is not null) parameters["template_id"] = templateId.Value;
            if (templateName is not null) parameters["template_name"] = templateName;
            if (viewId is not null) parameters["view_id"] = viewId.Value;
            if (viewIds is not null)
            {
                var parsedIds = ArgHelper.ParseIds(viewIds);
                if (parsedIds is null || parsedIds.Count == 0) { Console.WriteLine("Error: --view-ids format invalid"); return 1; }
                parameters["view_ids"] = parsedIds;
            }
            if (viewType is not null) parameters["view_type"] = viewType;
            if (allViews) parameters["all_views"] = true;

            return await sendCommand("apply_view_template", parameters);
        }
    }
}
