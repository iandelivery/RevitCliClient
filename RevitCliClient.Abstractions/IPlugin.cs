// SPDX-License-Identifier: MIT
using System.Collections.Generic;

namespace RevitCliClient.Abstractions
{
    public interface IPlugin
    {
        string Name { get; }

        string Version { get; }

        IEnumerable<ICliCommand> GetCommands();
    }
}
