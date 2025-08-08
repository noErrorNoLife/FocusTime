# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

FocusTime is an Avalonia-based desktop application written in C# targeting .NET 9.0. It appears to be a focus/pomodoro timer application (番茄钟 means "tomato clock" in Chinese, referring to the Pomodoro Technique).

## Architecture

- **Framework**: Avalonia UI 11.3.2 with Fluent theme
- **Pattern**: MVVM using CommunityToolkit.Mvvm
- **Target**: Cross-platform desktop application (.NET 9.0)
- **Entry Point**: `Program.cs` -> `App.axaml.cs` -> `MainWindow`

### Key Components

- `App.axaml.cs`: Application initialization, disables Avalonia data annotation validation in favor of CommunityToolkit
- `ViewModels/`: Contains view models inheriting from `ViewModelBase`
- `Views/`: Contains XAML views with code-behind
- `Models/`: Currently empty, intended for data models
- `ViewLocator.cs`: Handles view resolution for MVVM pattern

## Development Commands

### Build
```bash
dotnet build FocusTime/FocusTime.csproj
```

### Run
```bash
dotnet run --project FocusTime/FocusTime.csproj
```

### Debug Build
```bash
dotnet build FocusTime/FocusTime.csproj --configuration Debug
```

### Release Build
```bash
dotnet build FocusTime/FocusTime.csproj --configuration Release
```

### AOT Publishing (Windows x64)
```bash
dotnet publish FocusTime/FocusTime.csproj -c Release -r win-x64 --self-contained
```

### AOT Publishing (Linux x64)
```bash
dotnet publish FocusTime/FocusTime.csproj -c Release -r linux-x64 --self-contained
```

### AOT Publishing (macOS x64)
```bash
dotnet publish FocusTime/FocusTime.csproj -c Release -r osx-x64 --self-contained
```

## Project Structure Notes

- Main project is in `FocusTime/` directory
- Uses compiled bindings by default (`AvaloniaUseCompiledBindingsByDefault`)
- Avalonia.Diagnostics package is only included in Debug builds
- Application manifest file (`app.manifest`) is used for Windows-specific settings