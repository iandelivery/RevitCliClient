# CLI Bridge Server Implementation Guide

This document describes the architecture and protocol that a compatible CLI Bridge server must implement to work with RevitCliClient. It is intended as a **reference specification** — you can implement your own server in any language or framework, as long as it conforms to the HTTP API contract described below.

## The Core Problem

Revit's API is strictly single-threaded and bound to the Revit UI process. External processes (like AI agents or CLI tools) cannot call the Revit API directly. A **bridge** is needed to:

1. Receive HTTP requests from external processes
2. Forward them to the Revit main thread for execution
3. Return the results back to the caller

```
┌──────────────┐     HTTP      ┌───────────────┐    IPC    ┌──────────────────┐
│  CLI Client  │ ─────────────►│  HTTP Server  │ ─────────►│  Revit Main      │
│  AI Agent    │               │  (Bridge)     │           │  Thread          │
│              │◄──────────────│               │◄──────────│  (API Execution) │
└──────────────┘    JSON       └───────────────┘   Result  └──────────────────┘
```

## Architecture Overview

A CLI Bridge server consists of **6 layers**:

```
┌─────────────────────────────────────────────────┐
│  L1  HTTP Server        — Receive requests      │
│  L2  Task Registry      — Async-to-sync bridge  │
│  L3  Event Handler      — Revit main thread     │
│  L4  Command Router     — Dispatch to handlers  │
│  L5  Command Handlers   — Individual logic      │
│  L6  Failure Processor  — Auto-handle dialogs   │
└─────────────────────────────────────────────────┘
```

### L1 — HTTP Server

A lightweight HTTP server listening on `localhost`, providing REST API endpoints.

**Required endpoints:**

| Method | Path | Function |
|--------|------|----------|
| POST | `/api/execute` | Execute a command (sync or async) |
| GET | `/api/task/{task_id}` | Query task status and result |
| GET | `/api/task` | List all tasks (latest 50) |
| GET | `/api/status` | Server status |
| GET | `/api/health` | Health check |

**Implementation note:** Use `System.Net.HttpListener` (.NET Framework) or any lightweight HTTP library. Bind to `http://localhost:{port}/` only — never expose to external networks.

### L2 — Task Registry

The central coordination mechanism between HTTP threads and the Revit main thread.

**Three key components:**

| Component | Purpose |
|-----------|---------|
| `ConcurrentQueue<QueuedCommand>` | FIFO queue for incoming commands |
| `ConcurrentDictionary<string, TaskInfo>` | Track all tasks with lifecycle state |
| `TaskCompletionSource<string>` | Suspend HTTP response until Revit completes |

**Task lifecycle:**

```
pending → running → completed
                  → failed
                  → timeout
```

**Example implementation:**

```csharp
public class TaskInfo
{
    public string TaskId { get; set; }
    public string Command { get; set; }
    public string Status { get; set; }  // pending, running, completed, failed, timeout
    public int Progress { get; set; }
    public string? ProgressMessage { get; set; }
    public string? ResultJson { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    [JsonIgnore]
    public TaskCompletionSource<string> Tcs { get; set; } = new();
}

public static class TaskRegistry
{
    public static ConcurrentDictionary<string, TaskInfo> Tasks { get; } = new();
    public static ConcurrentQueue<QueuedCommand> CommandQueue { get; } = new();
    public static ExternalEvent? RevitEvent { get; set; }

    public static TaskInfo CreateTask(string taskId, string command)
    {
        var info = new TaskInfo { TaskId = taskId, Command = command, Status = "pending" };
        Tasks[taskId] = info;
        return info;
    }

    public static void SetRunning(string taskId) { /* update status */ }
    public static void SetProgress(string taskId, int pct, string? msg = null) { /* update progress */ }
    public static void SetCompleted(string taskId, string resultJson) { /* update + signal Tcs */ }
    public static void SetFailed(string taskId, string errorJson) { /* update + signal Tcs */ }
}
```

### L3 — External Event Handler

This is the **only** safe way to execute code on the Revit main thread from a background thread.

