# Startup

The main entry point for your application is the `Program.cs` file containing the static method `Main()`.
Here we will explain the various options you have for populating this file.

### Table of Contents

* [Basic Example](#basic-example)
* [Add Multiple Services](#add-multiple-services)
* [Add Timestamp to Console Logging](#add-timestamp-to-console-logging)

## Basic Example

Here is an example of a pretty basic setup:

```csharp
internal static class Program
{
    private static async Task Main(string[] args)
    {
        await Host.CreateDefaultBuilder(args)
            .ConfigureServices(services => {
                // Add MyAwesomeWorker as service.
                services.AddHostedService<MyAwesomeWorker>();
            })
            .Build()
            .RunAsync();
    }
}

internal class MyAwesomeWorker : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ctoken)
    {
        while (!ctoken.IsCancellationRequested)
        {
            // Write current date/time.
            Console.WriteLine(DateTimeOffset.Now);

            // Wait 1 second.
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
}
```

This will setup an app that adds `MyAwesomeWorker` as a service. The service will write the current date/time to console, every second.

## Add Multiple Services

If you want to add muiltiple services, you can do so like this:

```csharp
services.AddHostedService<MyAwesomeWorker>();
services.AddHostedService<MySecondService>();
services.AddHostedService<AThirdService>();
```

## Add Timestamp to Console Logging

You can alter the timestamp format for the console logger.
It supports the custom date/time format in .NET: [https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings](https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings)

Inside the `ConfigureServices()` function, you can add:

```csharp
services.AddLogging(
    ins => ins.AddSimpleConsole(
        opt => opt.TimestampFormat = "[HH:mm:ss] "));
```

This will add a timestamp looking like this: `[12:03:24]` (with a space at the end).