// SPDX-License-Identifier: MIT
using RevitCliClient.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RevitCliClient
{
    public static class HelpText
    {
        private static readonly (string Short, string Long, string Description)[] ArgShortcuts =
        {
            ("-i",  "--id",                 "Element ID"),
            ("-e",  "--element-id",         "Element ID"),
            ("-w",  "--wall-id",            "Wall ID"),
            ("-l",  "--level-id",           "Level ID"),
            ("-n",  "--name",               "Name"),
            ("-v",  "--value",              "Value"),
            ("-c",  "--category",           "Category"),
            ("-f",  "--family / --format",  "Family name / Export format"),
            ("-s",  "--symbol / --selected","Symbol name / Selected mode"),
            ("-si", "--symbol-id",          "Symbol element ID"),
            ("-a",  "--angle / --all-*",    "Angle / All"),
            ("-o",  "--output / --output-dir","Output path"),
            ("-t",  "--type",               "Type"),
            ("-j",  "--json",               "JSON data"),
            ("-fl", "--file",               "File path"),
            ("-vi", "--view-id",            "View ID"),
            ("-vn", "--view-name",          "View name"),
            ("-ti", "--template-id / --task-id","Template ID / Task ID"),
            ("-st", "--steps",              "Steps"),
        };

        private static readonly CommandCategory[] CategoryOrder =
        {
            CommandCategory.System,
            CommandCategory.Query,
            CommandCategory.Create,
            CommandCategory.Modify,
            CommandCategory.Transform,
            CommandCategory.ViewExport,
            CommandCategory.Raw,
        };

        public static string Generate(CommandRegistry registry)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Revit CLI Client - Command-line tool for AI agents to drive Autodesk Revit");
            sb.AppendLine();
            sb.AppendLine("Usage:");
            sb.AppendLine("  RevitCliClient.exe [--url <url>] <command> [arguments]");
            sb.AppendLine();
            sb.AppendLine("Argument Shortcuts:");
            foreach (var (s, l, d) in ArgShortcuts)
                sb.AppendLine($"  {s,-4} {l,-26} {d}");
            sb.AppendLine();
            sb.AppendLine("Commands:");
            sb.AppendLine();

            var allCommands = registry.GetAllCommands();
            var seenCategories = new HashSet<CommandCategory>();

            foreach (var category in CategoryOrder)
            {
                var commands = allCommands.Where(c => c.Category == category).ToList();
                if (commands.Count == 0) continue;

                sb.AppendLine($"  [{category.GetDisplayName()}]");
                foreach (var cmd in commands)
                    sb.AppendLine($"  {cmd.CommandName,-24} {cmd.Description}");
                sb.AppendLine();
                seenCategories.Add(category);
            }

            foreach (var category in allCommands
                .Select(c => c.Category)
                .Distinct()
                .Where(c => !seenCategories.Contains(c)))
            {
                var commands = allCommands.Where(c => c.Category == category).ToList();
                sb.AppendLine($"  [{category.GetDisplayName()}]");
                foreach (var cmd in commands)
                    sb.AppendLine($"  {cmd.CommandName,-24} {cmd.Description}");
                sb.AppendLine();
            }

            sb.AppendLine("  [HTTP]");
            sb.AppendLine("  status                  Server status (HTTP GET)");
            sb.AppendLine("  health                  Health check (HTTP GET)");
            sb.AppendLine();

            sb.AppendLine("Usage Details:");
            foreach (var cmd in allCommands)
            {
                sb.AppendLine($"  {cmd.CommandName}");
                sb.AppendLine($"    {cmd.Usage}");
            }
            sb.AppendLine();

            sb.AppendLine("Examples:");
            foreach (var cmd in allCommands)
            {
                if (cmd.Examples == null || cmd.Examples.Length == 0) continue;
                foreach (var example in cmd.Examples)
                    sb.AppendLine($"  {example}");
            }
            sb.AppendLine("  RevitCliClient.exe raw -j \"{\\\"command\\\":\\\"ping\\\"}\"");
            sb.AppendLine();

            sb.AppendLine("Options:");
            sb.AppendLine("  --url <url>             Set Revit CLI server address (default: http://localhost:5000)");
            sb.AppendLine("  --help, -h              Show help");

            return sb.ToString();
        }
    }
}