**Implementation pattern:**

```csharp
public class CliCommandHandler : IExternalEventHandler
{
    public string GetName() => "CliCommandHandler";

    public void Execute(UIApplication app)
    {
        while (TaskRegistry.CommandQueue.TryDequeue(out var cmd))
        {
            TaskRegistry.SetRunning(cmd.TaskId);
            try
            {
                var result = CommandRouter.Execute(app, cmd);
                TaskRegistry.SetCompleted(cmd.TaskId, result);
            }
            catch (Exception ex)
            {
                TaskRegistry.SetFailed(cmd.TaskId, SerializeError(ex));
            }
        }
    }
}
```

**Key points:**
- The `while` loop drains the entire queue in one wake-up cycle
- `try/catch` ensures failed commands still release the pending `TaskCompletionSource`
- Register this handler as an `ExternalEvent` during add-in startup

### L4 — Command Router

A registry pattern that maps command names to handler functions.

```csharp
public static class CommandRouter
{
    private static readonly Dictionary<string, Func<UIApplication, QueuedCommand, string>> _handlers = new();

    static CommandRouter()
    {
        Register("ping", PingHandler.Handle);
        Register("get_levels", GetLevelsHandler.Handle);
        Register("create_wall", CreateWallHandler.Handle);
        // ... register all commands
    }

    public static void Register(string name, Func<UIApplication, QueuedCommand, string> handler)
    {
        _handlers[name] = handler;
    }

    public static string Execute(UIApplication app, QueuedCommand cmd)
    {
        if (_handlers.TryGetValue(cmd.Command, out var handler))
            return handler(app, cmd);
        return CommandResponse.Error(cmd.TaskId, $"Unknown command: {cmd.Command}").ToJson();
    }
}
```

### L5 — Command Handlers

Each command has a dedicated handler. Handlers follow a consistent pattern:

```csharp
public static class CreateWallHandler
{
    public static string Handle(UIApplication app, QueuedCommand cmd)
    {
        var doc = app.ActiveUIDocument?.Document;
        if (doc == null)
            return CommandResponse.Error(cmd.TaskId, "No active document.").ToJson();

        var parameters = cmd.Parameters as Dictionary<string, object>
            ?? new Dictionary<string, object>();

        // Extract parameters
        var levelId = Convert.ToInt32(parameters["level_id"]);
        var startX = Convert.ToDouble(parameters["start_x"]);
        // ...

        using (var t = new Transaction(doc, "CLI: Create Wall"))
        {
            t.Start();
            var options = t.GetFailureHandlingOptions();
            options.SetFailuresPreprocessor(new CliFailurePreprocessor());
            t.SetFailureHandlingOptions(options);

            // Revit API calls
            var level = doc.GetElement(new ElementId(levelId)) as Level;
            var start = new XYZ(startX.MmToFeet(), startY.MmToFeet(), 0);
            var end = new XYZ(endX.MmToFeet(), endY.MmToFeet(), 0);
            var line = Line.CreateBound(start, end);
            var wall = Wall.Create(doc, line, level.Id, false);

            t.Commit();
        }

        return CommandResponse.Success(cmd.TaskId, result, "Wall created.").ToJson();
    }
}
```

### L6 — Failure Preprocessor

**Critical for AI agents.** Without this, Revit will pop up modal dialogs that freeze the entire workflow.

```csharp
public class CliFailurePreprocessor : IFailuresPreprocessor
{
    public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
    {
        var failures = failuresAccessor.GetFailureMessages().ToList();

        foreach (var failure in failures)
        {
            var severity = failure.GetSeverity();

            if (severity == FailureSeverity.Warning)
            {
                failuresAccessor.DeleteWarning(failure);
            }
            else if (severity == FailureSeverity.Error)
            {
                failure.ResolveFailure(failuresAccessor);
            }
        }

        return FailureProcessingResult.ProceedWithCommit;
    }
}
```

**Always attach this to every Transaction:**

```csharp
var options = t.GetFailureHandlingOptions();
options.SetFailuresPreprocessor(new CliFailurePreprocessor());
t.SetFailureHandlingOptions(options);
```

