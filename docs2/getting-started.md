# Getting Started

[Documentation](readme.md) &gt; Getting Started

## Requirements

Increo ServiceBase run on the .NET 6 framework, which can be downloaded from <https://dotnet.microsoft.com/en-us/download>.

## How to setup worker with Increo ServiceBase

The easiest way to setup a worker template with Increo ServiceBase is to use one of the automated setup scripts, either the [isb_setup.ps1 PowerShell script](../setup/isb_setup.ps1) or the [isb_setup.sh bash script](../setup/isb_setup.sh). Both scripts does the same thing and take the same parameters.

Parameters
* `project-name` - The name for your project. This parameter is required.
* `-ws` - Adds the Windows Service hosting package.
* `-ld` - Adds the Linux Daemon hosting package.

Example
```console
.\isb_setup.ps1 myCoolProject
```

Example with Windows Service
```console
.\isb_setup.ps1 myCoolProject -ws
```

## What's installed/setup

If you would like to set everything up yourself, here are all the steps the setup scripts takes.

1. Create a new C# project with the Worker template.
   ```console
   dotnet new worker myCoolProject
   ```
2. Enter the folder for the project.
   ```console
   cd myCoolProject
   ```
3. Add NyGet package for Microsoft EntityFramework Core.
   ```console
   TODO
   ```
4. Initiate a Git repo, create a `.gitignore` file, and add everything.
   ```console
   git init

   echo .vs/ >.gitignore
   echo bin/ >>.gitignore
   echo obj/ >>.gitignore
   echo Properties/ >>.gitignore
   echo *.user >>.gitignore

   git add *
   git commit -m "Initial commit"
   ```
5. Add Increo ServiceBase as a submodule.
   ```console
   git submodule add https://github.com/nagilum/IncreoServiceBase
   ```
6. Add the example worker file from assets.
   ```console
   TODO
   copy ./IncreoServiceBase/assets/worker-example.csharp ./Worker.cs /x
   ```
7. If the `-ws` param was used, add the NuGet package for Windows Service hosting.
   ```console
   TODO
   ```
8. If the `-ld` param was used, add the NuGet package for Linux Daemon hosting.
   ```console
   TODO
   ```

## How to run after install

To test that everything works after using the setup script, you can simply run `dotnet run` inside the folder to compile and run the project. If you used the setup script and didn't modify the `Worker.cs` file, then you should see the time and date being printed in console/terminal every second.

Running
```console
dotnet run
```

Should produce an output like this
```console
TODO
```

## How to write your first worker

Here is an example of a simple worker that will print date/time every second.

```csharp
// TODO
```