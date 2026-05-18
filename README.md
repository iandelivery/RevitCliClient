# RevitCliClient

A standalone command-line client that enables AI agents to drive Autodesk Revit through a CLI or HTTP API. This client is open source and communicates with the Revit CLI Bridge server running inside Revit via HTTP, converting CLI commands into Revit API operations.

The Revit CLI Bridge server is a Revit add-in component that is not open source. However, implementing your own compatible bridge is straightforward — each command handler is essentially a thin wrapper around the existing Revit API, making it well-suited for vibe coding. Refer to `BRIDGE_IMPLEMENTATION.md` for the full protocol specification and implementation guide.

**Open Source Client · Standalone · Zero Revit Dependency · Single-File EXE**

## How It Works

```
┌──────────────┐                          ┌──────────────────────────────┐
│              │   POST /api/execute      │                              │
│              │   {command, parameters}  │     Revit CLI Bridge         │
│  CLI Client  │ ───────────────────────► │     (Revit Add-in)           │
│              │                          │                              │
│  or          │   JSON Response          │  Receives HTTP requests,     │
│              │◄───────────────────────  │  executes on Revit main      │
│  AI Agent    │                          │  thread, returns results.    │
│              │   GET /api/task/{id}     │                              │
│              │ ───────────────────────► │  See BRIDGE_IMPLEMENTATION.md│
│              │                          │  for protocol spec.          │
└──────────────┘                          └──────────────────────────────┘
```

The CLI client is the **left side** of this architecture — it sends HTTP requests to the Revit CLI Bridge server and handles responses. The bridge server (right side) is a separate component that runs as a Revit add-in.

## Features

- **46+ Commands** — Query, create, modify, transform, and export Revit elements
- **Async Task Mode** — Long-running commands return a task ID for polling
- **Type-Safe Argument Parsing** — Built-in `ArgHelper` with shortcuts for all parameters
- **Unit Conversion** — Input in millimeters, auto-converted to Revit internal feet
- **Single-File EXE** — Self-contained publish, no runtime installation required
- **Direct HTTP API** — AI agents can also call the REST API directly without the CLI

## Requirements

- **.NET 8.0 SDK** — for building
- **Revit** — with the CLI Bridge add-in installed and running
- **Windows x64** — for the published executable

## Build

```bash
dotnet build -c Release
```

### Publish as Single-File EXE

```powershell
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:PublishAot=false -o ./publish
```

### Publish as Native AOT

```powershell
dotnet publish -c Release -r win-x64 -o ./publish
```

Or use the included script:

```powershell
# Single-file (default)
.\publish.ps1

# Native AOT
.\publish.ps1 --aot
```

| Mode | Startup | Size | Notes |
|------|---------|------|-------|
| Single-File | ~1s | ~30 MB | Self-contained, no runtime needed |
| Native AOT | Instant | ~5-10 MB | Fastest startup, smallest binary |

## Quick Start

1. Start Revit and enable the CLI Bridge (via the Ribbon toggle button)
2. Verify the connection:

```bash
RevitCliClient.exe ping
```

3. Run commands:

```bash
RevitCliClient.exe levels
RevitCliClient.exe elements -c OST_Walls
RevitCliClient.exe create_wall --start-x 0 --start-y 0 --end-x 5000 --end-y 0 -l 3001
```

## Command Reference

### Argument Shortcuts

