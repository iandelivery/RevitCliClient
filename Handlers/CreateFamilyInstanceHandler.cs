using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public static class CreateFamilyInstanceHandler
    {
        public static async Task<int> HandleAsync(string[] args, Func<string, object?, Task<int>> sendCommand)
        {
            var symbolId = ArgHelper.GetInt(args, "--symbol-id");
            var levelId = ArgHelper.GetInt(args, "--level-id", "-l");
            var x = ArgHelper.GetDouble(args, "--x");
            var y = ArgHelper.GetDouble(args, "--y");
            var z = ArgHelper.GetDouble(args, "--z");
            var structuralType = ArgHelper.FindArg(args, "--structural-type");

            if (symbolId == null)
            {
                Console.WriteLine("Error: --symbol-id is required");
                return 1;
            }

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
                ["symbol_id"] = symbolId.Value,
                ["level_id"] = levelId.Value,
                ["x"] = x.Value,
                ["y"] = y.Value
            };

            if (z != null)
            {
                parameters["z"] = z.Value;
            }

            if (structuralType != null)
            {
                parameters["structural_type"] = structuralType;
            }

            return await sendCommand("create_family_instance", parameters);
        }
    }
}
