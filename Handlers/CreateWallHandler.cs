using System;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public static class CreateWallHandler
    {
        public static async Task<int> HandleAsync(string[] args, Func<string, object?, Task<int>> sendCommand)
        {
            var startX = ArgHelper.GetDouble(args, "--start-x");
            var startY = ArgHelper.GetDouble(args, "--start-y");
            var endX = ArgHelper.GetDouble(args, "--end-x");
            var endY = ArgHelper.GetDouble(args, "--end-y");
            var height = ArgHelper.GetDouble(args, "--height");
            var levelId = ArgHelper.GetInt(args, "--level-id", "-l");

            if (startX == null || startY == null || endX == null || endY == null)
            {
                Console.WriteLine("Error: --start-x, --start-y, --end-x, --end-y are required");
                return 1;
            }

            if (levelId == null)
            {
                Console.WriteLine("Error: --level-id is required");
                return 1;
            }

            return await sendCommand("create_wall", new
            {
                start_x = startX.Value,
                start_y = startY.Value,
                end_x = endX.Value,
                end_y = endY.Value,
                level_id = levelId.Value,
                height = height ?? 3000.0
            });
        }
    }
}