| Shortcut | Long Form                     | Description                 |
| -------- | ----------------------------- | --------------------------- |
| `-i`     | `--id`                        | Element ID                  |
| `-e`     | `--element-id`                | Element ID                  |
| `-w`     | `--wall-id`                   | Wall ID                     |
| `-l`     | `--level-id`                  | Level ID                    |
| `-n`     | `--name`                      | Name                        |
| `-v`     | `--value`                     | Value                       |
| `-c`     | `--category`                  | Category                    |
| `-f`     | `--family` / `--format`       | Family name / Export format |
| `-s`     | `--symbol` / `--selected`     | Symbol name / Selected mode |
| `-si`    | `--symbol-id`                 | Symbol element ID           |
| `-a`     | `--angle` / `--all-*`         | Angle / All                 |
| `-o`     | `--output` / `--output-dir`   | Output path                 |
| `-t`     | `--type`                      | Type                        |
| `-j`     | `--json`                      | JSON data                   |
| `-fl`    | `--file`                      | File path                   |
| `-vi`    | `--view-id`                   | View ID                     |
| `-vn`    | `--view-name`                 | View name                   |
| `-ti`    | `--template-id` / `--task-id` | Template ID / Task ID       |
| `-st`    | `--steps`                     | Steps                       |

### System

| Command           | Description                           |
| ----------------- | ------------------------------------- |
| `ping`            | Test connection to Revit              |
| `status`          | Server status                         |
| `health`          | Health check                          |
| `task [-ti <id>]` | Query task status (list all if no ID) |

### Document & Query

| Command                                                                  | Description                                |
| ------------------------------------------------------------------------ | ------------------------------------------ |
| `doc_info`                                                               | Get active document info                   |
| `elements [-c <category>]`                                               | List elements                              |
| `element_by_id -i <id>`                                                  | Get element by ID                          |
| `types [--type-name <name>] [-c <cat>]`                                  | List ElementTypes (WallType, FloorType...) |
| `family_symbols [-f <name>] [-c <cat>]`                                  | List FamilySymbols                         |
| `get_family_symbol --instance-id <id> \| -f <name> -s <name> [-c <cat>]` | Get specific FamilySymbol ID               |
| `get_symbol_instances -si <id> [-vi <id>]`                               | Get all instances of a FamilySymbol        |
| `levels`                                                                 | List all levels                            |
| `params -i <id>`                                                         | Get element parameters                     |
| `views [-t <type>] [--template <name>] [--templates-only]`               | List views                                 |
| `sheets`                                                                 | List sheets                                |
| `rooms [-l <id>]`                                                        | List rooms                                 |
| `search_elements -c <cat> --param-name <name> [...]`                     | Search elements by parameter               |

### Create

| Command                                                                                              | Description            |
| ---------------------------------------------------------------------------------------------------- | ---------------------- |
| `create_wall --start-x <x> --start-y <y> --end-x <x> --end-y <y> -l <id> [--height <mm>]`            | Create wall            |
| `create_walls -fl <path> \| -j <json>`                                                               | Batch create walls     |
| `create_door -w <id> --family-type-id <id> --location-x <x> --location-y <y>`                        | Create door            |
| `create_window -w <id> --family-type-id <id> --location-x <x> --location-y <y> [--sill-height <mm>]` | Create window          |
| `create_grid --start-x <x> --start-y <y> --end-x <x> --end-y <y> -n <name>`                          | Create grid            |
| `create_family_instance --symbol-id <id> -l <id> --x <mm> --y <mm> [--z <mm>]`                       | Create family instance |
| `create_view -t <type> -l <id> [-n <name>] [-ti <id>]`                                               | Create view            |
| `create_sheet -n <name> --number <number> [--titleblock-id <id>]`                                    | Create sheet           |
| `create_room -l <id> --x <mm> --y <mm> [-n <name>]`                                                  | Create room            |

### Modify

