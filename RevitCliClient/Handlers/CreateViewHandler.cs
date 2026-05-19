// SPDX-License-Identifier: MIT
using RevitCliClient.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public class CreateViewHandler : ICliCommand
    {
        public string CommandName => "create_view";
        public string Description => "Create view";
        public string Usage => "create_view -t <plan|ceiling_plan|structural_plan|area_plan|section|3d> -l <id> [-n <name>] [-ti <id>]";
        public CommandCategory Category => CommandCategory.Create;
        public string[] Examples => new[] { "RevitCliClient.exe create_view -t plan -l 3001 -n \"Level 1 Copy\"" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var viewType = ArgHelper.FindArg(args, "--type", "-t");
            var levelId = ArgHelper.GetInt(args, "--level-id", "-l");
            var name = ArgHelper.FindArg(args, "--name", "-n");
            var templateId = ArgHelper.GetInt(args, "--template-id", "-ti");

            if (viewType is null)
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

            if (name is not null)
                parameters["name"] = name;

            if (templateId is not null)
                parameters["template_id"] = templateId.Value;

            return await sendCommand("create_view", parameters);
        }
    }
}
