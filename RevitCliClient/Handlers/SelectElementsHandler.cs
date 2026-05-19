// SPDX-License-Identifier: MIT
using RevitCliClient.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public class SelectElementsHandler : ICliCommand
    {
        public string CommandName => "select_elements";
        public string Description => "Get or set selection";
        public string Usage => "select_elements [-e <ids>]";
        public CommandCategory Category => CommandCategory.ViewExport;
        public string[] Examples => new[] { "RevitCliClient.exe select_elements -e 599906,599841" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var elementIds = ArgHelper.FindArg(args, "--element-ids", "-e");

            if (elementIds is null)
                return await sendCommand("select_elements", null);

            var ids = ArgHelper.ParseIdsToArray(elementIds);
            if (ids is null) { Console.WriteLine("Error: --element-ids format invalid"); return 1; }

            return await sendCommand("select_elements", new Dictionary<string, object> { ["element_ids"] = ids });
        }
    }
}