| Command                                                                                                  | Description                |
| -------------------------------------------------------------------------------------------------------- | -------------------------- |
| `batch -j <json> \| -fl <path> [-n <name>] [--no-rollback] [--no-assimilate]`                            | Execute multiple commands in a TransactionGroup |
| `set_param -i <id> -n <name> -v <value>`                                                                 | Set parameter value        |
| `batch_set_param -n <param> -v <val> --ids <ids> \| -c <cat> \| -s`                                      | Batch set parameter        |
| `set_wall_constraint -w <id> --top-level-id <id> \| --base-level-id <id>`                                | Set wall constraint        |
| `set_walls_constraint --top-level-id <id> \| --base-level-id <id> [-fl <path> \| -j <json>] [-c <cat>]`  | Batch set wall constraints |
| `set_offset -e <id> --base-offset <mm> [--top-offset <mm>]`                                              | Set element offset         |
| `apply_view_template -ti <id> \| --template-name <name> -vi <id> \| --view-ids <ids> \| -t <type> \| -a` | Apply view template        |
| `tag_rooms [-vi <id>] [--room-ids <ids>] [--tag-type-id <id>]`                                           | Tag rooms                  |
| `place_on_sheet -vi <id> --sheet-id <id> [--x <mm>] [--y <mm>]`                                          | Place viewport on sheet    |
| `hide_elements -e <id1,id2,...> [-vi <id>]`                                                               | Hide elements in view      |
| `unhide_elements -e <id1,id2,...> [-vi <id>]`                                                             | Unhide elements in view    |
| `delete -i <id>`                                                                                         | Delete element             |
| `undo [-st <n>]`                                                                                         | Undo operations            |

### Transform

| Command                                                                            | Description    |
| ---------------------------------------------------------------------------------- | -------------- |
| `move_element -e <id> --dx <mm> --dy <mm> [--dz <mm>]`                             | Move element   |
| `copy_element -e <id> --dx <mm> --dy <mm> [--dz <mm>]`                             | Copy element   |
| `rotate_element -e <id> -a <degrees> [--axis-x <x>] [--axis-y <y>] [--axis-z <z>]` | Rotate element |
| `mirror_element -e <id> --normal-x <x> --normal-y <y> --normal-z <z> [...]`        | Mirror element |

### View & Export

| Command                                                                                         | Description          |
| ----------------------------------------------------------------------------------------------- | -------------------- |
| `set_active_view -vi <id> \| -vn <name>`                                                        | Set active view      |
| `zoom_to_fit [-e <id> \| --element-ids <ids>]`                                                  | Zoom to fit          |
| `select_elements [-e <ids>]`                                                                    | Get or set selection |
| `export_view [-o <path>] [--dpi <72\|150\|300\|600>] [--resolution <px>] [-t <type>] [...]` | Export view as image |
| `batch_export -f <pdf\|dwg\|img> --view-ids <ids> \| --sheet-ids <ids> \| -a [-o <path>]`       | Batch export         |

### Raw JSON

```bash
RevitCliClient.exe raw -j '{"command":"ping"}'
```

## Command Details

### batch

Execute multiple CLI commands as a single atomic operation using Revit's `TransactionGroup`. All operations are wrapped in one transaction group, providing:

- **Atomicity** — If any operation fails, the entire group is rolled back (default behavior)
- **Single Undo** — All operations merge into one undo item (`Assimilate` mode)
- **Result References** — Use `$0`, `$1` to reference element IDs from previous operations

**Syntax:**

```bash
RevitCliClient.exe batch -j <json> | -fl <path> [-n <name>] [--no-rollback] [--no-assimilate]
```

**Parameters:**

| Parameter        | Shortcut | Required | Description                                                |
| ---------------- | -------- | -------- | ---------------------------------------------------------- |
| `--json`         | `-j`     | Yes*     | JSON operations (or use `--file`)                          |
| `--file`         | `-fl`    | Yes*     | Path to JSON file containing operations                    |
| `--name`         | `-n`     | No       | Transaction group name (shown in Revit undo history)       |
| `--no-rollback`  | —        | No       | Continue on error instead of rolling back (default: rollback) |
| `--no-assimilate`| —        | No       | Keep separate undo items instead of merging into one       |

**JSON Format (simple array):**

