// SPDX-License-Identifier: MIT
using RevitCliClient.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public class SetWallConstraintCliHandler : ICliCommand
    {
        public string CommandName => "set_wall_constraint";
        public string Description => "Set wall constraint";
        public string Usage => "set_wall_constraint -w <id> --top-level-id <id> | --base-level-id <id>";
        public CommandCategory Category => CommandCategory.Modify;
        public string[] Examples => new[] { "RevitCliClient.exe set_wall_constraint -w 599906 --top-level-id 196629" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var wallId = ArgHelper.GetInt(args, "--wall-id", "-w");
            var topLevelId = ArgHelper.GetInt(args, "--top-level-id");
            var baseLevelId = ArgHelper.GetInt(args, "--base-level-id");

            if (wallId is null) { Console.WriteLine("Error: --wall-id is required"); return 1; }
            if (topLevelId is null && baseLevelId is null) { Console.WriteLine("Error: at least one of --top-level-id or --base-level-id is required"); return 1; }

            var parameters = new Dictionary<string, object> { ["wall_id"] = wallId.Value };
            if (topLevelId is not null) parameters["top_level_id"] = topLevelId.Value;
            if (baseLevelId is not null) parameters["base_level_id"] = baseLevelId.Value;

            return await sendCommand("set_wall_constraint", parameters);
        }
    }
}
