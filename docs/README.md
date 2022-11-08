# Increo ServiceBase Documentation

### Table of Contents

* [Requirements](#requirements)
* [Setup](#setup)
* Implementation
    - [Startup, what goes into the Program.cs file](startup-program-cs-file.md)
    - [Using TaskHelper to streamline worker startup and logging](using-taskhelper.md)
    - [Working with config](working-with-config.md)
    - [Local/system logging](local-system-logging.md)
    - [Making database calls to MSSQL](database-mssql.md)
    - [Making API calls](api-calls.md)
    - [Example implementation](example.md)
    - [How to setup release as Windows service](how-to-windows-service.md)
    - [How to setup release as Linux daemon](how-to-linux-daemon.md)

## Requirements

Increo ServiceBase runs on the .NET 6 platform, which can be downloaded from [https://dotnet.microsoft.com/en-us/download](https://dotnet.microsoft.com/en-us/download).

## Setup

The easiest way to start developing services under the ServiceBase is to run one of the setup scripts which will scaffold everything you need to get started with development.

To run the setup, download either the [isb_setup.ps1 PowerShell script](setup/isb_setup.ps1) or the [isb_setup.sh bash script](setup/isb_setup.sh) and run it from where you want the project folder to be.
It requires one parameter, the name of the project.

Example:

```powershell
.\isb_setup.ps1 myCoolProject
```

```bash
./isb_setup.sh myCoolProject
```

### Manual Setup

If you want to setup the project manually, follow these steps:

* Create a new C# project using the [Worker Service](https://learn.microsoft.com/en-us/dotnet/core/extensions/workers) template.
* Set the OutputType of the project to EXE.
* Create a local Git repository.
* Add the [Increo ServiceBase](https://github.com/nagilum/IncreoServiceBase) as a submodule.
* Add the following NuGet packages:
    - [Microsoft.EntityFrameworkCore.SqlServer](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.SqlServer/)
    - [Microsoft.Extensions.Hosting.Systemd](https://www.nuget.org/packages/Microsoft.Extensions.Hosting.Systemd) or [Microsoft.Extensions.Hosting.WindowsServices](https://www.nuget.org/packages/Microsoft.Extensions.Hosting.WindowsServices/)