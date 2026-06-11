// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RevitCliClient
{
    /// <summary>
    /// Handles command execution via SSE stream with legacy polling fallback.
    /// </summary>
    internal class SseClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private string? _lastSseTaskId;

        public SseClient(HttpClient httpClient, string baseUrl)
        {
            _httpClient = httpClient;
            _baseUrl = baseUrl;
        }

        public async Task<int> ExecuteAsync(string command, object? parameters)
        {
            try
            {
                return await ConsumeSseStreamAsync(command, parameters);
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

        private async Task<int> ConsumeSseStreamAsync(string command, object? parameters)
        {
            var payload = new Dictionary<string, object>
            {
                ["command"] = command,
                ["timeout_seconds"] = 120
            };

            if (parameters != null)
                payload["parameters"] = parameters;

            var json = JsonConvert.SerializeObject(payload);
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/api/execute")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/event-stream"));

            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if (response.Content.Headers.ContentType?.MediaType != "text/event-stream")
                return await HandleLegacyResponseAsync(response);

            return await ReadSseStreamAsync(command, response);
        }

        private async Task<int> HandleLegacyResponseAsync(HttpResponseMessage response)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);

            if (responseObj != null
                && responseObj.TryGetValue("status", out var statusObj) && statusObj.ToString() == "pending"
                && responseObj.TryGetValue("task_id", out var taskIdObj))
            {
                var taskId = taskIdObj.ToString()!;
                Console.WriteLine($"Task submitted: {taskId}. Polling for result...");
                return await PollTaskResultAsync(taskId);
            }

            Console.WriteLine(responseContent);
            return response.IsSuccessStatusCode ? 0 : 1;
        }

        private async Task<int> ReadSseStreamAsync(string command, HttpResponseMessage response)
        {
            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);

            string? currentEvent = null;
            int lastProgress = -1;
            var heartbeatTimeout = TimeSpan.FromSeconds(30);

            try
            {
                while (!reader.EndOfStream)
                {
                    var readTask = reader.ReadLineAsync();
                    var timeoutTask = Task.Delay(heartbeatTimeout);
                    var completedFirst = await Task.WhenAny(readTask, timeoutTask);

                    if (completedFirst == timeoutTask)
                    {
                        Console.Error.WriteLine("[SSE] Heartbeat timeout (30s). Falling back to polling...");
                        return await FallbackPollLastTaskAsync(command);
                    }

                    var line = await readTask;

                    if (string.IsNullOrEmpty(line))
                    {
                        currentEvent = null;
                        continue;
                    }

                    if (line.StartsWith("event: "))
                    {
                        currentEvent = line.Substring(7);
                    }
                    else if (line.StartsWith("data: ") && currentEvent != null)
                    {
                        var data = line.Substring(6);
                        var result = HandleSseEvent(currentEvent, data, ref lastProgress);

                        currentEvent = null;

                        if (result != null)
                            return result.Value;
                    }
                }

                Console.Error.WriteLine("[SSE] Connection closed unexpectedly. Falling back to polling...");
                return await FallbackPollLastTaskAsync(command);
            }
            catch (IOException ex)
            {
                Console.Error.WriteLine($"[SSE] Stream I/O error: {ex.Message}. Falling back to polling...");
                return await FallbackPollLastTaskAsync(command);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[SSE] Unexpected error: {ex.GetType().Name}: {ex.Message}. Falling back to polling...");
                return await FallbackPollLastTaskAsync(command);
            }
        }

        /// <summary>
        /// Handles a single SSE event. Returns exit code for terminal events, null to continue.
        /// </summary>
        private int? HandleSseEvent(string eventName, string data, ref int lastProgress)
        {
            switch (eventName)
            {
                case "accepted":
                    var accepted = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
                    if (accepted?.ContainsKey("task_id") == true)
                        _lastSseTaskId = accepted["task_id"]?.ToString();
                    break;

                case "progress":
                    var prog = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
                    if (prog != null)
                    {
                        var pct = prog.ContainsKey("progress") ? Convert.ToInt32(prog["progress"]) : -1;
                        if (pct != lastProgress)
                        {
                            lastProgress = pct;
                            var msg = prog.ContainsKey("message") && prog["message"] != null
                                ? prog["message"]?.ToString() : null;
                            Console.Write($"\r  Progress: {pct}%{(msg != null ? $" - {msg}" : "")}    ");
                        }
                    }
                    break;

                case "completed":
                    var completed = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
                    if (completed?.ContainsKey("result") == true)
                        Console.WriteLine($"\n{completed["result"]}");
                    return 0;

                case "failed":
                    var failed = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
                    if (failed?.ContainsKey("result") == true)
                        Console.Error.WriteLine($"\n{failed["result"]}");
                    return 1;

                case "heartbeat":
                    break;

                default:
                    Console.Error.WriteLine($"[SSE] Unknown event: {eventName}");
                    break;
            }

            return null;
        }

        private async Task<int> FallbackPollLastTaskAsync(string command)
        {
            if (string.IsNullOrEmpty(_lastSseTaskId))
            {
                Console.Error.WriteLine($"[SSE] No task_id for fallback. Command: {command}");
                return 1;
            }

            try
            {
                return await PollTaskResultAsync(_lastSseTaskId);
            }
            catch (HttpRequestException ex)
            {
                Console.Error.WriteLine($"[SSE] Fallback polling failed: {ex.Message}");
                return 1;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[SSE] Fallback polling failed: {ex.GetType().Name}: {ex.Message}");
                return 1;
            }
        }

        public async Task<int> PollTaskResultAsync(string taskId, int maxWaitSeconds = 120, int pollIntervalMs = 500)
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
    }
}
