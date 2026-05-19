// SPDX-License-Identifier: MIT
using RevitCliClient.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public class BatchSetParamHandler : ICliCommand
    {
        public string CommandName => "batch_set_param";
        public string Description => "Batch set parameter";
        public string Usage => "batch_set_param -n <param> -v <val> --ids <id1,id2,...> | -c <cat> | -s";
        public CommandCategory Category => CommandCategory.Modify;
        public string[] Examples => new[] { "RevitCliClient.exe batch_set_param -n \"Comments\" -v \"Approved\" --ids 599906,599841" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var paramName = ArgHelper.FindArg(args, "--name", "-n");
            var value = ArgHelper.FindArg(args, "--value", "-v");
            var ids = ArgHelper.FindArg(args, "--ids");
            var category = ArgHelper.FindArg(args, "--category", "-c");
            var selected = ArgHelper.HasFlag(args, "--selected", "-s");

            if (paramName is null) { Console.WriteLine("Error: --name is required"); return 1; }
            if (value is null) { Console.WriteLine("Error: --value is required"); return 1; }
            if (ids is null && category is null && !selected) { Console.WriteLine("Error: one of --ids, --category, or --selected is required"); return 1; }

            var parameters = new Dictionary<string, object> { ["parameter_name"] = paramName, ["value"] = ArgHelper.TryParseValue(value) };

            if (ids is not null)
            {
                var idList = ArgHelper.ParseIds(ids);
                if (idList is null || idList.Count == 0) { Console.WriteLine("Error: --ids format invalid"); return 1; }
                parameters["element_ids"] = idList;
            }
            if (category is not null) parameters["category"] = category;
            if (selected) parameters["use_selection"] = true;

            return await sendCommand("batch_set_param", parameters);
        }
    }
}