```json
[
  {"command": "create_wall", "parameters": {"level_id": 3001, "start_x": 0, "start_y": 0, "end_x": 5000, "end_y": 0}},
  {"command": "set_param", "parameters": {"element_id": "$0", "param_name": "Comments", "param_value": "New wall"}},
  {"command": "rotate_element", "parameters": {"element_id": "$0", "angle": 45}}
]
```

**JSON Format (with options):**

```json
{
  "name": "Create and configure door",
  "rollback_on_error": true,
  "assimilate": true,
  "operations": [
    {"command": "create_family_instance", "parameters": {"symbol_id": 950367, "level_id": 3001, "x": 2500, "y": 0}},
    {"command": "set_param", "parameters": {"element_id": "$0", "param_name": "Comments", "param_value": "Main entrance"}},
    {"command": "rotate_element", "parameters": {"element_id": "$0", "angle": 90}}
  ]
}
```

**Result References:**

| Syntax     | Meaning                                        |
| ---------- | ---------------------------------------------- |
| `$0`       | `element_id` from operation 0's result         |
| `$1`       | `element_id` from operation 1's result         |
| `$0.data.element_id` | Explicit path to any field in result  |

**Examples:**

```bash
# Create a wall and set its parameter (atomic)
RevitCliClient.exe batch -j '[{"command":"create_wall","parameters":{"level_id":3001,"start_x":0,"start_y":0,"end_x":5000,"end_y":0}},{"command":"set_param","parameters":{"element_id":"$0","param_name":"Comments","param_value":"New wall"}}]'

# Load operations from a file
RevitCliClient.exe batch -fl operations.json -n "Batch creation"

# Continue on error (don't rollback)
RevitCliClient.exe batch -fl ops.json --no-rollback
```

**Expected response (success):**

```json
{
  "status": "success",
  "data": {
    "name": "CLI Batch",
    "total": 3,
    "succeeded": 3,
    "failed": 0,
    "rollback": false,
    "results": [
      {"index": 0, "command": "create_wall", "status": "success", "message": "Wall created.", "data": {"element_id": 600123}},
      {"index": 1, "command": "set_param", "status": "success", "message": "Parameter set.", "data": null},
      {"index": 2, "command": "rotate_element", "status": "success", "message": "Element rotated.", "data": null}
    ]
  }
}
```

**Expected response (rollback on failure):**

```json
{
  "status": "error",
  "message": "Operation 2 ('rotate_element') failed. Transaction group 'CLI Batch' rolled back. All 2 previously committed operations have been undone.",
  "error_details": "{\"name\":\"CLI Batch\",\"rollback\":true,\"failed_at_index\":2,...}"
}
```

### get_symbol_instances

Get all placed instances of a specific FamilySymbol in the document, optionally scoped to a single view for better performance.

**Syntax:**

```bash
RevitCliClient.exe get_symbol_instances -si <symbol_id> [-vi <view_id>]
```

**Parameters:**

| Parameter     | Shortcut | Required | Description                                      |
| ------------- | -------- | -------- | ------------------------------------------------ |
| `--symbol-id` | `-si`    | Yes      | Element ID of the FamilySymbol                   |
| `--view-id`   | `-vi`    | No       | View ID to scope the search (improves performance) |

**Examples:**

```bash
# Get all instances of symbol 950367 in the entire document
RevitCliClient.exe get_symbol_instances -si 950367

# Get instances visible in a specific view (faster for large models)
RevitCliClient.exe get_symbol_instances -si 950367 -vi 3001
```

**Expected response:**

```json
{
  "status": "success",
  "data": {
    "symbol_id": 950367,
    "symbol_name": "900x2100",
    "family_name": "Door",
    "count": 3,
    "instances": [
      { "element_id": 599906, "name": "Door", "category": "Doors", "level_id": 3001 },
      { "element_id": 599841, "name": "Door", "category": "Doors", "level_id": 3002 },
      { "element_id": 599951, "name": "Door", "category": "Doors", "level_id": 3001 }
    ]
  }
}
```

### hide_elements

