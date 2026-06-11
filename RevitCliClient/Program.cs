// SPDX-License-Identifier: MIT
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace RevitCliClient
{
    public static class Program
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public static async Task<int> Main(string[] args)
        {
            if (args.Length == 0 || args[0] == "--help" || args[0] == "-h")
            {
                PrintHelp(null);
                return 0;
            }

            var (baseUrl, cmdIndex) = ParseArgs(args);

            if (cmdIndex >= args.Length)
            {
                PrintHelp(null);
                return 0;
            }

            var registry = CommandRegistryFactory.Create();
            var command = args[cmdIndex];

            if (registry.TryGetCommand(command, out var cmd) && cmd is not null)
            {
                var client = new SseClient(_httpClient, baseUrl);
                return await cmd.HandleAsync(args, client.ExecuteAsync);
            }

            Console.WriteLine($"Unknown command: {command}");
            PrintHelp(registry);
            return 1;
        }

        private static (string baseUrl, int cmdIndex) ParseArgs(string[] args)
        {
            var baseUrl = "http://localhost:5000";
            var cmdIndex = 0;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--url" && i + 1 < args.Length)
                {
                    baseUrl = args[i + 1].TrimEnd('/');
                    cmdIndex = i + 2;
                    break;
                }
            }

            return (baseUrl, cmdIndex);
        }

        private static void PrintHelp(CommandRegistry? registry)
        {
            registry ??= CommandRegistryFactory.Create();
            Console.WriteLine(HelpText.Generate(registry));
        }
    }
}
