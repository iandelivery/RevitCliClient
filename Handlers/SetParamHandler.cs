using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public static class SetParamHandler
    {
        public static async Task<int> HandleAsync(string[] args, Func<string, object?, Task<int>> sendCommand)
        {
            var id = ArgHelper.GetInt(args, "--id", "-i");
            var paramName = ArgHelper.FindArg(args, "--name", "-n");
            var value = ArgHelper.FindArg(args, "--value", "-v");

            if (id == null || paramName == null || value == null)
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