Hide elements in a view. This is a per-view visibility setting, not a global change. Requires a Transaction.

**Syntax:**

```bash
RevitCliClient.exe hide_elements -e <id1,id2,...> [-vi <view_id>]
```

**Parameters:**

| Parameter      | Shortcut | Required | Description                                |
| -------------- | -------- | -------- | ------------------------------------------ |
| `--element-ids`| `-e`     | Yes      | Comma-separated element IDs to hide        |
| `--view-id`    | `-vi`    | No       | View ID (defaults to active view)          |

**Examples:**

```bash
# Hide elements in the active view
RevitCliClient.exe hide_elements -e 599906,599841

# Hide elements in a specific view
RevitCliClient.exe hide_elements -e 599906,599841 -vi 3001
```

**Expected response:**

```json
{
  "status": "success",
  "data": {
    "view_id": 3001,
    "view_name": "Level 1",
    "action": "hide",
    "count": 2,
    "element_ids": [599906, 599841]
  }
}
```

### unhide_elements

Unhide (make visible again) elements in a view. The elements must currently be hidden in the view. Requires a Transaction.

**Syntax:**

```bash
RevitCliClient.exe unhide_elements -e <id1,id2,...> [-vi <view_id>]
```

**Parameters:**

| Parameter      | Shortcut | Required | Description                                |
| -------------- | -------- | -------- | ------------------------------------------ |
| `--element-ids`| `-e`     | Yes      | Comma-separated element IDs to unhide      |
| `--view-id`    | `-vi`    | No       | View ID (defaults to active view)          |

**Examples:**

```bash
# Unhide elements in the active view
RevitCliClient.exe unhide_elements -e 599906,599841

# Unhide elements in a specific view
RevitCliClient.exe unhide_elements -e 599906,599841 -vi 3001
```

**Expected response:**

```json
{
  "status": "success",
  "data": {
    "view_id": 3001,
    "view_name": "Level 1",
    "action": "unhide",
    "count": 2,
    "element_ids": [599906, 599841]
  }
}
```

## HTTP API

