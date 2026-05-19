// SPDX-License-Identifier: MIT
using RevitCliClient.Abstractions;
using System;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public class GetSheetsHandler : ICliCommand
    {
        public string CommandName => "sheets";
        public string Description => "List sheets";
        public string Usage => "sheets";
        public CommandCategory Category => CommandCategory.Query;
        public string[] Examples => new[] { "RevitCliClient.exe sheets" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            return await sendCommand("get_sheets", null);
        }
    }
}
