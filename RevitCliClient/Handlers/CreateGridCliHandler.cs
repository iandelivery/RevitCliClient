// SPDX-License-Identifier: MIT
using RevitCliClient.Abstractions;
using System;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public class CreateGridCliHandler : ICliCommand
    {
        public string CommandName => "create_grid";
        public string Description => "Create grid";
        public string Usage => "create_grid --start-x <x> --start-y <y> --end-x <x> --end-y <y> -n <name>";
        public CommandCategory Category => CommandCategory.Create;
        public string[] Examples => new[] { "RevitCliClient.exe create_grid --start-x 0 --start-y 0 --end-x 10000 --end-y 0 -n \"Grid-A\"" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var startX = ArgHelper.GetDouble(args, "--start-x");
            var startY = ArgHelper.GetDouble(args, "--start-y");
            var endX = ArgHelper.GetDouble(args, "--end-x");
            var endY = ArgHelper.GetDouble(args, "--end-y");
            var name = ArgHelper.FindArg(args, "--name", "-n");

            if (startX is null || startY is null || endX is null || endY is null || name is null)
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