AI agents can call the REST API directly without the CLI client. For the full API specification, see [`BRIDGE_IMPLEMENTATION.md`](BRIDGE_IMPLEMENTATION.md#http-api-contract).

## Project Structure

```
RevitCliClient/
├── Program.cs                        # CLI entry point, command routing
├── HelpText.cs                       # Built-in help documentation
├── RevitCliClient.csproj             # .NET 6.0 project file
├── publish.ps1                       # PowerShell publish script
├── publish.bat                       # Batch publish script
└── Handlers/                         # CLI-side command handlers
    ├── ArgHelper.cs                  # Shared argument parsing utilities
    ├── CreateWallHandler.cs
    ├── CreateWallsHandler.cs
    ├── CreateDoorHandler.cs
    ├── CreateWindowHandler.cs
    ├── CreateGridCliHandler.cs
    ├── CreateFamilyInstanceHandler.cs
    ├── CreateViewHandler.cs
    ├── CreateSheetHandler.cs
    ├── CreateRoomHandler.cs
    ├── SetWallConstraintCliHandler.cs
    ├── SetWallsConstraintCliHandler.cs
    ├── SetParamHandler.cs
    ├── BatchSetParamHandler.cs
    ├── ExportViewHandler.cs
    ├── GetFamilySymbolHandler.cs
    ├── GetFamilySymbolsCliHandler.cs
    ├── GetSymbolInstancesHandler.cs
    ├── SetActiveViewCliHandler.cs
    ├── ZoomToFitHandler.cs
    ├── SelectElementsHandler.cs
    ├── SearchElementsHandler.cs
    ├── GetViewsHandler.cs
    ├── ApplyViewTemplateHandler.cs
    ├── GetSheetsHandler.cs
    ├── GetRoomsHandler.cs
    ├── PlaceOnSheetHandler.cs
    ├── BatchExportHandler.cs
    ├── TagRoomsHandler.cs
    ├── BatchHandler.cs
    ├── HideElementsHandler.cs
    ├── UndoHandler.cs
    └── TransformElementHandlers.cs   # Move, Copy, Rotate, Mirror, SetOffset
```

## Architecture

### Handler Pattern

Each CLI command has a dedicated handler in `Handlers/` that:

1. Parses arguments using `ArgHelper`
2. Constructs the request payload
3. Sends it to the Revit server via `SendCommandAsync`
4. Returns the exit code

```csharp
public static class CreateWallHandler
{
    public static async Task<int> HandleAsync(string[] args, Func<string, object?, Task<int>> send)
    {
        var levelId = ArgHelper.GetInt(args, "--level-id", "-l");
        if (levelId == null) { Console.WriteLine("Error: --level-id is required"); return 1; }

        var startX = ArgHelper.GetDouble(args, "--start-x");
        var startY = ArgHelper.GetDouble(args, "--start-y");
        var endX = ArgHelper.GetDouble(args, "--end-x");
        var endY = ArgHelper.GetDouble(args, "--end-y");

        return await send("create_wall", new {
            level_id = levelId,
            start_x = startX, start_y = startY,
            end_x = endX, end_y = endY
        });
    }
}
```

### ArgHelper — Type-Safe Argument Parsing

`ArgHelper` provides centralized, type-safe argument parsing with multi-alias support:

```csharp
// Find a string argument by any of its aliases
var name = ArgHelper.FindArg(args, "--name", "-n");

// Type-safe integer parsing (returns null on failure)
var id = ArgHelper.GetInt(args, "--id", "-i");

// Type-safe double parsing
var angle = ArgHelper.GetDouble(args, "--angle", "-a");

// Boolean flag check
if (ArgHelper.HasFlag(args, "--all", "-a")) { ... }

// Parse comma-separated IDs
var ids = ArgHelper.ParseIds("599906,599841,599951");
```

### Async Task Polling

When the Revit server returns `{status: "pending", task_id: "..."}`, the CLI client automatically polls for the result:

```
POST /api/execute → {task_id, status: "pending"}
    │
    ▼  (automatic polling every 500ms)
GET /api/task/{task_id} → {status: "running", progress: 50}
    │
GET /api/task/{task_id} → {status: "completed", result: {...}}
```

## Unit Convention

All dimension parameters in CLI commands use **millimeters (mm)**. The server-side handler auto-converts to Revit's internal **feet**:

```
CLI Input: 5000 mm → Server: 5000 × 0.00328084 = 16.404 ft → Revit API
```

## Adding a New Command

1. Create a handler in `Handlers/`:

```csharp
public static class MyCommandHandler
{
    public static async Task<int> HandleAsync(string[] args, Func<string, object?, Task<int>> send)
    {
        var name = ArgHelper.FindArg(args, "--name", "-n");
        return await send("my_command", new { name });
    }
}
```

2. Add a case in `Program.cs`:

```csharp
case "my_command":
    return await MyCommandHandler.HandleAsync(args, SendCommandAsync);
```

3. Update `HelpText.cs` with the command description and example.
4. On the Revit server side, follow [`BRIDGE_IMPLEMENTATION.md`](BRIDGE_IMPLEMENTATION.md#l5--command-handlers) to create the corresponding handler.

## Configuration

| Option         | Default                 | Description              |
| -------------- | ----------------------- | ------------------------ |
| `--url <url>`  | `http://localhost:5000` | Revit CLI server address |
| `--help`, `-h` | —                       | Show help                |

For server-side configuration, see [`BRIDGE_IMPLEMENTATION.md`](BRIDGE_IMPLEMENTATION.md#configuration).

## Dependencies

- **Newtonsoft.Json 13.0.3** — JSON serialization
- **.NET 8.0** — Target framework

No Revit API dependency. The CLI client can be compiled and run independently.

## License

MIT
