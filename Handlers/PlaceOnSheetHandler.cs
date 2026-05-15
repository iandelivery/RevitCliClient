using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public static class PlaceOnSheetHandler
    {
        public static async Task<int> HandleAsync(string[] args, Func<string, object?, Task<int>> sendCommand)
        {
            var viewId = ArgHelper.GetInt(args, "--view-id", "-vi");
            var sheetId = ArgHelper.GetInt(args, "--sheet-id");
            var x = ArgHelper.GetDouble(args, "--x");
            var y = ArgHelper.GetDouble(args, "--y");
            var viewportTypeId = ArgHelper.GetInt(args, "--viewport-type-id");

            if (viewId == null)
            {
                Console.WriteLine("Error: --view-id is required");
                return 1;
            }

            if (sheetId == null)
            {
                Console.WriteLine("Error: --sheet-id is required");
                return 1;
            }

            var parameters = new Dictionary<string, object>
            {
                ["view_id"] = viewId.Value,
                ["sheet_id"] = sheetId.Value
            };

            if (x != null)
                parameters["x"] = x.Value;

            if (y != null)
                parameters["y"] = y.Value;

            if (viewportTypeId != null)
                parameters["viewport_type_id"] = viewportTypeId.Value;

            return await sendCommand("place_on_sheet", parameters);
        }
    }
}
