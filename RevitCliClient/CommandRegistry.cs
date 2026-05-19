// SPDX-License-Identifier: MIT
using RevitCliClient.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitCliClient
{
    public class CommandRegistry
    {
        private readonly Dictionary<string, ICliCommand> _commands = new(StringComparer.OrdinalIgnoreCase);
        private readonly List<ICliCommand> _registrationOrder = new();

        public void Register(ICliCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            _commands[command.CommandName] = command;
            _registrationOrder.Add(command);
        }

        public bool TryGetCommand(string commandName, out ICliCommand? command)
        {
            return _commands.TryGetValue(commandName, out command);
        }

        public IReadOnlyList<ICliCommand> GetAllCommands()
        {
            return _registrationOrder.AsReadOnly();
        }

        public IEnumerable<IGrouping<CommandCategory, ICliCommand>> GetCommandsByCategory()
        {
            return _registrationOrder.GroupBy(c => c.Category);
        }

        public int Count => _commands.Count;
    }
}
