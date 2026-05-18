namespace RevitCliClient
{
    public static class HelpText
    {
        public const string Content = @"
Revit CLI Client - Command-line tool for AI agents to drive Autodesk Revit

Usage:
  RevitCliClient.exe <command> [arguments]

Argument Shortcuts:
  -i  --id                 Element ID
  -e  --element-id         Element ID
  -w  --wall-id            Wall ID
  -l  --level-id           Level ID
  -n  --name               Name
  -v  --value              Value
  -c  --category           Category
  -f  --family / --format  Family name / Export format
  -s  --symbol / --selected  Symbol name / Selected mode
  -si --symbol-id           Symbol element ID
  -a  --angle / --all-*    Angle / All
  -o  --output / --output-dir  Output path
  -t  --type               Type
  -j  --json               JSON data
  -fl --file               File path
  -vi --view-id            View ID
  -vn --view-name          View name
  -ti --template-id / --task-id  Template ID / Task ID
  -st --steps              Steps

Commands:

  [System]
  ping                    Test connection to Revit
  status                  Server status
  health                  Health check
  task [-ti <id>]          Query task status (list all if no ID)

  [Document & Query]
  doc_info                Get active document info
  elements [-c <category>]  List elements
  element_by_id -i <id>   Get element by ID
  types [--type-name <name>] [-c <cat>]  List ElementTypes (WallType, FloorType, etc.)
  family_symbols [-f <name>] [-c <cat>]  List FamilySymbols
  get_family_symbol --instance-id <id> | -f <name> -s <name> [-c <cat>]  Get specific FamilySymbol ID
  get_symbol_instances -si <id> [-vi <id>]  Get all instances of a FamilySymbol
  levels                  List all levels
  params -i <id>          Get element parameters
  views [-t <type>] [--template <name>] [--templates-only]  List views
  sheets                   List sheets
  rooms [-l <id>]          List rooms
  search_elements -c <cat> --param-name <name> [--param-value <val>] [--param-operator <eq|neq|gt|lt|contains|empty|notempty>] [--limit <n>]  Search elements by parameter

  [Create]
  create_wall --start-x <x> --start-y <y> --end-x <x> --end-y <y> -l <id> [--height <mm>]
  create_walls -fl <path> | -j <json>  Batch create walls
  create_door -w <id> --family-type-id <id> --location-x <x> --location-y <y>
  create_window -w <id> --family-type-id <id> --location-x <x> --location-y <y> [--sill-height <mm>]
  create_grid --start-x <x> --start-y <y> --end-x <x> --end-y <y> -n <name>  Create grid
  create_family_instance --symbol-id <id> -l <id> --x <mm> --y <mm> [--z <mm>] [--structural-type <type>]  Create family instance
  create_view -t <plan|ceiling_plan|structural_plan|area_plan|section|3d> -l <id> [-n <name>] [-ti <id>]  Create view
  create_sheet -n <name> --number <number> [--titleblock-id <id>]  Create sheet
  create_room -l <id> --x <mm> --y <mm> [-n <name>] [--number <number>]  Create room

  [Modify]
  batch -j <json> | -fl <path> [-n <name>] [--no-rollback] [--no-assimilate]  Execute multiple commands in a TransactionGroup
  set_param -i <id> -n <name> -v <value>  Set parameter value
  batch_set_param -n <param> -v <val> --ids <id1,id2,...> | -c <cat> | -s  Batch set parameter
  set_wall_constraint -w <id> --top-level-id <id> | --base-level-id <id>  Set wall constraint
  set_walls_constraint --top-level-id <id> | --base-level-id <id> [-fl <path> | -j <json>] [-c <cat>]  Batch set wall constraints
  set_offset -e <id> --base-offset <mm> [--top-offset <mm>]  Set element offset
  apply_view_template -ti <id> | --template-name <name> -vi <id> | --view-ids <id1,id2,...> | -t <type> | -a  Apply view template
  tag_rooms [-vi <id>] [--room-ids <id1,id2,...>] [--tag-type-id <id>]  Tag rooms
  place_on_sheet -vi <id> --sheet-id <id> [--x <mm>] [--y <mm>]  Place viewport on sheet
  hide_elements -e <id1,id2,...> [-vi <id>]  Hide elements in view
  unhide_elements -e <id1,id2,...> [-vi <id>]  Unhide elements in view
  delete -i <id>          Delete element
  undo [-st <n>]            Undo operations

  [Transform]
  move_element -e <id> --dx <mm> --dy <mm> [--dz <mm>]  Move element
  copy_element -e <id> --dx <mm> --dy <mm> [--dz <mm>]  Copy element
  rotate_element -e <id> -a <degrees> [--axis-x <x>] [--axis-y <y>] [--axis-z <z>]  Rotate element
  mirror_element -e <id> --normal-x <x> --normal-y <y> --normal-z <z> [--origin-x <x>] [--origin-y <y>] [--origin-z <z>]  Mirror element

  [View & Export]
  set_active_view -vi <id> | -vn <name>  Set active view
  zoom_to_fit [-e <id> | --element-ids <id1,id2,...>]  Zoom to fit
  select_elements [-e <id1,id2,...>]  Get or set selection
  export_view [-o <path>] [--fit-direction <h/v>] [--zoom-type <fit/zoom>] [--resolution <dpi>]
  batch_export -f <pdf|dwg|img> --view-ids <id1,id2,...> | --sheet-ids <id1,id2,...> | -a [-o <path>] [--pdf-setup <name>] [--dwg-setup <name>]  Batch export

  [Raw]
  raw -j <json>            Send raw JSON command

Examples:
  RevitCliClient.exe ping
  RevitCliClient.exe elements -c OST_Walls
  RevitCliClient.exe element_by_id -i 599906
  RevitCliClient.exe params -i 599906
  RevitCliClient.exe set_param -i 599906 -n ""Comments"" -v ""Test""
  RevitCliClient.exe create_wall --start-x 0 --start-y 0 --end-x 5000 --end-y 0 -l 3001
  RevitCliClient.exe create_door -w 599906 --family-type-id 950367 --location-x 2500 --location-y 0
  RevitCliClient.exe create_grid --start-x 0 --start-y 0 --end-x 10000 --end-y 0 -n ""Grid-A""
  RevitCliClient.exe create_walls -j ""[{\""start_x\"":0,\""start_y\"":0,\""end_x\"":5000,\""end_y\"":0,\""level_id\"":3001}]""
  RevitCliClient.exe create_family_instance --symbol-id 950367 -l 3001 --x 2500 --y 0
  RevitCliClient.exe get_symbol_instances -si 950367
  RevitCliClient.exe get_symbol_instances -si 950367 -vi 3001
  RevitCliClient.exe delete -i 599906
  RevitCliClient.exe set_wall_constraint -w 599906 --top-level-id 196629
  RevitCliClient.exe move_element -e 599906 --dx 1000 --dy 0
  RevitCliClient.exe rotate_element -e 599906 -a 45
  RevitCliClient.exe set_active_view -vn ""Floor Plan: Level 1""
  RevitCliClient.exe set_active_view -vi 3001
  RevitCliClient.exe select_elements -e 599906,599841
  RevitCliClient.exe batch_set_param -n ""Comments"" -v ""Approved"" --ids 599906,599841
  RevitCliClient.exe search_elements -c OST_Walls --param-name ""Comments"" --param-value ""demolish""
  RevitCliClient.exe export_view -o ""C:\temp\view.png""
  RevitCliClient.exe hide_elements -e 599906,599841
  RevitCliClient.exe unhide_elements -e 599906,599841 -vi 3001
  RevitCliClient.exe undo -st 3
  RevitCliClient.exe batch -j ""[{\""command\"":\""create_wall\"",\""parameters\"":{\""level_id\"":3001,\""start_x\"":0,\""start_y\"":0,\""end_x\"":5000,\""end_y\"":0}},{\""command\"":\""set_param\"",\""parameters\"":{\""element_id\"":\""$0\"",\""param_name\"":\""Comments\"",\""param_value\"":\""New wall\""}}]""
  RevitCliClient.exe raw -j ""{\""command\"":\""ping\""}""

Options:
  --url <url>             Set Revit CLI server address (default: http://localhost:5000)
  --help, -h              Show help
";
    }
}
