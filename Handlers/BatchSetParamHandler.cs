using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public static class BatchSetParamHandler
    {
        public static async Task<int> HandleAsync(string[] args, Func<string, object?, Task<int>> sendCommand)
        {
            var paramName = ArgHelper.FindArg(args, "--name", "-n");
            var value = ArgHelper.FindArg(args, "--value", "-v");
            var ids = ArgHelper.FindArg(args, "--ids");
            var category = ArgHelper.FindArg(args, "--category", "-c");
            var selected = ArgHelper.HasFlag(args, "--selected", "-s");

            if (paramName == null)
            {
                Console.WriteLine("Error: --name is required");
                return 1;
            }

            if (value == null)
            {
                Console.WriteLine("Error: --value is required");
                return 1;
            }

            if (ids == null && category == null && !selected)
            {
                Console.WriteLine("Error: one of --ids, --category, or --selected is required");
                return 1;
            }

            var parameters = new Dictionary<string, object>
            {
                ["parameter_name"] = paramName,
                ["value"] = ArgHelper.TryParseValue(value)
            };

            if (ids != null)
            {
                var idList = ArgHelper.ParseIds(ids);
                if (idList == null)
                {
                    Console.WriteLine("Error: --ids format invalid. Expected comma-separated integers (e.g., 100,200,300)");
                    return 1;
                }

                if (idList.Count == 0)
                {
                    Console.WriteLine("Error: --ids list is empty");
                    return 1;
                }

                parameters["element_ids"] = idList;
            }

            if (category != null)
            {
                parameters["category"] = category;
            }

            if (selected)
            {
                parameters["use_selection"] = true;
            }

            return await sendCommand("batch_set_param", parameters);
        }
    }
}
