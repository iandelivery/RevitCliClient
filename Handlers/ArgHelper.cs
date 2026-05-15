using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitCliClient.Handlers
{
    public static class ArgHelper
    {
        public static string? FindArg(string[] args, params string[] flags)
        {
            foreach (var flag in flags)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == flag && i + 1 < args.Length)
                        return args[i + 1];
                }
            }
            return null;
        }

        public static bool HasFlag(string[] args, params string[] flags)
        {
            foreach (var flag in flags)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == flag)
                        return true;
                }
            }
            return false;
        }

        public static int? GetInt(string[] args, params string[] flags)
        {
            var val = FindArg(args, flags);
            if (val == null) return null;
            return int.TryParse(val, out int result) ? result : (int?)null;
        }

        public static double? GetDouble(string[] args, params string[] flags)
        {
            var val = FindArg(args, flags);
            if (val == null) return null;
            return double.TryParse(val, out double result) ? result : (double?)null;
        }

        public static object TryParseValue(string value)
        {
            if (int.TryParse(value, out int i)) return i;
            if (double.TryParse(value, out double d)) return d;
            return value;
        }

        public static List<int>? ParseIds(string ids)
        {
            try
            {
                var result = new List<int>();
                foreach (var part in ids.Split(','))
                {
                    var trimmed = part.Trim();
                    if (string.IsNullOrEmpty(trimmed)) continue;
                    if (!int.TryParse(trimmed, out int id))
                        return null;
                    result.Add(id);
                }
                return result;
            }
            catch
            {
                return null;
            }
        }

        public static int[]? ParseIdsToArray(string ids)
        {
            try
            {
                var result = new List<int>();
                foreach (var part in ids.Split(','))
                {
                    var trimmed = part.Trim();
                    if (string.IsNullOrEmpty(trimmed)) continue;
                    if (!int.TryParse(trimmed, out int id))
                        return null;
                    result.Add(id);
                }
                return result.ToArray();
            }
            catch
            {
                return null;
            }
        }
    }
}
