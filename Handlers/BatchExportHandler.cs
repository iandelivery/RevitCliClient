using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public static class BatchExportHandler
    {
        public static async Task<int> HandleAsync(string[] args, Func<string, object?, Task<int>> sendCommand)
        {
            var format = ArgHelper.FindArg(args, "--format", "-f");
            var viewIds = ArgHelper.FindArg(args, "--view-ids");
            var sheetIds = ArgHelper.FindArg(args, "--sheet-ids");
            var outputDir = ArgHelper.FindArg(args, "--output-dir", "-o");
            var pdfSetup = ArgHelper.FindArg(args, "--pdf-setup");
            var dwgSetup = ArgHelper.FindArg(args, "--dwg-setup");
            var allSheets = ArgHelper.HasFlag(args, "--all-sheets", "-a");

            if (format == null)
            {
                Console.WriteLine("Error: --format is required (pdf, dwg, img)");
                return 1;
            }

            if (viewIds == null && sheetIds == null && !allSheets)
            {
                Console.WriteLine("Error: one of --view-ids, --sheet-ids, or --all-sheets is required");
                return 1;
            }

            var parameters = new Dictionary<string, object>
            {
                ["format"] = format
            };

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

            if (sheetIds != null)
            {
                var parsedIds = ArgHelper.ParseIds(sheetIds);
                if (parsedIds == null || parsedIds.Count == 0)
                {
                    Console.WriteLine("Error: --sheet-ids format invalid. Expected comma-separated integers");
                    return 1;
                }
                parameters["sheet_ids"] = parsedIds;
            }

            if (outputDir != null)
                parameters["output_dir"] = outputDir;

            if (pdfSetup != null)
                parameters["pdf_setup"] = pdfSetup;

            if (dwgSetup != null)
                parameters["dwg_setup"] = dwgSetup;

            if (allSheets)
                parameters["all_sheets"] = true;

            return await sendCommand("batch_export", parameters);
        }
    }
}
