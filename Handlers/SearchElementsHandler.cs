using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public static class SearchElementsHandler
    {
        public static async Task<int> HandleAsync(string[] args, Func<string, object?, Task<int>> sendCommand)
        {
            var category = ArgHelper.FindArg(args, "--category", "-c");
            var paramName = ArgHelper.FindArg(args, "--param-name");
            var paramValue = ArgHelper.FindArg(args, "--param-value");
            var paramOperator = ArgHelper.FindArg(args, "--param-operator");
            var limit = ArgHelper.GetInt(args, "--limit");

            if (category == null)
            {
                Console.WriteLine("Error: --category is required");
                return 1;
            }

            if (paramName == null)
            {
                Console.WriteLine("Error: --param-name is required");
                return 1;
            }

            if (paramValue == null && paramOperator != "empty")
            {
                Console.WriteLine("Error: --param-value is required (unless --param-operator is 'empty')");
                return 1;
            }

            var parameters = new Dictionary<string, object>
            {
                ["category"] = category,
                ["param_name"] = paramName
            };

            if (paramValue != null)
                parameters["param_value"] = paramValue;

            if (paramOperator != null)
                parameters["param_operator"] = paramOperator;

            if (limit != null)
                parameters["limit"] = limit.Value;

            return await sendCommand("search_elements", parameters);
        }
    }
}
