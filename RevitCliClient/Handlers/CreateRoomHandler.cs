// SPDX-License-Identifier: MIT
using RevitCliClient.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public class CreateRoomHandler : ICliCommand
    {
        public string CommandName => "create_room";
        public string Description => "Create room";
        public string Usage => "create_room -l <id> --x <mm> --y <mm> [-n <name>] [--number <number>]";
        public CommandCategory Category => CommandCategory.Create;
        public string[] Examples => new[] { "RevitCliClient.exe create_room -l 3001 --x 2500 --y 1500 -n \"Living Room\"" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var levelId = ArgHelper.GetInt(args, "--level-id", "-l");
            var x = ArgHelper.GetDouble(args, "--x");
            var y = ArgHelper.GetDouble(args, "--y");
            var name = ArgHelper.FindArg(args, "--name", "-n");
            var number = ArgHelper.FindArg(args, "--number");

            if (levelId is null) { Console.WriteLine("Error: --level-id is required"); return 1; }
            if (x is null || y is null) { Console.WriteLine("Error: --x and --y are required"); return 1; }

            var parameters = new Dictionary<string, object> { ["level_id"] = levelId.Value, ["x"] = x.Value, ["y"] = y.Value };
            if (name is not null) parameters["name"] = name;
            if (number is not null) parameters["number"] = number;
            return await sendCommand("create_room", parameters);
        }
    }
}
