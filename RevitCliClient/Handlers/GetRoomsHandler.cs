// SPDX-License-Identifier: MIT
using RevitCliClient.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public class GetRoomsHandler : ICliCommand
    {
        public string CommandName => "rooms";
        public string Description => "List rooms";
        public string Usage => "rooms [-l <id>]";
        public CommandCategory Category => CommandCategory.Query;
        public string[] Examples => new[] { "RevitCliClient.exe rooms -l 3001" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var levelId = ArgHelper.GetInt(args, "--level-id", "-l");
            var parameters = new Dictionary<string, object>();
            if (levelId is not null) parameters["level_id"] = levelId.Value;
            return await sendCommand("get_rooms", parameters.Count > 0 ? parameters : null);
        }
    }
}
