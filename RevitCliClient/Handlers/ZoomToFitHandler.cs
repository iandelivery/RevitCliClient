// SPDX-License-Identifier: MIT
using RevitCliClient.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public class ZoomToFitHandler : ICliCommand
    {
        public string CommandName => "zoom_to_fit";
        public string Description => "Zoom to fit";
        public string Usage => "zoom_to_fit [-e <id> | --element-ids <id1,id2,...>]";
        public CommandCategory Category => CommandCategory.ViewExport;
        public string[] Examples => new[] { "RevitCliClient.exe zoom_to_fit -e 599906" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var elementId = ArgHelper.GetInt(args, "--element-id", "-e");
            var elementIds = ArgHelper.FindArg(args, "--element-ids");

            if (elementId is null && elementIds is null)
                return await sendCommand("zoom_to_fit", null);

            if (elementId is not null)
                return await sendCommand("zoom_to_fit", new Dictionary<string, object> { ["element_id"] = elementId.Value });

            if (elementIds is not null)
            {
                var ids = ArgHelper.ParseIdsToArray(elementIds);
                if (ids is null) { Console.WriteLine("Error: --element-ids format invalid"); return 1; }
                return await sendCommand("zoom_to_fit", new Dictionary<string, object> { ["element_ids"] = ids });
            }

            return 1;
        }
    }
}