## HTTP API Contract

This is the protocol that the CLI Bridge server **must** implement to be compatible with RevitCliClient.

### POST /api/execute

**Request body:**

```json
{
    "task_id": "optional-uuid",
    "command": "create_wall",
    "parameters": {
        "level_id": 3001,
        "start_x": 0,
        "start_y": 0,
        "end_x": 5000,
        "end_y": 0
    },
    "timeout_seconds": 120,
    "async": false
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `command` | `string` | Yes | Command name |
| `parameters` | `object` | No | Command-specific parameters |
| `timeout_seconds` | `int` | No | Default: 120 |
| `task_id` | `string` | No | Auto-generated if omitted |
| `async` | `bool` | No | Default: false |

**Sync mode response (default):**

The HTTP connection is held until the command completes or times out.

```json
{
    "task_id": "abc-123",
    "status": "success",
    "message": "Wall created.",
    "data": { "element_id": 599906 }
}
```

**Async mode response:**

Immediately returns a task ID. The client polls `/api/task/{id}` for the result.

```json
{
    "task_id": "abc-123",
    "status": "pending"
}
```

**Error response:**

```json
{
    "task_id": "abc-123",
    "status": "error",
    "message": "Command failed: ...",
    "error_details": "System.Exception: ..."
}
```

### GET /api/task/{task_id}

Returns the current state of a task.

```json
{
    "task_id": "abc-123",
    "command": "create_walls",
    "status": "running",
    "progress": 50,
    "progress_message": "Creating wall 50/100...",
    "started_at": "2024-01-15T14:30:45+08:00",
    "completed_at": null
}
```

When completed:

```json
{
    "task_id": "abc-123",
    "command": "create_walls",
    "status": "completed",
    "progress": 100,
    "started_at": "2024-01-15T14:30:45+08:00",
    "completed_at": "2024-01-15T14:30:48+08:00",
    "result": {
        "task_id": "abc-123",
        "status": "success",
        "data": { "results": [...] }
    }
}
```

### GET /api/task

List all tasks (latest 50). Returns an array of task objects.

### GET /api/status

```json
{
    "status": "running",
    "port": 5000,
    "pending_tasks": 0,
    "total_tasks": 42
}
```

### GET /api/health

```json
{
    "status": "healthy"
}
```

## Sync vs Async Execution Flow

### Sync Mode

```
HTTP POST /api/execute
    │
    ▼
1. Parse JSON → command + parameters
    │
    ▼
2. Create TaskInfo, enqueue to CommandQueue
    │
    ▼
3. ExternalEvent.Raise() — wake up Revit main thread
    │
    ▼
4. await Task.WhenAny(taskInfo.Tcs.Task, Task.Delay(timeout))
    │              │                          │
    │         Timeout                   Revit completed
    │         SetFailed()               SetCompleted()
    │              │                          │
    ▼◀─────────────┴──────────────────────────┘
5. Return JSON response
```

### Async Mode

```
HTTP POST /api/execute {async: true}
    │
    ▼
1-3. Same as sync
    │
    ▼
4. Immediately return {task_id, status: "pending"}
    │
    │    ┌──────────────────────────────────────┐
    │    │  GET /api/task/{task_id} (polling)   │
    │    │  → {status: "running", progress: 50} │
    │    │  → {status: "completed", result: …}  │
    │    └──────────────────────────────────────┘
