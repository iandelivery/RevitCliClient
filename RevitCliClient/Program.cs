// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using RevitCliClient.Abstractions;
using RevitCliClient.Extensions;
using RevitCliClient.Handlers;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RevitCliClient
{
    public static class Program
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static string _baseUrl = "http://localhost:5000";
        private static CommandRegistry? _registry;

        public static async Task<int> Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintHelp();
                return 0;
            }

            if (args[0] == "--help" || args[0] == "-h")
            {
                PrintHelp();
                return 0;
            }

            var cmdIndex = 0;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--url" && i + 1 < args.Length)
                {
                    _baseUrl = args[i + 1].TrimEnd('/');
                    cmdIndex = i + 2;
                    break;
                }
            }

            if (cmdIndex >= args.Length)
            {
                PrintHelp();
                return 0;
            }

            _registry = CreateRegistry();

            var command = args[cmdIndex];

            if (_registry.TryGetCommand(command, out var cmd) && cmd != null)
            {
                return await cmd.HandleAsync(args, SendCommandAsync);
            }

            Console.WriteLine($"Unknown command: {command}");
            PrintHelp();
            return 1;
        }

        private static void RegisterBuiltInPlugins(CommandRegistry registry)
        {
            var builtInPlugin = new ExtensionsPlugin();
            foreach (var command in builtInPlugin.GetCommands())
            {
                registry.Register(command);
            }
        }

        private static CommandRegistry CreateRegistry()
        {
            var registry = new CommandRegistry();

            registry.Register(new PingHandler());
            registry.Register(new StatusHandler());
            registry.Register(new HealthHandler());
            registry.Register(new TaskHandler());
            registry.Register(new DocInfoHandler());
            registry.Register(new ElementsHandler());
            registry.Register(new ElementByIdHandler());
            registry.Register(new TypesHandler());
            registry.Register(new LevelsHandler());
            registry.Register(new ParamsHandler());
            registry.Register(new DeleteHandler());
            registry.Register(new CreateWallHandler());
            registry.Register(new CreateWallsHandler());
            registry.Register(new CreateDoorHandler());
            registry.Register(new CreateWindowHandler());
            registry.Register(new CreateGridCliHandler());
            registry.Register(new CreateFamilyInstanceHandler());
            registry.Register(new CreateViewHandler());
            registry.Register(new CreateSheetHandler());
            registry.Register(new CreateRoomHandler());
            registry.Register(new SetParamHandler());
            registry.Register(new BatchSetParamHandler());
            registry.Register(new SetWallConstraintCliHandler());
            registry.Register(new SetWallsConstraintCliHandler());
            registry.Register(new ApplyViewTemplateHandler());
            registry.Register(new TagRoomsHandler());
            registry.Register(new PlaceOnSheetHandler());
            registry.Register(new HideElementsHandler("hide_elements"));
            registry.Register(new HideElementsHandler("unhide_elements"));
            registry.Register(new UndoHandler());
            registry.Register(new BatchHandler());
            registry.Register(new MoveElementHandler());
            registry.Register(new CopyElementHandler());
            registry.Register(new RotateElementHandler());
            registry.Register(new MirrorElementHandler());
            registry.Register(new SetOffsetHandler());
            registry.Register(new SearchElementsHandler());
            registry.Register(new GetViewsHandler());
            registry.Register(new GetSheetsHandler());
            registry.Register(new GetRoomsHandler());
            registry.Register(new GetFamilySymbolHandler());
            registry.Register(new GetFamilySymbolsCliHandler());
            registry.Register(new GetSymbolInstancesHandler());
            registry.Register(new SetActiveViewCliHandler());
            registry.Register(new ZoomToFitHandler());
            registry.Register(new SelectElementsHandler());
            registry.Register(new ExportViewHandler());
            registry.Register(new BatchExportHandler());
            registry.Register(new RawHandler());

            var pluginDir = Path.Combine(AppContext.BaseDirectory, "plugins");

            RegisterBuiltInPlugins(registry);

            PluginLoader.LoadPlugins(registry, pluginDir);

            return registry;
        }

        private static async Task<int> SendCommandAsync(string command, object? parameters)
        {
            try
            {
                var payload = new System.Collections.Generic.Dictionary<string, object>
                {
                    ["command"] = command,
                    ["timeout_seconds"] = 120
                };

                if (parameters != null)
                    payload["parameters"] = parameters;

                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/api/execute", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                var responseObj = JsonConvert.DeserializeObject<System.Collections.Generic.Dictionary<string, object>>(responseContent);

                if (responseObj != null && responseObj.TryGetValue("status", out var statusObj)
                    && statusObj.ToString() == "pending" && responseObj.TryGetValue("task_id", out var taskIdObj))
                {
                    var taskId = taskIdObj.ToString()!;
                    Console.WriteLine($"Task submitted: {taskId}. Polling for result...");
                    return await PollTaskResultAsync(taskId);
                }

                Console.WriteLine(responseContent);
                return response.IsSuccessStatusCode ? 0 : 1;
            }
            catch (HttpRequestException ex)
            {
                var error = new
                {
                    status = "error",
                    message = $"Cannot connect to Revit CLI server at {_baseUrl}",
                    detail = ex.Message
                };
                Console.WriteLine(JsonConvert.SerializeObject(error, Formatting.Indented));
                return 1;
            }
        }

        private static async Task<int> PollTaskResultAsync(string taskId, int maxWaitSeconds = 120, int pollIntervalMs = 500)
        {
            var startTime = DateTime.Now;

            while ((DateTime.Now - startTime).TotalSeconds < maxWaitSeconds)
            {
                await Task.Delay(pollIntervalMs);

                try
                {
                    var response = await _httpClient.GetAsync($"{_baseUrl}/api/task/{taskId}");
                    var responseContent = await response.Content.ReadAsStringAsync();

                    var taskObj = JsonConvert.DeserializeObject<System.Collections.Generic.Dictionary<string, object>>(responseContent);
                    if (taskObj == null) continue;

                    var status = taskObj["status"]?.ToString();

                    if (status == "completed" || status == "failed" || status == "timeout")
                    {
                        if (taskObj.TryGetValue("result", out var resultObj))
                        {
                            Console.WriteLine(resultObj.ToString());
                        }
                        else
                        {
                            Console.WriteLine(responseContent);
                        }
                        return status == "completed" ? 0 : 1;
                    }

                    if (taskObj.TryGetValue("progress", out var progress) && progress != null)
                    {
                        var pct = Convert.ToInt32(progress);
                        if (pct > 0)
                        {
                            var msg = taskObj.TryGetValue("progress_message", out var pm) ? pm?.ToString() : null;
                            Console.Write($"\r  Progress: {pct}%{(msg != null ? $" - {msg}" : "")}    ");
                        }
                    }
                }
                catch (HttpRequestException)
                {
                    continue;
                }
            }

            Console.WriteLine($"\nTask {taskId} timed out after {maxWaitSeconds} seconds.");
            return 1;
        }

        private static void PrintHelp()
        {
            if (_registry != null)
            {
                Console.WriteLine(HelpText.Generate(_registry));
            }
            else
            {
                var registry = CreateRegistry();
                Console.WriteLine(HelpText.Generate(registry));
            }
        }
    }
}
