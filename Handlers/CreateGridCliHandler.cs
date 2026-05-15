using System;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public static class CreateGridCliHandler
    {
        public static async Task<int> HandleAsync(string[] args, Func<string, object?, Task<int>> sendCommand)
        {
            var startX = ArgHelper.GetDouble(args, "--start-x");
            var startY = ArgHelper.GetDouble(args, "--start-y");
            var endX = ArgHelper.GetDouble(args, "--end-x");
            var endY = ArgHelper.GetDouble(args, "--end-y");
            var name = ArgHelper.FindArg(args, "--name", "-n");

            if (startX == null || startY == null || endX == null || endY == null || name == null)
            {
                Console.WriteLine("Error: --start-x, --start-y, --end-x, --end-y, --name are required");
                return 1;
            }

            return await sendCommand("create_grid", new
            {
                start_x = startX.Value,
                start_y = startY.Value,
                end_x = endX.Value,
                end_y = endY.Value,
                name = name
            });
        }
    }
}
