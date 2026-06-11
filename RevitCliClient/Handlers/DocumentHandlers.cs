using RevitCliClient.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public class DocOpenHandler : ICliCommand
    {
        public string CommandName => "doc_open";
        public string Description => "Open a Revit document";
        public string Usage => "doc_open -p <path> [--detach] [--audit]";
        public CommandCategory Category => CommandCategory.Document;
        public string[] Examples => new[]
        {
            "RevitCliClient.exe doc_open -p \"C:\\Projects\\Building.rvt\"",
            "RevitCliClient.exe doc_open -p \"C:\\Projects\\Building.rvt\" --detach --audit"
        };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var path = ArgHelper.FindArg(args, "--path", "-p");
            if (path is null) { System.Console.WriteLine("Error: --path is required"); return 1; }

            var detach = ArgHelper.HasFlag(args, "--detach");
            var audit = ArgHelper.HasFlag(args, "--audit");

            var parameters = new Dictionary<string, object> { ["path"] = path };
            if (detach) parameters["detach"] = true;
            if (audit) parameters["audit"] = true;

            return await sendCommand("doc_open", parameters);
        }
    }

    public class DocCloseHandler : ICliCommand
    {
        public string CommandName => "doc_close";
        public string Description => "Close document(s)";
        public string Usage => "doc_close [--save] [--all]";
        public CommandCategory Category => CommandCategory.Document;
        public string[] Examples => new[]
        {
            "RevitCliClient.exe doc_close",
            "RevitCliClient.exe doc_close --save",
            "RevitCliClient.exe doc_close --all"
        };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var save = ArgHelper.HasFlag(args, "--save", "-s");
            var all = ArgHelper.HasFlag(args, "--all", "-a");

            var parameters = new Dictionary<string, object>();
            if (save) parameters["save"] = true;
            if (all) parameters["all"] = true;

            return await sendCommand("doc_close", parameters.Count > 0 ? parameters : null);
        }
    }

    public class DocSaveHandler : ICliCommand
    {
        public string CommandName => "doc_save";
        public string Description => "Save the active document";
        public string Usage => "doc_save [--compact] [--preview-view-id <id>]";
        public CommandCategory Category => CommandCategory.Document;
        public string[] Examples => new[]
        {
            "RevitCliClient.exe doc_save",
            "RevitCliClient.exe doc_save --compact",
            "RevitCliClient.exe doc_save --preview-view-id 1234"
        };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var compact = ArgHelper.HasFlag(args, "--compact");
            var previewViewId = ArgHelper.GetInt(args, "--preview-view-id");

            var parameters = new Dictionary<string, object>();
            if (compact) parameters["compact"] = true;
            if (previewViewId is not null) parameters["preview_view_id"] = previewViewId.Value;

            return await sendCommand("doc_save", parameters.Count > 0 ? parameters : null);
        }
    }

    public class DocSaveAsHandler : ICliCommand
    {
        public string CommandName => "doc_save_as";
        public string Description => "Save the active document to a new file";
        public string Usage => "doc_save_as -p <path> [--overwrite] [--compact] [--save-as-central] [--preview-view-id <id>]";
        public CommandCategory Category => CommandCategory.Document;
        public string[] Examples => new[]
        {
            "RevitCliClient.exe doc_save_as -p \"C:\\Projects\\Building_v2.rvt\"",
            "RevitCliClient.exe doc_save_as -p \"C:\\Projects\\Building_v2.rvt\" --overwrite --compact",
            "RevitCliClient.exe doc_save_as -p \"C:\\Projects\\Central.rvt\" --save-as-central"
        };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var path = ArgHelper.FindArg(args, "--path", "-p");
            if (path is null) { System.Console.WriteLine("Error: --path is required"); return 1; }

            var overwrite = ArgHelper.HasFlag(args, "--overwrite");
            var compact = ArgHelper.HasFlag(args, "--compact");
            var saveAsCentral = ArgHelper.HasFlag(args, "--save-as-central");
            var previewViewId = ArgHelper.GetInt(args, "--preview-view-id");

            var parameters = new Dictionary<string, object> { ["path"] = path };
            if (overwrite) parameters["overwrite"] = true;
            if (compact) parameters["compact"] = true;
            if (saveAsCentral) parameters["save_as_central"] = true;
            if (previewViewId is not null) parameters["preview_view_id"] = previewViewId.Value;

            return await sendCommand("doc_save_as", parameters);
        }
    }

    public class DocSyncHandler : ICliCommand
    {
        public string CommandName => "doc_sync";
        public string Description => "Synchronize with central model";
        public string Usage => "doc_sync [-m <comment>] [--relinquish] [--no-save-local] [--no-wait]";
        public CommandCategory Category => CommandCategory.Document;
        public string[] Examples => new[]
        {
            "RevitCliClient.exe doc_sync",
            "RevitCliClient.exe doc_sync -m \"自动同步\" --relinquish",
            "RevitCliClient.exe doc_sync --no-wait --no-save-local"
        };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var comment = ArgHelper.FindArg(args, "--comment", "-m");
            var relinquish = ArgHelper.HasFlag(args, "--relinquish");
            var noSaveLocal = ArgHelper.HasFlag(args, "--no-save-local");
            var noWait = ArgHelper.HasFlag(args, "--no-wait");

            var parameters = new Dictionary<string, object>();
            if (comment is not null) parameters["comment"] = comment;
            if (relinquish) parameters["relinquish"] = true;
            if (noSaveLocal) parameters["save_local"] = false;
            if (noWait) parameters["wait_for_lock"] = false;

            return await sendCommand("doc_sync", parameters.Count > 0 ? parameters : null);
        }
    }

    public class DocListHandler : ICliCommand
    {
        public string CommandName => "doc_list";
        public string Description => "List all open documents";
        public string Usage => "doc_list";
        public CommandCategory Category => CommandCategory.Document;
        public string[] Examples => new[] { "RevitCliClient.exe doc_list" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            return await sendCommand("doc_list", null);
        }
    }
}
