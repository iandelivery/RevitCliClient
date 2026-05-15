using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public static class CreateSheetHandler
    {
        public static async Task<int> HandleAsync(string[] args, Func<string, object?, Task<int>> sendCommand)
        {
            var name = ArgHelper.FindArg(args, "--name", "-n");
            var number = ArgHelper.FindArg(args, "--number");
            var titleblockId = ArgHelper.GetInt(args, "--titleblock-id");

            if (name == null)
            {
                Console.WriteLine("Error: --name is required");
                return 1;
            }

            if (number == null)
            {
                Console.WriteLine("Error: --number is required");
                return 1;
            }

            var parameters = new Dictionary<string, object>
            {
                ["name"] = name,
                ["number"] = number
            };

            if (titleblockId != null)
                parameters["titleblock_id"] = titleblockId.Value;

            return await sendCommand("create_sheet", parameters);
        }
    }
}
