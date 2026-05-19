# Contributing to RevitCliClient

Thank you for your interest in contributing to RevitCliClient! This project enables AI agents to drive Autodesk Revit through a CLI and HTTP API.

## How to Contribute

### Reporting Issues

- Search existing issues before creating a new one
- Include steps to reproduce, expected behavior, and actual behavior
- Specify the .NET version, OS, and Revit version (if applicable)

### Submitting Pull Requests

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/my-new-command`)
3. Make your changes
4. Ensure the project builds (`dotnet build`)
5. Commit with a clear message
6. Push to your fork and submit a pull request

### Adding a New Command

Follow the guide in [README.md](README.md#adding-a-new-command) for the two options:

- **Built-in command** — Add a handler in `Handlers/` implementing `ICliCommand`, register in `Program.cs`
- **Plugin command** — Create a separate class library referencing `RevitCliClient.Abstractions`

For the bridge-side handler, follow [BRIDGE_IMPLEMENTATION.md](BRIDGE_IMPLEMENTATION.md).

### Code Style

- Follow the existing code patterns in the repository
- Use `ArgHelper` for argument parsing in all handlers
- Every handler must implement `ICliCommand` with all metadata properties
- Use `CommandCategory` enum for categorization
- Target .NET 8.0 for the main project, netstandard2.0 + net8.0 for Abstractions

### Commit Messages

- Use the imperative mood (`Add feature` not `Added feature`)
- Keep the first line under 72 characters
- Reference issues when applicable (`Fix #123`)

## License

By contributing, you agree that your contributions will be licensed under the MIT License.
