using System;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public static class GetSheetsHandler
    {
        public static async Task<int> HandleAsync(string[] args, Func<string, object?, Task<int>> sendCommand)
        {
            return await sendCommand("get_sheets", null);
        }
    }
}
