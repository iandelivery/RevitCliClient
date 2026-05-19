// SPDX-License-Identifier: MIT
using RevitCliClient.Abstractions;
using System;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public class UndoHandler : ICliCommand
    {
        public string CommandName => "undo";
        public string Description => "Undo operations";
        public string Usage => "undo [-st <n>]";
        public CommandCategory Category => CommandCategory.Modify;
        public string[] Examples => new[] { "RevitCliClient.exe undo -st 3" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var steps = ArgHelper.GetInt(args, "--steps", "-st");
            return await sendCommand("undo", new { steps = steps ?? 1 });
        }
    }
}
