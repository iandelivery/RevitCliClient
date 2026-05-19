// SPDX-License-Identifier: MIT
using RevitCliClient.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public class CreateFamilyInstanceHandler : ICliCommand
    {
        public string CommandName => "create_family_instance";
        public string Description => "Create family instance";
        public string Usage => "create_family_instance --symbol-id <id> -l <id> --x <mm> --y <mm> [--z <mm>] [--structural-type <type>]";
        public CommandCategory Category => CommandCategory.Create;
        public string[] Examples => new[] { "RevitCliClient.exe create_family_instance --symbol-id 950367 -l 3001 --x 2500 --y 0" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var symbolId = ArgHelper.GetInt(args, "--symbol-id");
            var levelId = ArgHelper.GetInt(args, "--level-id", "-l");
            var x = ArgHelper.GetDouble(args, "--x");
            var y = ArgHelper.GetDouble(args, "--y");
            var z = ArgHelper.GetDouble(args, "--z");
            var structuralType = ArgHelper.FindArg(args, "--structural-type");

            if (symbolId is null)
            {
                Console.WriteLine("Error: --symbol-id is required");
                return 1;
            }

            if (levelId is null)
            {
                Console.WriteLine("Error: --level-id is required");
                return 1;
            }

            if (x is null || y is null)
            {
                Console.WriteLine("Error: --x and --y are required");
                return 1;
            }

            var parameters = new Dictionary<string, object>
            {
                ["symbol_id"] = symbolId.Value,
                ["level_id"] = levelId.Value,
                ["x"] = x.Value,
                ["y"] = y.Value
            };

            if (z is not null)
            {
                parameters["z"] = z.Value;
            }

            if (structuralType is not null)
            {
                parameters["structural_type"] = structuralType;
            }

            return await sendCommand("create_family_instance", parameters);
        }
    }
}