```

## Thread Model

```
┌─────────────────────────────────────────────────────┐
│                    Revit Process                    │
│                                                     │
│  ┌─────────────┐    ┌──────────────────────────┐    │
│  │ HTTP Server │    │   Revit Main Thread      │    │
│  │ (background)│    │   (UI Thread)            │    │
│  │             │    │                          │    │
│  │ Receive     │───►│ IExternalEventHandler    │    │
│  │ Create Task │    │   .Execute()             │    │
│  │ Enqueue     │    │  CommandRouter.Execute() │    │
│  │ await TCS   │◄───│  SetCompleted/SetFailed  │    │
│  │ Return      │    │                          │    │
│  └─────────────┘    └──────────────────────────┘    │
└─────────────────────────────────────────────────────┘
```

**Critical rules:**
- Revit API **must only** be called on the main thread
- `ExternalEvent.Raise()` is the only safe way to wake the main thread from a background thread
- `IExternalEventHandler.Execute()` is called automatically during the next UI idle
- All `Transaction` objects must be created and committed within `Execute()`

## Data Models

### CommandResponse

All handlers must return a JSON string following this format:

**Success:**

```json
{
    "task_id": "abc-123",
    "status": "success",
    "message": "Description of what happened.",
    "data": { }
}
```

**Error:**

```json
{
    "task_id": "abc-123",
    "status": "error",
    "message": "Human-readable error description.",
    "error_details": "Exception stack trace or technical details"
}
```

### QueuedCommand

Internal model representing a command waiting to be executed:

```csharp
public class QueuedCommand
{
    public string TaskId { get; set; }
    public string Command { get; set; }
    public object? Parameters { get; set; }
}
```

## Unit Convention

All dimension parameters in CLI commands use **millimeters (mm)**. The server-side handler must convert to Revit's internal **feet**:

```csharp
// mm → feet (1 ft = 304.8 mm)
public static double MillimeterToFeet(this double mm) => mm / 304.8;

// feet → mm
public static double FeetToMillimeter(this double feet) => feet * 304.8;
```

## Command Naming Convention

| Pattern | Prefix | Examples |
|---------|--------|----------|
| Query | `get_` | `get_elements`, `get_levels` |
| Create | `create_` | `create_wall`, `create_grid` |
| Set | `set_` | `set_parameter`, `set_wall_constraint` |
| Transform | `verb_element` | `move_element`, `rotate_element` |
| Batch | `batch_` or plural | `batch_set_param`, `create_walls` |
| Other | direct verb | `delete_element`, `undo` |

## Add-in Integration

### Startup

```csharp
public class MyApp : IExternalApplication
{
    public Result OnStartup(UIControlledApplication application)
    {
        var handler = new CliCommandHandler();
        var externalEvent = ExternalEvent.Create(handler);
        TaskRegistry.RevitEvent = externalEvent;

        var server = new CliHttpServer(port: 5000);
        server.Start();
        return Result.Succeeded;
    }

    public Result OnShutdown(UIControlledApplication application)
    {
        server.Stop();
        return Result.Succeeded;
    }
}
```

### Toggle Command

Provide a Revit Ribbon button to toggle the bridge on/off:

```csharp
public class ToggleBridgeCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        bool newState = BridgeStateManager.Toggle();
        TaskDialog.Show("CLI Bridge", $"Bridge is now {(newState ? "ON" : "OFF")}");
        return Result.Succeeded;
    }
}
```

## Configuration

```json
{
    "enabled": true,
    "port": 5000,
    "timeout_seconds": 120,
    "max_command_queue_size": 100
}
```

## Safety Checklist

| Risk | Protection |
|------|-----------|
| Revit dialog freeze | `IFailuresPreprocessor` auto-handles warnings and errors |
| Deadlock | `Task.WhenAny` with timeout (default 120s) |
| Edit mode conflict | Check `doc.IsModifiable` before transactions |
| Invalid parameters | Type-safe parsing + handler validation |
| External network exposure | Listen on `localhost` only |
| Task memory leak | Periodic cleanup of completed tasks (e.g., after 5 min) |

## Minimum Viable Server

To implement a compatible server, you need at minimum:

1. **HTTP server** with `/api/execute` and `/api/task/{id}` endpoints
2. **Command queue** (thread-safe FIFO)
3. **ExternalEvent + IExternalEventHandler** to run commands on the Revit main thread
4. **TaskCompletionSource** to bridge async HTTP → sync Revit execution
5. **IFailuresPreprocessor** to prevent dialog freezes
6. **CommandResponse** JSON format for all results

Everything else (command handlers, routing, state management, progress reporting) can be customized to your needs.
