// SPDX-License-Identifier: MIT
using RevitCliClient.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public class TagRoomsHandler : ICliCommand
    {
        public string CommandName => "tag_rooms";
        public string Description => "Tag rooms";
        public string Usage => "tag_rooms [-vi <id>] [--room-ids <id1,id2,...>] [--tag-type-id <id>]";
        public CommandCategory Category => CommandCategory.Modify;
        public string[] Examples => new[] { "RevitCliClient.exe tag_rooms -vi 3001" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var viewId = ArgHelper.GetInt(args, "--view-id", "-vi");
            var roomIds = ArgHelper.FindArg(args, "--room-ids", "-r");
            var tagTypeId = ArgHelper.GetInt(args, "--tag-type-id");

            var parameters = new Dictionary<string, object>();
            if (viewId is not null) parameters["view_id"] = viewId.Value;
            if (roomIds is not null)
            {
                var parsedIds = ArgHelper.ParseIds(roomIds);
                if (parsedIds is null || parsedIds.Count == 0) { Console.WriteLine("Error: --room-ids format invalid"); return 1; }
                parameters["room_ids"] = parsedIds;
            }
            if (tagTypeId is not null) parameters["tag_type_id"] = tagTypeId.Value;



            return await sendCommand("tag_rooms", parameters.Count > 0 ? parameters : null);
        }
    }
}
