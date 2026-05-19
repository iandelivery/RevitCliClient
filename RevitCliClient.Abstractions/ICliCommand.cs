// SPDX-License-Identifier: MIT
using System.Threading.Tasks;

namespace RevitCliClient.Abstractions
{
    public interface ICliCommand : ICommandMetadata
    {
        Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand);
    }
}
