// SPDX-License-Identifier: MIT
using RevitCliClient.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public class ExportViewHandler : ICliCommand
    {
        public string CommandName => "export_view";
        public string Description => "Export view as image";
        public string Usage => "export_view [-o <path>] [--fit-direction <h/v>] [--zoom-type <fit/zoom>] [--resolution <72|150|300|600>] [-t <png|jpeg|bmp|tiff>] [--shadow-file-type <png|jpeg|bmp|tiff>] [--pixel-size <px>] [--export-range <current_view|visible_region>]";
        public CommandCategory Category => CommandCategory.ViewExport;
        public string[] Examples => new[]
        {
            "RevitCliClient.exe export_view -o \"/temp/view.png\"",
            "RevitCliClient.exe export_view -o \"/temp/view.jpg\" -t jpeg --resolution 600",
            "RevitCliClient.exe export_view -o \"/temp/view.png\" --pixel-size 4096 --export-range visible_region"
        };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var output = ArgHelper.FindArg(args, "--output", "-o");
            var fitDirection = ArgHelper.FindArg(args, "--fit-direction");
            var zoomType = ArgHelper.FindArg(args, "--zoom-type");
            var dpi = ArgHelper.FindArg(args, "--dpi");
            var fileType = ArgHelper.FindArg(args, "--file-type", "-t");
            var shadowFileType = ArgHelper.FindArg(args, "--shadow-file-type");
            var exportRange = ArgHelper.FindArg(args, "--export-range");
            var resolution = ArgHelper.GetInt(args, "--resolution");
            var pixelSize = ArgHelper.GetInt(args, "--pixel-size");

            var parameters = new Dictionary<string, object?>();
            if (output is not null) parameters["output_path"] = output;
            if (fitDirection is not null) parameters["fit_direction"] = fitDirection;
            if (zoomType is not null) parameters["zoom_type"] = zoomType;
            if (dpi is not null) parameters["dpi"] = dpi;
            if (fileType is not null) parameters["file_type"] = fileType;
            if (shadowFileType is not null) parameters["shadow_file_type"] = shadowFileType;
            if (exportRange is not null) parameters["export_range"] = exportRange;
            if (resolution is not null) parameters["resolution"] = resolution.Value;
            if (pixelSize is not null) parameters["pixel_size"] = pixelSize.Value;

            return await sendCommand("export_view", parameters.Count > 0 ? parameters : null);
        }
    }
}
