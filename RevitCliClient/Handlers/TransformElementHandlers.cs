// SPDX-License-Identifier: MIT
using RevitCliClient.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public class MoveElementHandler : ICliCommand
    {
        public string CommandName => "move_element";
        public string Description => "Move element";
        public string Usage => "move_element -e <id> --dx <mm> --dy <mm> [--dz <mm>]";
        public CommandCategory Category => CommandCategory.Transform;
        public string[] Examples => new[] { "RevitCliClient.exe move_element -e 599906 --dx 1000 --dy 0" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var elementId = ArgHelper.GetInt(args, "--element-id", "-e");
            var dx = ArgHelper.GetDouble(args, "--dx");
            var dy = ArgHelper.GetDouble(args, "--dy");
            var dz = ArgHelper.GetDouble(args, "--dz");

            if (elementId is null) { Console.WriteLine("Error: --element-id is required"); return 1; }
            if (dx is null || dy is null) { Console.WriteLine("Error: --dx and --dy are required"); return 1; }

            var parameters = new Dictionary<string, object> { ["element_id"] = elementId.Value, ["dx"] = dx.Value, ["dy"] = dy.Value };
            if (dz is not null) parameters["dz"] = dz.Value;
            return await sendCommand("move_element", parameters);
        }
    }

    public class CopyElementHandler : ICliCommand
    {
        public string CommandName => "copy_element";
        public string Description => "Copy element";
        public string Usage => "copy_element -e <id> --dx <mm> --dy <mm> [--dz <mm>]";
        public CommandCategory Category => CommandCategory.Transform;
        public string[] Examples => new[] { "RevitCliClient.exe copy_element -e 599906 --dx 1000 --dy 0" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var elementId = ArgHelper.GetInt(args, "--element-id", "-e");
            var dx = ArgHelper.GetDouble(args, "--dx");
            var dy = ArgHelper.GetDouble(args, "--dy");
            var dz = ArgHelper.GetDouble(args, "--dz");

            if (elementId is null) { Console.WriteLine("Error: --element-id is required"); return 1; }
            if (dx is null || dy is null) { Console.WriteLine("Error: --dx and --dy are required"); return 1; }

            var parameters = new Dictionary<string, object> { ["element_id"] = elementId.Value, ["dx"] = dx.Value, ["dy"] = dy.Value };
            if (dz is not null) parameters["dz"] = dz.Value;
            return await sendCommand("copy_element", parameters);
        }
    }

    public class RotateElementHandler : ICliCommand
    {
        public string CommandName => "rotate_element";
        public string Description => "Rotate element";
        public string Usage => "rotate_element -e <id> -a <degrees> [--axis-x <x>] [--axis-y <y>] [--axis-z <z>]";
        public CommandCategory Category => CommandCategory.Transform;
        public string[] Examples => new[] { "RevitCliClient.exe rotate_element -e 599906 -a 45" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var elementId = ArgHelper.GetInt(args, "--element-id", "-e");
            var axisX = ArgHelper.GetDouble(args, "--axis-x");
            var axisY = ArgHelper.GetDouble(args, "--axis-y");
            var axisZ = ArgHelper.GetDouble(args, "--axis-z");
            var angle = ArgHelper.GetDouble(args, "--angle", "-a");

            if (elementId is null) { Console.WriteLine("Error: --element-id is required"); return 1; }
            if (angle is null) { Console.WriteLine("Error: --angle is required"); return 1; }

            var parameters = new Dictionary<string, object> { ["element_id"] = elementId.Value, ["angle"] = angle.Value };
            if (axisX is not null) parameters["axis_x"] = axisX.Value;
            if (axisY is not null) parameters["axis_y"] = axisY.Value;
            if (axisZ is not null) parameters["axis_z"] = axisZ.Value;
            return await sendCommand("rotate_element", parameters);
        }
    }

    public class MirrorElementHandler : ICliCommand
    {
        public string CommandName => "mirror_element";
        public string Description => "Mirror element";
        public string Usage => "mirror_element -e <id> --normal-x <x> --normal-y <y> --normal-z <z> [--origin-x <x>] [--origin-y <y>] [--origin-z <z>]";
        public CommandCategory Category => CommandCategory.Transform;
        public string[] Examples => new[] { "RevitCliClient.exe mirror_element -e 599906 --normal-x 1 --normal-y 0" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var elementId = ArgHelper.GetInt(args, "--element-id", "-e");
            var originX = ArgHelper.GetDouble(args, "--origin-x");
            var originY = ArgHelper.GetDouble(args, "--origin-y");
            var originZ = ArgHelper.GetDouble(args, "--origin-z");
            var normalX = ArgHelper.GetDouble(args, "--normal-x");
            var normalY = ArgHelper.GetDouble(args, "--normal-y");
            var normalZ = ArgHelper.GetDouble(args, "--normal-z");

            if (elementId is null) { Console.WriteLine("Error: --element-id is required"); return 1; }
            if (normalX is null && normalY is null && normalZ is null) { Console.WriteLine("Error: mirror plane normal is required"); return 1; }

            var parameters = new Dictionary<string, object> { ["element_id"] = elementId.Value };
            if (originX is not null) parameters["origin_x"] = originX.Value;
            if (originY is not null) parameters["origin_y"] = originY.Value;
            if (originZ is not null) parameters["origin_z"] = originZ.Value;
            if (normalX is not null) parameters["normal_x"] = normalX.Value;
            if (normalY is not null) parameters["normal_y"] = normalY.Value;
            if (normalZ is not null) parameters["normal_z"] = normalZ.Value;
            return await sendCommand("mirror_element", parameters);
        }
    }

    public class SetOffsetHandler : ICliCommand
    {
        public string CommandName => "set_offset";
        public string Description => "Set element offset";
        public string Usage => "set_offset -e <id> --base-offset <mm> [--top-offset <mm>]";
        public CommandCategory Category => CommandCategory.Modify;
        public string[] Examples => new[] { "RevitCliClient.exe set_offset -e 599906 --base-offset 500" };

        public async Task<int> HandleAsync(string[] args, SendCommandFunc sendCommand)
        {
            var elementId = ArgHelper.GetInt(args, "--element-id", "-e");
            var baseOffset = ArgHelper.GetDouble(args, "--base-offset");
            var topOffset = ArgHelper.GetDouble(args, "--top-offset");

            if (elementId is null) { Console.WriteLine("Error: --element-id is required"); return 1; }
            if (baseOffset is null && topOffset is null) { Console.WriteLine("Error: at least one of --base-offset or --top-offset is required"); return 1; }

            var parameters = new Dictionary<string, object> { ["element_id"] = elementId.Value };
            if (baseOffset is not null) parameters["base_offset"] = baseOffset.Value;
            if (topOffset is not null) parameters["top_offset"] = topOffset.Value;
            return await sendCommand("set_offset", parameters);
        }
    }
}
