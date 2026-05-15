using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public static class TagRoomsHandler
    {
        public static async Task<int> HandleAsync(string[] args, Func<string, object?, Task<int>> sendCommand)
        {
            var viewId = ArgHelper.GetInt(args, "--view-id", "-vi");
            var roomIds = ArgHelper.FindArg(args, "--room-ids", "-r");
            var tagTypeId = ArgHelper.GetInt(args, "--tag-type-id");

            var parameters = new Dictionary<string, object>();

            if (viewId != null)
                parameters["view_id"] = viewId.Value;

            if (roomIds != null)
            {
                var parsedIds = ArgHelper.ParseIds(roomIds);
                if (parsedIds == null || parsedIds.Count == 0)
                {
                    Console.WriteLine("Error: --room-ids format invalid. Expected comma-separated integers");
                    return 1;
                }
                parameters["room_ids"] = parsedIds;
            }

            if (tagTypeId != null)
                parameters["tag_type_id"] = tagTypeId.Value;

            return await sendCommand("tag_rooms", parameters.Count > 0 ? parameters : null);
        }
    }
}
