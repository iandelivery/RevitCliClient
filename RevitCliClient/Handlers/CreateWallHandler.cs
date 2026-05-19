// SPDX-License-Identifier: MIT
using RevitCliClient.Abstractions;
using System;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public class CreateWallHandler : ICliCommand
    {
        public string CommandName => "create_wall";
        public string Description => "Create wall";
        public string Usage => "create_wall --start-x <x> --start-y <y> --end-x <x> --end-y <y> -l <id> [--height <mm>]";
        public CommandCategory Category => CommandCategory.Create;
        public string[] Examples => new[] { "RevitCliClient.exe create_wall --start-x 0 --start-y 0 --end-x 5000 --end-y 0 -l 3001" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var startX = ArgHelper.GetDouble(args, "--start-x");
            var startY = ArgHelper.GetDouble(args, "--start-y");
            var endX = ArgHelper.GetDouble(args, "--end-x");
            var endY = ArgHelper.GetDouble(args, "--end-y");
            var height = ArgHelper.GetDouble(args, "--height");
            var levelId = ArgHelper.GetInt(args, "--level-id", "-l");

            if (startX is null || startY is null || endX is null || endY is null)
            {
                Console.WriteLine("Error: --start-x, --start-y, --end-x, --end-y are required");
                return 1;
            }

            if (levelId is null)
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
