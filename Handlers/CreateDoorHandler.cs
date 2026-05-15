using System;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public static class CreateDoorHandler
    {
        public static async Task<int> HandleAsync(string[] args, Func<string, object?, Task<int>> sendCommand)
        {
            var wallId = ArgHelper.GetInt(args, "--wall-id", "-w");
            var familyTypeId = ArgHelper.GetInt(args, "--family-type-id");
            var locX = ArgHelper.GetDouble(args, "--location-x");
            var locY = ArgHelper.GetDouble(args, "--location-y");

            if (wallId == null || familyTypeId == null || locX == null || locY == null)
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
