// SPDX-License-Identifier: MIT
using RevitCliClient.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public class BatchExportHandler : ICliCommand
    {
        public string CommandName => "batch_export";
        public string Description => "Batch export";
        public string Usage => "batch_export -f <pdf|dwg|img> --view-ids <id1,id2,...> | --sheet-ids <id1,id2,...> | -a [-o <path>] [--pdf-setup <name>] [--dwg-setup <name>]";
        public CommandCategory Category => CommandCategory.ViewExport;
        public string[] Examples => new[] { "RevitCliClient.exe batch_export -f pdf -a -o \"C:\\temp\\export\"" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var format = ArgHelper.FindArg(args, "--format", "-f");
            var viewIds = ArgHelper.FindArg(args, "--view-ids");
            var sheetIds = ArgHelper.FindArg(args, "--sheet-ids");
            var outputDir = ArgHelper.FindArg(args, "--output-dir", "-o");
            var pdfSetup = ArgHelper.FindArg(args, "--pdf-setup");
            var dwgSetup = ArgHelper.FindArg(args, "--dwg-setup");
            var allSheets = ArgHelper.HasFlag(args, "--all-sheets", "-a");

            if (format is null) { Console.WriteLine("Error: --format is required (pdf, dwg, img)"); return 1; }
            if (viewIds is null && sheetIds is null && !allSheets) { Console.WriteLine("Error: one of --view-ids, --sheet-ids, or --all-sheets is required"); return 1; }

            var parameters = new Dictionary<string, object> { ["format"] = format };
            if (viewIds is not null)
            {
                var parsedIds = ArgHelper.ParseIds(viewIds);
                if (parsedIds is null || parsedIds.Count == 0) { Console.WriteLine("Error: --view-ids format invalid"); return 1; }
                parameters["view_ids"] = parsedIds;
            }
            if (sheetIds is not null)
            {
                var parsedIds = ArgHelper.ParseIds(sheetIds);
                if (parsedIds is null || parsedIds.Count == 0) { Console.WriteLine("Error: --sheet-ids format invalid"); return 1; }
                parameters["sheet_ids"] = parsedIds;
            }
            if (outputDir is not null) parameters["output_dir"] = outputDir;
            if (pdfSetup is not null) parameters["pdf_setup"] = pdfSetup;
            if (dwgSetup is not null) parameters["dwg_setup"] = dwgSetup;
            if (allSheets) parameters["all_sheets"] = true;

            return await sendCommand("batch_export", parameters);
        }
    }
}
