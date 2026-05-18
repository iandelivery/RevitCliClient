using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using RevitCliClient.Handlers;

namespace RevitCliClient
{
    public static class Program
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static string _baseUrl = "http://localhost:5000";

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

            var command = args[cmdIndex];

            switch (command)
            {
                case "ping":
                    return await SendCommandAsync("ping", null);
                case "status":
                    return await GetAsync("/api/status");
                case "health":
                    return await GetAsync("/api/health");
                case "doc_info":
                    return await SendCommandAsync("get_document_info", null);
                case "elements":
                    var category = ArgHelper.FindArg(args, "--category", "-c");
                    var elementsParams = category != null
                        ? new { category = category }
                        : null;
                    return await SendCommandAsync("get_elements", elementsParams);
                case "element_by_id":
                    var id = ArgHelper.GetInt(args, "--id", "-i");
                    if (id == null) { Console.WriteLine("Error: --id is required"); return 1; }
                    return await SendCommandAsync("get_element_by_id", new { element_id = id.Value });
                case "types":
                    var typeName = ArgHelper.FindArg(args, "--type-name");
                    var typeCategory = ArgHelper.FindArg(args, "--category", "-c");
                    var typesParams = new Dictionary<string, object>();
                    if (typeName != null) typesParams["type_name"] = typeName;
                    if (typeCategory != null) typesParams["category"] = typeCategory;
                    return await SendCommandAsync("get_element_types", typesParams.Count > 0 ? typesParams : null);
                case "family_symbols":
                    return await GetFamilySymbolsCliHandler.HandleAsync(args, SendCommandAsync);
                case "levels":
                    return await SendCommandAsync("get_levels", null);
                case "params":
                    var elemId = ArgHelper.GetInt(args, "--id", "-i");
                    if (elemId == null) { Console.WriteLine("Error: --id is required"); return 1; }
                    return await SendCommandAsync("get_parameters", new { element_id = elemId.Value });
                case "set_param":
                    return await SetParamHandler.HandleAsync(args, SendCommandAsync);
                case "create_wall":
                    return await CreateWallHandler.HandleAsync(args, SendCommandAsync);
                case "create_door":
                    return await CreateDoorHandler.HandleAsync(args, SendCommandAsync);
                case "delete":
                    var delId = ArgHelper.GetInt(args, "--id", "-i");
                    if (delId == null) { Console.WriteLine("Error: --id is required"); return 1; }
                    return await SendCommandAsync("delete_element", new { element_id = delId.Value });
                case "export_view":
                    return await ExportViewHandler.HandleAsync(args, SendCommandAsync);
                case "create_grid":
                    return await CreateGridCliHandler.HandleAsync(args, SendCommandAsync);
                case "create_walls":
                    return await CreateWallsHandler.HandleAsync(args, SendCommandAsync);
                case "set_wall_constraint":
                    return await SetWallConstraintCliHandler.HandleAsync(args, SendCommandAsync);
                case "set_walls_constraint":
                    return await SetWallsConstraintCliHandler.HandleAsync(args, SendCommandAsync);
                case "get_family_symbol":
                    return await GetFamilySymbolHandler.HandleAsync(args, SendCommandAsync);
                case "get_symbol_instances":
                    return await GetSymbolInstancesHandler.HandleAsync(args, SendCommandAsync);
                case "create_family_instance":
                    return await CreateFamilyInstanceHandler.HandleAsync(args, SendCommandAsync);
                case "move_element":
                    return await MoveElementHandler.HandleAsync(args, SendCommandAsync);
                case "copy_element":
                    return await CopyElementHandler.HandleAsync(args, SendCommandAsync);
                case "rotate_element":
                    return await RotateElementHandler.HandleAsync(args, SendCommandAsync);
                case "mirror_element":
                    return await MirrorElementHandler.HandleAsync(args, SendCommandAsync);
                case "set_offset":
                    return await SetOffsetHandler.HandleAsync(args, SendCommandAsync);
                case "set_active_view":
                    return await SetActiveViewCliHandler.HandleAsync(args, SendCommandAsync);
                case "zoom_to_fit":
                    return await ZoomToFitHandler.HandleAsync(args, SendCommandAsync);
                case "select_elements":
                    return await SelectElementsHandler.HandleAsync(args, SendCommandAsync);
                case "batch_set_param":
                    return await BatchSetParamHandler.HandleAsync(args, SendCommandAsync);
                case "search_elements":
                    return await SearchElementsHandler.HandleAsync(args, SendCommandAsync);
                case "views":
                    return await GetViewsHandler.HandleAsync(args, SendCommandAsync);
                case "create_view":
                    return await CreateViewHandler.HandleAsync(args, SendCommandAsync);
                case "apply_view_template":
                    return await ApplyViewTemplateHandler.HandleAsync(args, SendCommandAsync);
                case "create_window":
                    return await CreateWindowHandler.HandleAsync(args, SendCommandAsync);
                case "sheets":
                    return await GetSheetsHandler.HandleAsync(args, SendCommandAsync);
                case "create_sheet":
                    return await CreateSheetHandler.HandleAsync(args, SendCommandAsync);
                case "place_on_sheet":
                    return await PlaceOnSheetHandler.HandleAsync(args, SendCommandAsync);
                case "batch_export":
                    return await BatchExportHandler.HandleAsync(args, SendCommandAsync);
                case "rooms":
                    return await GetRoomsHandler.HandleAsync(args, SendCommandAsync);
                case "create_room":
                    return await CreateRoomHandler.HandleAsync(args, SendCommandAsync);
                case "tag_rooms":
                    return await TagRoomsHandler.HandleAsync(args, SendCommandAsync);
                case "undo":
                    return await UndoHandler.HandleAsync(args, SendCommandAsync);
                case "hide_elements":
                    return await HideElementsHandler.HandleAsync(args, SendCommandAsync, "hide_elements");
                case "unhide_elements":
                    return await HideElementsHandler.HandleAsync(args, SendCommandAsync, "unhide_elements");
                case "task":
                    return await HandleTaskCommandAsync(args);
                case "raw":
                    return await HandleRawJsonAsync(args);
                default:
                    Console.WriteLine($"Unknown command: {command}");
                    PrintHelp();
                    return 1;
            }
        }

        private static async Task<int> HandleRawJsonAsync(string[] args)
        {
            var json = ArgHelper.FindArg(args, "--json", "-j");
            if (json == null)
            {
                Console.WriteLine("Error: --json is required");
                return 1;
            }

            try
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseUrl}/api/execute", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseContent);
                return response.IsSuccessStatusCode ? 0 : 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonConvert.SerializeObject(new { status = "error", message = ex.Message }, Formatting.Indented));
                return 1;
            }
        }

        private static async Task<int> HandleTaskCommandAsync(string[] args)
        {
            var taskId = ArgHelper.FindArg(args, "--task-id", "-ti");

            if (taskId != null)
            {
                return await GetAsync($"/api/task/{taskId}");
            }

            return await GetAsync("/api/task");
        }

        private static async Task<int> SendCommandAsync(string command, object? parameters)
        {
            try
            {
                var payload = new Dictionary<string, object>
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

                var responseObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);

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

                    var taskObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);
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

        private static async Task<int> GetAsync(string path)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}{path}");
                var responseContent = await response.Content.ReadAsStringAsync();
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

        private static void PrintHelp()
        {
            Console.WriteLine(HelpText.Content);
        }
    }
}
