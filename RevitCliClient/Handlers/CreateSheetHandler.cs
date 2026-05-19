// SPDX-License-Identifier: MIT
using RevitCliClient.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public class CreateSheetHandler : ICliCommand
    {
        public string CommandName => "create_sheet";
        public string Description => "Create sheet";
        public string Usage => "create_sheet -n <name> --number <number> [--titleblock-id <id>]";
        public CommandCategory Category => CommandCategory.Create;
        public string[] Examples => new[] { "RevitCliClient.exe create_sheet -n \"A-101\" --number \"A-101\"" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var name = ArgHelper.FindArg(args, "--name", "-n");
            var number = ArgHelper.FindArg(args, "--number");
            var titleblockId = ArgHelper.GetInt(args, "--titleblock-id");

            if (name is null)
            {
                Console.WriteLine("Error: --name is required");
                return 1;
            }

            if (number is null)
            {
                Console.WriteLine("Error: --number is required");
                return 1;
            }

            var parameters = new Dictionary<string, object>
            {
                ["name"] = name,
                ["number"] = number
            };

            if (titleblockId is not null)
                parameters["titleblock_id"] = titleblockId.Value;

            return await sendCommand("create_sheet", parameters);
        }
    }
}
