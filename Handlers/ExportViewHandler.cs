using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public static class ExportViewHandler
    {
        public static async Task<int> HandleAsync(string[] args, Func<string, object?, Task<int>> sendCommand)
        {
            var output = ArgHelper.FindArg(args, "--output", "-o");
            var fitDirection = ArgHelper.FindArg(args, "--fit-direction");
            var zoomType = ArgHelper.FindArg(args, "--zoom-type");
            var dpi = ArgHelper.FindArg(args, "--dpi");
            var fileType = ArgHelper.FindArg(args, "--file-type", "-t");
            var shadowFileType = ArgHelper.FindArg(args, "--shadow-file-type");
            var exportRange = ArgHelper.FindArg(args, "--export-range");
            var resolution = ArgHelper.GetInt(args, "--resolution");

            var parameters = new Dictionary<string, object?>();

            if (output != null) parameters["output_path"] = output;
            if (fitDirection != null) parameters["fit_direction"] = fitDirection;
            if (zoomType != null) parameters["zoom_type"] = zoomType;
            if (dpi != null) parameters["dpi"] = dpi;
            if (fileType != null) parameters["file_type"] = fileType;
            if (shadowFileType != null) parameters["shadow_file_type"] = shadowFileType;
            if (exportRange != null) parameters["export_range"] = exportRange;
            if (resolution != null) parameters["resolution"] = resolution.Value;

            return await sendCommand("export_view", parameters.Count > 0 ? parameters : null);
        }
    }
}
