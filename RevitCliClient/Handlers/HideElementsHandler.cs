// SPDX-License-Identifier: MIT
using RevitCliClient.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public class HideElementsHandler : ICliCommand
    {
        private readonly string _serverCommand;

        public HideElementsHandler(string serverCommand)
        {
            _serverCommand = serverCommand;
        }

        public string CommandName => _serverCommand == "hide_elements" ? "hide_elements" : "unhide_elements";
        public string Description => _serverCommand == "hide_elements" ? "Hide elements in view" : "Unhide elements in view";
        public string Usage => $"{CommandName} -e <id1,id2,...> [-vi <id>]";
        public CommandCategory Category => CommandCategory.Modify;
        public string[] Examples => _serverCommand == "hide_elements"
            ? new[] { "RevitCliClient.exe hide_elements -e 599906,599841" }
            : new[] { "RevitCliClient.exe unhide_elements -e 599906,599841 -vi 3001" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var idsStr = ArgHelper.FindArg(args, "--element-ids", "-e");
            if (idsStr is null) { Console.WriteLine("Error: --element-ids is required"); return 1; }

            var ids = ArgHelper.ParseIdsToArray(idsStr);
            if (ids is null || ids.Length == 0) { Console.WriteLine("Error: Invalid element IDs format"); return 1; }

            var viewId = ArgHelper.GetInt(args, "--view-id", "-vi");
            var parameters = new Dictionary<string, object> { ["element_ids"] = ids };
            if (viewId is not null) parameters["view_id"] = viewId.Value;

            return await sendCommand(_serverCommand, parameters);
        }
    }
}
