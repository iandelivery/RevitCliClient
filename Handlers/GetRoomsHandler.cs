using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public static class GetRoomsHandler
    {
        public static async Task<int> HandleAsync(string[] args, Func<string, object?, Task<int>> sendCommand)
        {
            var levelId = ArgHelper.GetInt(args, "--level-id", "-l");

            var parameters = new Dictionary<string, object>();
            if (levelId != null)
                parameters["level_id"] = levelId.Value;

            return await sendCommand("get_rooms", parameters.Count > 0 ? parameters : null);
        }
    }
}
