// SPDX-License-Identifier: MIT
using RevitCliClient.Abstractions;
using System;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public class PingHandler : ICliCommand
    {
        public string CommandName => "ping";
        public string Description => "Test connection to Revit";
        public string Usage => "ping";
        public CommandCategory Category => CommandCategory.System;
        public string[] Examples => new[] { "RevitCliClient.exe ping" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            return await sendCommand("ping", null);
        }
    }

    public class StatusHandler : ICliCommand
    {
        public string CommandName => "status";
        public string Description => "Server status";
        public string Usage => "status";
        public CommandCategory Category => CommandCategory.System;
        public string[] Examples => new[] { "RevitCliClient.exe status" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            return await sendCommand("status", null);
        }
    }

    public class HealthHandler : ICliCommand
    {
        public string CommandName => "health";
        public string Description => "Health check";
        public string Usage => "health";
        public CommandCategory Category => CommandCategory.System;
        public string[] Examples => new[] { "RevitCliClient.exe health" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            return await sendCommand("health", null);
        }
    }

    public class TaskHandler : ICliCommand
    {
        public string CommandName => "task";
        public string Description => "Query task status";
        public string Usage => "task [-ti <id>]";
        public CommandCategory Category => CommandCategory.System;
        public string[] Examples => new[]
        {
            "RevitCliClient.exe task",
            "RevitCliClient.exe task -ti abc123"
        };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var taskId = ArgHelper.FindArg(args, "--task-id", "-ti");
            if (taskId != null)
                return await sendCommand("task", new { task_id = taskId });
            return await sendCommand("task", null);
        }
    }

    public class DocInfoHandler : ICliCommand
    {
        public string CommandName => "doc_info";
        public string Description => "Get active document info";
        public string Usage => "doc_info";
        public CommandCategory Category => CommandCategory.Query;
        public string[] Examples => new[] { "RevitCliClient.exe doc_info" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            return await sendCommand("get_document_info", null);
        }
    }

    public class ElementsHandler : ICliCommand
    {
        public string CommandName => "elements";
        public string Description => "List elements";
        public string Usage => "elements [-c <category>]";
        public CommandCategory Category => CommandCategory.Query;
        public string[] Examples => new[]
        {
            "RevitCliClient.exe elements",
            "RevitCliClient.exe elements -c OST_Walls"
        };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var category = ArgHelper.FindArg(args, "--category", "-c");
            var parameters = category != null ? new { category } : null;
            return await sendCommand("get_elements", parameters);
        }
    }

    public class ElementByIdHandler : ICliCommand
    {
        public string CommandName => "element_by_id";
        public string Description => "Get element by ID";
        public string Usage => "element_by_id -i <id>";
        public CommandCategory Category => CommandCategory.Query;
        public string[] Examples => new[] { "RevitCliClient.exe element_by_id -i 599906" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var id = ArgHelper.GetInt(args, "--id", "-i");
            if (id is null) { Console.WriteLine("Error: --id is required"); return 1; }
            return await sendCommand("get_element_by_id", new { element_id = id.Value });
        }
    }

    public class TypesHandler : ICliCommand
    {
        public string CommandName => "types";
        public string Description => "List ElementTypes (WallType, FloorType, etc.)";
        public string Usage => "types [--type-name <name>] [-c <cat>]";
        public CommandCategory Category => CommandCategory.Query;
        public string[] Examples => new[]
        {
            "RevitCliClient.exe types",
            "RevitCliClient.exe types -c OST_Walls"
        };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var typeName = ArgHelper.FindArg(args, "--type-name");
            var category = ArgHelper.FindArg(args, "--category", "-c");
            var parameters = new System.Collections.Generic.Dictionary<string, object>();
            if (typeName != null) parameters["type_name"] = typeName;
            if (category != null) parameters["category"] = category;
            return await sendCommand("get_element_types", parameters.Count > 0 ? parameters : null);
        }
    }

    public class LevelsHandler : ICliCommand
    {
        public string CommandName => "levels";
        public string Description => "List all levels";
        public string Usage => "levels";
        public CommandCategory Category => CommandCategory.Query;
        public string[] Examples => new[] { "RevitCliClient.exe levels" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            return await sendCommand("get_levels", null);
        }
    }

    public class ParamsHandler : ICliCommand
    {
        public string CommandName => "params";
        public string Description => "Get element parameters";
        public string Usage => "params -i <id>";
        public CommandCategory Category => CommandCategory.Query;
        public string[] Examples => new[] { "RevitCliClient.exe params -i 599906" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var id = ArgHelper.GetInt(args, "--id", "-i");
            if (id is null) { Console.WriteLine("Error: --id is required"); return 1; }
            return await sendCommand("get_parameters", new { element_id = id.Value });
        }
    }

    public class DeleteHandler : ICliCommand
    {
        public string CommandName => "delete";
        public string Description => "Delete element";
        public string Usage => "delete -i <id>";
        public CommandCategory Category => CommandCategory.Modify;
        public string[] Examples => new[] { "RevitCliClient.exe delete -i 599906" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var id = ArgHelper.GetInt(args, "--id", "-i");
            if (id is null) { Console.WriteLine("Error: --id is required"); return 1; }
            return await sendCommand("delete_element", new { element_id = id.Value });
        }
    }

    public class RawHandler : ICliCommand
    {
        public string CommandName => "raw";
        public string Description => "Send raw JSON command";
        public string Usage => "raw -j <json>";
        public CommandCategory Category => CommandCategory.Raw;
        public string[] Examples => new[]
        {
            "RevitCliClient.exe raw -j \"{\\\"command\\\":\\\"ping\\\"}\""
        };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var json = ArgHelper.FindArg(args, "--json", "-j");
            if (json == null)
            {
                Console.WriteLine("Error: --json is required");
                return 1;
            }

            try
            {
                var payload = Newtonsoft.Json.Linq.JObject.Parse(json);
                var command = payload["command"]?.ToString();
                if (command == null)
                {
                    Console.WriteLine("Error: JSON must contain a \"command\" field");
                    return 1;
                }

                var parameters = payload["parameters"];
                return await sendCommand(command, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: Invalid JSON: {ex.Message}");
                return 1;
            }
        }
    }
}
