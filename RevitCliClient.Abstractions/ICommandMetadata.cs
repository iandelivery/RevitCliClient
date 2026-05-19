// SPDX-License-Identifier: MIT
namespace RevitCliClient.Abstractions
{
    public interface ICommandMetadata
    {
        string CommandName { get; }

        string Description { get; }

        string Usage { get; }

        CommandCategory Category { get; }

        string[] Examples { get; }
    }
}
