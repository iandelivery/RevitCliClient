// SPDX-License-Identifier: MIT
using RevitCliClient.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public class SetActiveViewCliHandler : ICliCommand
    {
        public string CommandName => "set_active_view";
        public string Description => "Set active view";
        public string Usage => "set_active_view -vi <id> | -vn <name>";
        public CommandCategory Category => CommandCategory.ViewExport;
        public string[] Examples => new[]
        {
            "RevitCliClient.exe set_active_view -vi 3001",
            "RevitCliClient.exe set_active_view -vn \"Floor Plan: Level 1\""
        };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var viewId = ArgHelper.GetInt(args, "--view-id", "-vi");
            var viewName = ArgHelper.FindArg(args, "--view-name", "-vn");

            if (viewId is null && viewName is null) { Console.WriteLine("Error: either --view-id or --view-name is required"); return 1; }

            var parameters = new Dictionary<string, object>();
            if (viewId is not null) parameters["view_id"] = viewId.Value;
            if (viewName is not null) parameters["view_name"] = viewName;
            return await sendCommand("set_active_view", parameters);
        }
    }
}
