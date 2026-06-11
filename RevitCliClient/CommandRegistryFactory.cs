using RevitCliClient.Extensions;
using RevitCliClient.Handlers;
using System.IO;

namespace RevitCliClient
{
    internal static class CommandRegistryFactory
    {
        public static CommandRegistry Create()
        {
            var registry = new CommandRegistry();

            // System / utility
            registry.Register(new PingHandler());
            registry.Register(new StatusHandler());
            registry.Register(new HealthHandler());
            registry.Register(new TaskHandler());
            registry.Register(new DocInfoHandler());
            registry.Register(new ElementsHandler());
            registry.Register(new ElementByIdHandler());
            registry.Register(new TypesHandler());
            registry.Register(new LevelsHandler());
            registry.Register(new ParamsHandler());
            registry.Register(new DeleteHandler());
            registry.Register(new UndoHandler());
            registry.Register(new BatchHandler());
            registry.Register(new RawHandler());

            // Create elements
            registry.Register(new CreateWallHandler());
            registry.Register(new CreateWallsHandler());
            registry.Register(new CreateDoorHandler());
            registry.Register(new CreateWindowHandler());
            registry.Register(new CreateGridCliHandler());
            registry.Register(new CreateFamilyInstanceHandler());
            registry.Register(new CreateViewHandler());
            registry.Register(new CreateSheetHandler());
            registry.Register(new CreateRoomHandler());

            // Modify elements
            registry.Register(new SetParamHandler());
            registry.Register(new BatchSetParamHandler());
            registry.Register(new SetWallConstraintCliHandler());
            registry.Register(new SetWallsConstraintCliHandler());
            registry.Register(new ApplyViewTemplateHandler());
            registry.Register(new TagRoomsHandler());
            registry.Register(new PlaceOnSheetHandler());
            registry.Register(new HideElementsHandler("hide_elements"));
            registry.Register(new HideElementsHandler("unhide_elements"));
            registry.Register(new MoveElementHandler());
            registry.Register(new CopyElementHandler());
            registry.Register(new RotateElementHandler());
            registry.Register(new MirrorElementHandler());
            registry.Register(new SetOffsetHandler());

            // Query / get
            registry.Register(new SearchElementsHandler());
            registry.Register(new GetViewsHandler());
            registry.Register(new GetSheetsHandler());
            registry.Register(new GetRoomsHandler());
            registry.Register(new GetFamilySymbolHandler());
            registry.Register(new GetFamilySymbolsCliHandler());
            registry.Register(new GetSymbolInstancesHandler());
            registry.Register(new SetActiveViewCliHandler());
            registry.Register(new ZoomToFitHandler());
            registry.Register(new SelectElementsHandler());

            // Export
            registry.Register(new ExportViewHandler());
            registry.Register(new BatchExportHandler());

            // Document management
            registry.Register(new DocOpenHandler());
            registry.Register(new DocCloseHandler());
            registry.Register(new DocSaveHandler());
            registry.Register(new DocSaveAsHandler());
            registry.Register(new DocSyncHandler());
            registry.Register(new DocListHandler());

            // Built-in plugins
            var builtInPlugin = new ExtensionsPlugin();
            foreach (var command in builtInPlugin.GetCommands())
                registry.Register(command);

            // External plugins
            var pluginDir = Path.Combine(AppContext.BaseDirectory, "plugins");
            PluginLoader.LoadPlugins(registry, pluginDir);

            return registry;
        }
    }
}
