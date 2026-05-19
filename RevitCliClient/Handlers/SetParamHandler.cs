// SPDX-License-Identifier: MIT
using RevitCliClient.Abstractions;
using System;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public class SetParamHandler : ICliCommand
    {
        public string CommandName => "set_param";
        public string Description => "Set parameter value";
        public string Usage => "set_param -i <id> -n <name> -v <value>";
        public CommandCategory Category => CommandCategory.Modify;
        public string[] Examples => new[] { "RevitCliClient.exe set_param -i 599906 -n \"Comments\" -v \"Test\"" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var id = ArgHelper.GetInt(args, "--id", "-i");
            var paramName = ArgHelper.FindArg(args, "--name", "-n");
            var value = ArgHelper.FindArg(args, "--value", "-v");

            if (id is null || paramName is null || value is null)
            {
                Console.WriteLine("Error: --id, --name, and --value are all required");
                return 1;
            }

            return await sendCommand("set_parameter", new
            {
                element_id = id.Value,
                parameter_name = paramName,
                value = ArgHelper.TryParseValue(value)
            });
        }
    }
}
