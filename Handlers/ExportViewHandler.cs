using System;
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
            var resolution = ArgHelper.FindArg(args, "--resolution");

            var parameters = new {
                output_path = output,
                fit_direction = fitDirection,
                zoom_type = zoomType,
                resolution = resolution
            };

            return await sendCommand("export_view", parameters);
        }
    }
}
