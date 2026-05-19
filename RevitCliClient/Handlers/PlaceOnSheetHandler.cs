// SPDX-License-Identifier: MIT
using RevitCliClient.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public class PlaceOnSheetHandler : ICliCommand
    {
        public string CommandName => "place_on_sheet";
        public string Description => "Place viewport on sheet";
        public string Usage => "place_on_sheet -vi <id> --sheet-id <id> [--x <mm>] [--y <mm>]";
        public CommandCategory Category => CommandCategory.Modify;
        public string[] Examples => new[] { "RevitCliClient.exe place_on_sheet -vi 3001 --sheet-id 4001" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var viewId = ArgHelper.GetInt(args, "--view-id", "-vi");
            var sheetId = ArgHelper.GetInt(args, "--sheet-id");
            var x = ArgHelper.GetDouble(args, "--x");
            var y = ArgHelper.GetDouble(args, "--y");
            var viewportTypeId = ArgHelper.GetInt(args, "--viewport-type-id");

            if (viewId is null) { Console.WriteLine("Error: --view-id is required"); return 1; }
            if (sheetId is null) { Console.WriteLine("Error: --sheet-id is required"); return 1; }

            var parameters = new Dictionary<string, object> { ["view_id"] = viewId.Value, ["sheet_id"] = sheetId.Value };
            if (x is not null) parameters["x"] = x.Value;
            if (y is not null) parameters["y"] = y.Value;
            if (viewportTypeId is not null) parameters["viewport_type_id"] = viewportTypeId.Value;

            return await sendCommand("place_on_sheet", parameters);
        }
    }
}
