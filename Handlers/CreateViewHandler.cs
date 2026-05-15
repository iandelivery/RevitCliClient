using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public static class CreateViewHandler
    {
        public static async Task<int> HandleAsync(string[] args, Func<string, object?, Task<int>> sendCommand)
        {
            var viewType = ArgHelper.FindArg(args, "--type", "-t");
            var levelId = ArgHelper.GetInt(args, "--level-id", "-l");
            var name = ArgHelper.FindArg(args, "--name", "-n");
            var templateId = ArgHelper.GetInt(args, "--template-id", "-ti");

            if (viewType == null)
            {
                Console.WriteLine("Error: --type is required (plan, ceiling_plan, structural_plan, area_plan, section, 3d)");
                return 1;
            }

            var parameters = new Dictionary<string, object>
            {
                ["type"] = viewType
            };

            if (levelId != null)
                parameters["level_id"] = levelId.Value;

            if (name != null)
                parameters["name"] = name;

            if (templateId != null)
                parameters["template_id"] = templateId.Value;

            return await sendCommand("create_view", parameters);
        }
    }
}
