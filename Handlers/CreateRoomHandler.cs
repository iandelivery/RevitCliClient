using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public static class CreateRoomHandler
    {
        public static async Task<int> HandleAsync(string[] args, Func<string, object?, Task<int>> sendCommand)
        {
            var levelId = ArgHelper.GetInt(args, "--level-id", "-l");
            var x = ArgHelper.GetDouble(args, "--x");
            var y = ArgHelper.GetDouble(args, "--y");
            var name = ArgHelper.FindArg(args, "--name", "-n");
            var number = ArgHelper.FindArg(args, "--number");

            if (levelId == null)
            {
                Console.WriteLine("Error: --level-id is required");
                return 1;
            }

            if (x == null || y == null)
            {
                Console.WriteLine("Error: --x and --y are required");
                return 1;
            }

            var parameters = new Dictionary<string, object>
            {
                ["level_id"] = levelId.Value,
                ["x"] = x.Value,
                ["y"] = y.Value
            };

            if (name != null)
                parameters["name"] = name;

            if (number != null)
                parameters["number"] = number;

            return await sendCommand("create_room", parameters);
        }
    }
}
