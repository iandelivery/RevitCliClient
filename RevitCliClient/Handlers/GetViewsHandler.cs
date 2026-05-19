// SPDX-License-Identifier: MIT
using RevitCliClient.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public class GetViewsHandler : ICliCommand
    {
        public string CommandName => "views";
        public string Description => "List views";
        public string Usage => "views [-t <type>] [--template <name>] [--templates-only]";
        public CommandCategory Category => CommandCategory.Query;
        public string[] Examples => new[] { "RevitCliClient.exe views -t plan" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var viewType = ArgHelper.FindArg(args, "--type", "-t");
            var templateName = ArgHelper.FindArg(args, "--template");
            var isTemplate = ArgHelper.HasFlag(args, "--templates-only");
            var parameters = new Dictionary<string, object>();
            if (viewType is not null) parameters["type"] = viewType;
            if (templateName is not null) parameters["template"] = templateName;
            if (isTemplate) parameters["is_template"] = true;
            return await sendCommand("get_views", parameters.Count > 0 ? parameters : null);
        }
    }
}
