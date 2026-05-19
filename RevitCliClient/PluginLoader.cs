// SPDX-License-Identifier: MIT
using RevitCliClient.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace RevitCliClient
{
    public static class PluginLoader
    {
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode",
            Justification = "Plugin loading uses reflection - not used in AOT mode")]
        [UnconditionalSuppressMessage("Trimming", "IL2072:DynamicallyAccessedMembers",
            Justification = "Plugin loading uses reflection - not used in AOT mode")]
        public static void LoadPlugins(CommandRegistry registry, string pluginDir)
        {
            if (!Directory.Exists(pluginDir))
                return;

            foreach (var dll in Directory.GetFiles(pluginDir, "*.dll"))
            {
                LoadPluginDll(registry, dll);
            }
        }

        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode",
            Justification = "Plugin loading uses reflection - not used in AOT mode")]
        [UnconditionalSuppressMessage("Trimming", "IL2072:DynamicallyAccessedMembers",
            Justification = "Plugin loading uses reflection - not used in AOT mode")]
        private static void LoadPluginDll(CommandRegistry registry, string dllPath)
        {
            try
            {
                var context = new PluginLoadContext(dllPath);
                var assembly = context.LoadFromAssemblyPath(dllPath);

                var pluginTypes = assembly.GetTypes()
                    .Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);

                foreach (var pluginType in pluginTypes)
                {
                    var plugin = (IPlugin)Activator.CreateInstance(pluginType)!;

                    foreach (var command in plugin.GetCommands())
                    {
                        registry.Register(command);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Plugin loading failed: {Path.GetFileName(dllPath)} - {ex.Message}");
            }
        }

        private sealed class PluginLoadContext : AssemblyLoadContext
        {
            private readonly AssemblyDependencyResolver _resolver;

            public PluginLoadContext(string pluginPath) : base(isCollectible: false)
            {
                _resolver = new AssemblyDependencyResolver(pluginPath);
            }

        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode",
            Justification = "Plugin loading uses reflection - not used in AOT mode")]
        protected override Assembly? Load(AssemblyName assemblyName)
            {
                var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
                if (assemblyPath != null)
                    return LoadFromAssemblyPath(assemblyPath);

                return null;
            }
        }
    }
}
