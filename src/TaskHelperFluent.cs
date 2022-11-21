using Increo.ServiceBase.Extenders;
using Increo.ServiceBase.Models.Database;

namespace Increo.ServiceBase
{
    internal class TaskHelperFluent
    {
        #region Properties

        /// <summary>
        /// Run name.
        /// </summary>
        private string? Name { get; set; }

        /// <summary>
        /// Logging interface.
        /// </summary>
        private ILogger? Logger { get; set; }

        /// <summary>
        /// Startup delay.
        /// </summary>
        private TimeSpan? StartupDelay { get; set; }

        /// <summary>
        /// Interval delay.
        /// </summary>
        private TimeSpan? IntervalDelay { get; set; }

        /// <summary>
        /// List of functions to run.
        /// </summary>
        private List<Func<TaskHelper, CancellationToken, Task>> Functions { get; set; } = new();

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new fluent helper.
        /// </summary>
        /// <param name="name">Name of run.</param>
        public TaskHelperFluent(string? name)
        {
            Name = name;
        }

        #endregion

        #region Setup

        /// <summary>
        /// Set up task run functions.
        /// </summary>
        /// <param name="name">Name of run.</param>
        public static TaskHelperFluent Setup(string name)
        {
            return new(name);
        }

        #endregion

        #region Triggers

        /// <summary>
        /// Add a logging interface.
        /// </summary>
        /// <param name="logger">Logging interface.</param>
        public TaskHelperFluent AddLogger(ILogger logger)
        {
            Logger = logger;
            return this;
        }

        /// <summary>
        /// Add a startup delay.
        /// </summary>
        /// <param name="delay">Delay.</param>
        public TaskHelperFluent AddStartupDelay(TimeSpan delay)
        {
            StartupDelay = delay;
            return this;
        }

        /// <summary>
        /// Add an interval delay.
        /// </summary>
        /// <param name="delay">Delay.</param>
        public TaskHelperFluent AddIntervalDelay(TimeSpan delay)
        {
            IntervalDelay = delay;
            return this;
        }

        /// <summary>
        /// Add function to run.
        /// </summary>
        /// <param name="function">Function.</param>
        public TaskHelperFluent AddFunction(Func<TaskHelper, CancellationToken, Task> function)
        {
            Functions.Add(function);
            return this;
        }

        /// <summary>
        /// Run the whole setup.
        /// </summary>
        /// <param name="ctoken">Cancellation token.</param>
        public async Task RunAsync(CancellationToken? ctoken = null)
        {
            // Do we have any functions to run?
            if (Functions.Count == 0)
            {
                throw new Exception(
                    "No functions defined to run. Use AddFunction() to add one.");
            }

            // Do we have a startup delay to wait for?
            if (StartupDelay.HasValue)
            {
                Logger?.LogWarning(
                    $"Waiting {StartupDelay.Value.ToHumanReadable(true)} before starting the first run.");

                await Task.Delay(
                    StartupDelay.Value,
                    ctoken ?? CancellationToken.None);
            }

            // Run until interrupted.
            while (ctoken?.IsCancellationRequested != true)
            {
                // Create a new run.
                var run = await TaskRun.CreateNewAsync(
                    Name ?? string.Empty,
                    ctoken);

                var eventId = new EventId((int)run.Id);
                var helper = new TaskHelper(run);

                Logger?.LogInformation(
                    eventId,
                    $"Starting a new run with {Functions.Count} function(s).");

                // Run all attached functions.
                try
                {
                    foreach (var function in Functions)
                    {
                        Logger?.LogInformation(
                            eventId,
                            $"Running function \"{function.Method.Name}\".");

                        var start = DateTimeOffset.Now;

                        helper.SetFunctionStart(start);

                        await function.Invoke(
                            helper,
                            ctoken ?? CancellationToken.None);

                        var end = DateTimeOffset.Now;
                        var duration = end - start;

                        Logger?.LogInformation(
                            eventId,
                            $"Finished running function \"{function.Method.Name}\" which took {duration.ToHumanReadable(true)}.");

                        await helper.LogInformationAsync(
                            $"Finished running function \"{function.Method.Name}\" which took {duration.ToHumanReadable(true)}.",
                            cancellationToken: ctoken);
                    }
                }
                catch (Exception ex)
                {
                    Logger?.LogError(
                        eventId,
                        ex,
                        ex.Message);

                    await helper.LogErrorAsync(
                        ex,
                        ctoken);
                }

                // Mark run as finished.
                var finished = DateTimeOffset.Now;

                run.Finished = finished;
                run.RunTimeSeconds = (int)(finished - run.Started).TotalSeconds;

                // Save run.
                await helper.SaveAsync();

                // Do we have a interval delay to wait for?
                if (IntervalDelay.HasValue)
                {
                    Logger?.LogInformation(
                        eventId,
                        $"Waiting {IntervalDelay.Value.ToHumanReadable(true)} before re-running.");

                    await Task.Delay(
                        IntervalDelay.Value,
                        ctoken ?? CancellationToken.None);
                }
            }

            // The service has been cancelled, log it.
            Logger?.LogWarning(
                "Cancellation requested. Shutting down.");
        }

        #endregion
    }
}