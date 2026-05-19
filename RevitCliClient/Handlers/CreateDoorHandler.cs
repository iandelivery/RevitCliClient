// SPDX-License-Identifier: MIT
using RevitCliClient.Abstractions;
using System;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public class CreateDoorHandler : ICliCommand
    {
        public string CommandName => "create_door";
        public string Description => "Create door on wall";
        public string Usage => "create_door -w <id> --family-type-id <id> --location-x <x> --location-y <y>";
        public CommandCategory Category => CommandCategory.Create;
        public string[] Examples => new[] { "RevitCliClient.exe create_door -w 599906 --family-type-id 950367 --location-x 2500 --location-y 0" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var wallId = ArgHelper.GetInt(args, "--wall-id", "-w");
            var familyTypeId = ArgHelper.GetInt(args, "--family-type-id");
            var locX = ArgHelper.GetDouble(args, "--location-x");
            var locY = ArgHelper.GetDouble(args, "--location-y");

            if (wallId is null || familyTypeId is null || locX is null || locY is null)
            {
                Console.WriteLine("Error: --wall-id, --family-type-id, --location-x, --location-y are required");
                return 1;
            }

            return await sendCommand("create_door", new
            {
                wall_id = wallId.Value,
                family_type_id = familyTypeId.Value,
                location_x = locX.Value,
                location_y = locY.Value
            });
        }
    }
}
