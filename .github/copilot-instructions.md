# Copilot Instructions for theswamp-web

## Project Overview

This repository is the codebase for [www.theswamp.co.uk](https://www.theswamp.co.uk), a personal website that includes a Blazor WebAssembly front-end, Azure Functions API back-end, an IoT agent running on a Raspberry Pi, and a shared class library.

## Repository Structure

```
theswamp-web/
├── Client/          # Blazor WebAssembly front-end (net6.0)
├── Api/             # Azure Functions v4 API back-end (net6.0)
├── Shared/          # Shared C# class library (netstandard2.0)
├── Agent/           # IoT agent for Raspberry Pi sensors
├── Tests/
│   └── Integration/ # Integration tests
└── ImportLWIN/      # LWIN (wine database) data import tool
```

## Technology Stack

- **Front-end**: Blazor WebAssembly (.NET 6), Plotly.Blazor charts, SignalR client
- **Back-end**: Azure Functions v4 (.NET 6), Entity Framework Core (SQL Server), SignalR, Azure Service Bus, OpenAI GPT API
- **Shared**: C# class library (netstandard2.0), Newtonsoft.Json / System.Text.Json
- **Infrastructure**: Azure Static Web Apps, Azure SQL Server

## Build & Run

Build the solution from the repository root:
```bash
dotnet build www.theswamp.co.uk.sln
```

Run tests:
```bash
dotnet test www.theswamp.co.uk.sln
```

Run the Blazor client locally:
```bash
cd Client
dotnet run
```

Run the Azure Functions API locally (requires `Api/local.settings.json` — see `Api/local.settings.example.json`):
```bash
cd Api
func start
```

## Coding Conventions

- **Namespaces**: `TheSwamp.Client`, `TheSwamp.Api`, `TheSwamp.Shared`, `TheSwamp.Agent`
- **Language**: C# — follow standard .NET naming conventions (PascalCase for types and members, camelCase for local variables)
- **Dependency Injection**: Services are registered in `Api/Startup.cs` using `IFunctionsHostBuilder`
- **Data access**: Entity Framework Core with SQL Server; separate `DbContext` classes per domain (`ApiContext`, `IotContext`, `LWINContext`, `TrackingContext`) in `Api/DAL/`
- **API**: HTTP-triggered Azure Functions in `Api/`; shared request/response models live in `Shared/`
- **Front-end components**: Razor components in `Client/Pages/` (pages) and `Client/Shared/` (reusable components); use `_Imports.razor` for common `using` directives

## Configuration

- API configuration uses `local.settings.json` (not committed); copy `local.settings.example.json` to `local.settings.json` and fill in values
- Key config keys: `connectionstring.swampnet` (SQL Server), `openai.apikey`
- Client base address is read from `appsettings.json` under `BaseAddress`, falling back to the host environment base address

## Testing

- Integration tests are in `Tests/Integration/`
- Run with `dotnet test Tests/Integration/Integration.csproj`
- There are currently no unit tests; prefer adding integration tests for new API functionality

## Deployment

- Deployment is handled automatically via GitHub Actions (see `.github/workflows/`)
- Pushes to `main` trigger a build and deploy to Azure Static Web Apps
- The `Client` directory is the app source, `Api` is the API source, and `wwwroot` is the output directory
