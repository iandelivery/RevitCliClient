// SPDX-License-Identifier: MIT
using RevitCliClient.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Extensions.Handlers
{
    public class ExampleExtensionHandler : ICliCommand
    {
        public string CommandName => "example_extension";
        public string Description => "Example extension command (plugin demo)";
        public string Usage => "example_extension -i <id> [--option <value>]";
        public CommandCategory Category => CommandCategory.Create;
        public string[] Examples => new[] { "RevitCliClient.exe example_extension -i 599906 --option \"demo\"" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var elementId = ArgHelper.GetInt(args, "--element-id", "-i");
            var option = ArgHelper.FindArg(args, "--option");

            if (elementId == null)
            {
                Console.WriteLine("Error: --element-id is required");
                return 1;
            }

            var parameters = new Dictionary<string, object>
            {
                ["element_id"] = elementId.Value
            };

            if (option != null)
                parameters["option"] = option;

            return await sendCommand("example_extension", parameters);
        }
    }
}
