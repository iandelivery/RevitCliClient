// SPDX-License-Identifier: MIT
using RevitCliClient.Abstractions;
using RevitCliClient.Extensions.Handlers;
using System.Collections.Generic;

namespace RevitCliClient.Extensions
{
    public class ExtensionsPlugin : IPlugin
    {
        public string Name => "RevitCliClient.Extensions";

        public string Version => "1.0.0";

        public IEnumerable<ICliCommand> GetCommands()
        {
            return new ICliCommand[]
            {
                new ExampleExtensionHandler(),
            };
        }
    }
}
