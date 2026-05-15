using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public static class SetWallConstraintCliHandler
    {
        public static async Task<int> HandleAsync(string[] args, Func<string, object?, Task<int>> sendCommand)
        {
            var wallId = ArgHelper.GetInt(args, "--wall-id", "-w");
            var topLevelId = ArgHelper.GetInt(args, "--top-level-id");
            var baseLevelId = ArgHelper.GetInt(args, "--base-level-id");

            if (wallId == null)
            {
                Console.WriteLine("Error: --wall-id is required");
                return 1;
            }

            if (topLevelId == null && baseLevelId == null)
            {
                Console.WriteLine("Error: at least one of --top-level-id or --base-level-id is required");
                return 1;
            }

            var parameters = new Dictionary<string, object>
            {
                ["wall_id"] = wallId.Value
            };

            if (topLevelId != null)
                parameters["top_level_id"] = topLevelId.Value;

            if (baseLevelId != null)
                parameters["base_level_id"] = baseLevelId.Value;

            return await sendCommand("set_wall_constraint", parameters);
        }
    }
}
