// SPDX-License-Identifier: MIT
namespace RevitCliClient.Abstractions
{
    public enum CommandCategory
    {
        System,
        Query,
        Create,
        Modify,
        Transform,
        ViewExport,
        Raw,
        Custom
    }

    public static class CommandCategoryDisplay
    {
        public static string GetDisplayName(this CommandCategory category) => category switch
        {
            CommandCategory.System => "System",
            CommandCategory.Query => "Document & Query",
            CommandCategory.Create => "Create",
            CommandCategory.Modify => "Modify",
            CommandCategory.Transform => "Transform",
            CommandCategory.ViewExport => "View & Export",
            CommandCategory.Raw => "Raw",
            CommandCategory.Custom => "Custom",
            _ => category.ToString()
        };
    }
}
