using System;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public static class UndoHandler
    {
        public static async Task<int> HandleAsync(string[] args, Func<string, object?, Task<int>> sendCommand)
        {
            var steps = ArgHelper.GetInt(args, "--steps", "-st");

            var parameters = new { steps = steps ?? 1 };

            return await sendCommand("undo", parameters);
        }
    }
}
