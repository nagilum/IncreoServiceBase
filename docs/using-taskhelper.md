# TaskHelper

TaskHelper is used in two ways, to scaffold repeating tasks, and to give logging functionality to each sync.

### Table of Contents

* [Examples](#examples)
* [GetData](#getdata)
* [SetData](#setdata)
* [Log functions](#log-functions)
* [Save](#save)
* [Info](#info)
* [Increment a Counter](#increment-a-counter)
* [Decrease a Counter](#decrease-a-counter)

## Examples

Inside the `ExecuteAsync` function in your worker you can create the looping, invoking, and logging yourself, or you can use `TaskHelper` to set everything up for you, like so:

```csharp
internal class MyAwesomeWorker : BackgroundService
{
    private ILogger<MyAwesomeWorker> Logger { get; }

    public MyAwesomeWorker(ILogger<MyAwesomeWorker> logger)
    {
        this.Logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ctoken)
    {
        await TaskHelper.Setup(
            this.GetType().Name,        // The name of the worker, for logging.
            ctoken,                     // Pass along the cancellation token.
            new TimeSpan(0, 20, 0),     // Delay 20 minutes before starting the main function 'Sync'.
            new TimeSpan(6, 0, 0),      // When the 'Sync' function has finished running, wait 6 hours before re-running.
            this.Logger,                // Pass along the logging interface.
            this.Sync);                 // The actual function to run. You can chain multiple functions here.
    }

    private async Task Sync(
        TashHelper helper,
        CancellationToken ctoken)
    {
        // Here you can use the logging interface with a event-id for Windows logging.
        this.Logger.LogInformation(
            helper.GetEventId(),
            "This is a info log message.");

        // It's easy to log errors via the helper class.
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception ex)
        {
            await helper.LogErrorAsync(ex, ctoken);
        }

        // You can also store data on each run.
        // This will store a key/value called 'Total' with the value 300.
        helper.SetData(
            DataKey.Total,
            300);

        // You can also fetch data.
        var x = helper.GetData(DataKey.Total);

        // If you're working with items and just wanna increment a counter.
        helper.IncrementCounter(DataKey.Created);

        // If you're working with a list of items and wanna log to console every x with remaining time.
        var processed = 0;
        var items = new List<object>(); // Let's pretend this list is populated with a lot of items.

        foreach (var item in items)
        {
            processed++;

            // This will trigger every 50 items.
            if (processed % 50 == 0)
            {
                // This will give you a TimeSpan back with a calculated remaining time.
                var remainingTimeEta = helper.CalcAverageTimeRemaining(items.Count, processed);

                this.Logger.LogInformation(
                    helper.GetEventId(),
                    $"Processed {processed} of {items.Count} items. " +
                    $"Roughly {remainingTimeEta} remaining.");
            }
        }
    }
}
```

## GetData

XX

## SetData

XX

## Log functions

XX

## Save

XX

## Info

XX

## Increment a Counter

XX

## Decrease a Counter

XX