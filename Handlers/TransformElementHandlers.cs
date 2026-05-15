using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevitCliClient.Handlers
{
    public static class MoveElementHandler
    {
        public static async Task<int> HandleAsync(string[] args, Func<string, object?, Task<int>> sendCommand)
        {
            var elementId = ArgHelper.GetInt(args, "--element-id", "-e");
            var dx = ArgHelper.GetDouble(args, "--dx");
            var dy = ArgHelper.GetDouble(args, "--dy");
            var dz = ArgHelper.GetDouble(args, "--dz");

            if (elementId == null)
            {
                Console.WriteLine("Error: --element-id is required");
                return 1;
            }

            if (dx == null || dy == null)
            {
                Console.WriteLine("Error: --dx and --dy are required");
                return 1;
            }

            var parameters = new Dictionary<string, object>
            {
                ["element_id"] = elementId.Value,
                ["dx"] = dx.Value,
                ["dy"] = dy.Value
            };

            if (dz != null)
            {
                parameters["dz"] = dz.Value;
            }

            return await sendCommand("move_element", parameters);
        }
    }

    public static class CopyElementHandler
    {
        public static async Task<int> HandleAsync(string[] args, Func<string, object?, Task<int>> sendCommand)
        {
            var elementId = ArgHelper.GetInt(args, "--element-id", "-e");
            var dx = ArgHelper.GetDouble(args, "--dx");
            var dy = ArgHelper.GetDouble(args, "--dy");
            var dz = ArgHelper.GetDouble(args, "--dz");

            if (elementId == null)
            {
                Console.WriteLine("Error: --element-id is required");
                return 1;
            }

            if (dx == null || dy == null)
            {
                Console.WriteLine("Error: --dx and --dy are required");
                return 1;
            }

            var parameters = new Dictionary<string, object>
            {
                ["element_id"] = elementId.Value,
                ["dx"] = dx.Value,
                ["dy"] = dy.Value
            };

            if (dz != null)
            {
                parameters["dz"] = dz.Value;
            }

            return await sendCommand("copy_element", parameters);
        }
    }

    public static class RotateElementHandler
    {
        public static async Task<int> HandleAsync(string[] args, Func<string, object?, Task<int>> sendCommand)
        {
            var elementId = ArgHelper.GetInt(args, "--element-id", "-e");
            var axisX = ArgHelper.GetDouble(args, "--axis-x");
            var axisY = ArgHelper.GetDouble(args, "--axis-y");
            var axisZ = ArgHelper.GetDouble(args, "--axis-z");
            var angle = ArgHelper.GetDouble(args, "--angle", "-a");

            if (elementId == null)
            {
                Console.WriteLine("Error: --element-id is required");
                return 1;
            }

            if (angle == null)
            {
                Console.WriteLine("Error: --angle is required");
                return 1;
            }

            var parameters = new Dictionary<string, object>
            {
                ["element_id"] = elementId.Value,
                ["angle"] = angle.Value
            };

            if (axisX != null) parameters["axis_x"] = axisX.Value;
            if (axisY != null) parameters["axis_y"] = axisY.Value;
            if (axisZ != null) parameters["axis_z"] = axisZ.Value;

            return await sendCommand("rotate_element", parameters);
        }
    }

    public static class MirrorElementHandler
    {
        public static async Task<int> HandleAsync(string[] args, Func<string, object?, Task<int>> sendCommand)
        {
            var elementId = ArgHelper.GetInt(args, "--element-id", "-e");
            var originX = ArgHelper.GetDouble(args, "--origin-x");
            var originY = ArgHelper.GetDouble(args, "--origin-y");
            var originZ = ArgHelper.GetDouble(args, "--origin-z");
            var normalX = ArgHelper.GetDouble(args, "--normal-x");
            var normalY = ArgHelper.GetDouble(args, "--normal-y");
            var normalZ = ArgHelper.GetDouble(args, "--normal-z");

            if (elementId == null)
            {
                Console.WriteLine("Error: --element-id is required");
                return 1;
            }

            if (normalX == null && normalY == null && normalZ == null)
            {
                Console.WriteLine("Error: mirror plane normal is required (--normal-x, --normal-y, --normal-z)");
                return 1;
            }

            var parameters = new Dictionary<string, object>
            {
                ["element_id"] = elementId.Value
            };

            if (originX != null) parameters["origin_x"] = originX.Value;
            if (originY != null) parameters["origin_y"] = originY.Value;
            if (originZ != null) parameters["origin_z"] = originZ.Value;
            if (normalX != null) parameters["normal_x"] = normalX.Value;
            if (normalY != null) parameters["normal_y"] = normalY.Value;
            if (normalZ != null) parameters["normal_z"] = normalZ.Value;

            return await sendCommand("mirror_element", parameters);
        }
    }

    public static class SetOffsetHandler
    {
        public static async Task<int> HandleAsync(string[] args, Func<string, object?, Task<int>> sendCommand)
        {
            var elementId = ArgHelper.GetInt(args, "--element-id", "-e");
            var baseOffset = ArgHelper.GetDouble(args, "--base-offset");
            var topOffset = ArgHelper.GetDouble(args, "--top-offset");

            if (elementId == null)
            {
                Console.WriteLine("Error: --element-id is required");
                return 1;
            }

            if (baseOffset == null && topOffset == null)
            {
                Console.WriteLine("Error: at least one of --base-offset or --top-offset is required");
                return 1;
            }

            var parameters = new Dictionary<string, object>
            {
                ["element_id"] = elementId.Value
            };

            if (baseOffset != null)
                parameters["base_offset"] = baseOffset.Value;

            if (topOffset != null)
                parameters["top_offset"] = topOffset.Value;

            return await sendCommand("set_offset", parameters);
        }
    }
}
