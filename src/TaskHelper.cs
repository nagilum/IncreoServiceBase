using Increo.ServiceBase.Attributes;
using Increo.ServiceBase.Extenders;
using Increo.ServiceBase.Models.Database;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace Increo.ServiceBase
{
    internal class TaskHelper
    {
        /// <summary>
        /// Run identificator.
        /// </summary>
        public TaskRun Run { get; set; }

        /// <summary>
        /// Internal storage while running.
        /// </summary>
        public Dictionary<string, object>? DataStorage { get; set; }

        /// <summary>
        /// Local storage of the event ID.
        /// </summary>
        private EventId? EventId { get; set; }

        #region Constructor

        public TaskHelper(TaskRun run)
        {
            Run = run;
        }

        #endregion

        #region Get data

        /// <summary>
        /// Get data from storage.
        /// </summary>
        /// <param name="dataKey">Key.</param>
        /// <param name="keyPrefix">Key prefix.</param>
        /// <returns>Data.</returns>
        public object? GetData(
            DataKey dataKey,
            string? keyPrefix = null)
        {
            var key = Enum.GetName(
                typeof(DataKey),
                dataKey);

            if (keyPrefix != null)
            {
                key = keyPrefix + "." + key;
            }

            if (key == null)
            {
                return null;
            }

            return GetData(key);
        }

        /// <summary>
        /// Get data from storage.
        /// </summary>
        /// <typeparam name="T">Cast as type.</typeparam>
        /// <param name="dataKey">Key.</param>
        /// <param name="keyPrefix">Key prefix.</param>
        /// <returns>Data.</returns>
        public T? GetData<T>(
            DataKey dataKey,
            string? keyPrefix = null)
        {
            var key = Enum.GetName(
                typeof(DataKey),
                dataKey);

            if (keyPrefix != null)
            {
                key = keyPrefix + "." + key;
            }

            if (key == null)
            {
                return default;
            }

            return GetData<T>(key);
        }

        /// <summary>
        /// Get data from storage.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns>Data.</returns>
        public object? GetData(string key)
        {
            return DataStorage?.ContainsKey(key) == true
                ? DataStorage[key]
                : null;
        }

        /// <summary>
        /// Get data from storage.
        /// </summary>
        /// <typeparam name="T">Cast as type.</typeparam>
        /// <param name="key">Key.</param>
        /// <returns>Data.</returns>
        public T? GetData<T>(string key)
        {
            var data = GetData(key);

            return data != null
                ? (T)data
                : default;
        }

        #endregion

        #region Set data

        /// <summary>
        /// Set data in storage.
        /// </summary>
        /// <param name="dataKey">Key.</param>
        /// <param name="value">Value.</param>
        /// <param name="keyPrefix">Key prefix.</param>
        public void SetData(
            DataKey dataKey,
            object value,
            string? keyPrefix = null)
        {
            var key = Enum.GetName(
                typeof(DataKey),
                dataKey);

            if (keyPrefix != null)
            {
                key = keyPrefix + "." + key;
            }

            if (key == null)
            {
                return;
            }

            SetData(key, value);
        }

        /// <summary>
        /// Set data in storage.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public void SetData(
            string key,
            object value)
        {
            DataStorage ??= new();
            DataStorage[key] = value;
        }

        #endregion

        #region Log functions

        /// <summary>
        /// Log a message to database.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="stackTrace">Possible stack-trace.</param>
        /// <param name="source">Possible source.</param>
        /// <param name="referenceIds">Possible reference IDs to external objects.</param>
        /// <param name="data">Extra data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task LogAsync(
            LogType logType,
            string? message = null,
            string? stackTrace = null,
            string? source = null,
            long[]? referenceIds = null,
            System.Collections.IDictionary? data = null,
            CancellationToken? cancellationToken = null)
        {
            using var db = new EfContext();

            var logTypeStr = logType.GetAttribute<LogTypeDatabaseValueAttribute>()?.Name
                             ?? "unkn";

            var entry = new TaskRunLog
            {
                RunId = Run.Id,
                Created = DateTimeOffset.Now,
                LogType = logTypeStr,
                Message = message,
                StackTrace = stackTrace,
                Source = source
            };

            if (referenceIds != null)
            {
                using var stream = new MemoryStream();

                await JsonSerializer.SerializeAsync(
                    stream,
                    referenceIds,
                    cancellationToken: cancellationToken ?? CancellationToken.None);

                entry.ReferenceIds =
                    Encoding.UTF8.GetString(
                        stream.ToArray());
            }

            if (data != null)
            {
                using var stream = new MemoryStream();

                await JsonSerializer.SerializeAsync(
                    stream,
                    data,
                    cancellationToken: cancellationToken ?? CancellationToken.None);

                entry.Data =
                    Encoding.UTF8.GetString(
                        stream.ToArray());
            }

            await db.TaskRunLogs.AddAsync(entry, cancellationToken ?? CancellationToken.None);
            await db.SaveChangesAsync(cancellationToken ?? CancellationToken.None);
        }

        /// <summary>
        /// Log a message as 'debug'.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="stackTrace">Possible stack-trace.</param>
        /// <param name="source">Possible source.</param>
        /// <param name="referenceIds">Possible reference IDs to external objects.</param>
        /// <param name="data">Extra data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task LogDebugAsync(
            string? message = null,
            string? stackTrace = null,
            string? source = null,
            long[]? referenceIds = null,
            System.Collections.IDictionary? data = null,
            CancellationToken? cancellationToken = null)
        {
            await LogAsync(
                LogType.Debug,
                message,
                stackTrace,
                source,
                referenceIds,
                data,
                cancellationToken);
        }

        /// <summary>
        /// Log a message as 'info'.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="stackTrace">Possible stack-trace.</param>
        /// <param name="source">Possible source.</param>
        /// <param name="referenceIds">Possible reference IDs to external objects.</param>
        /// <param name="data">Extra data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task LogInformationAsync(
            string? message = null,
            string? stackTrace = null,
            string? source = null,
            long[]? referenceIds = null,
            System.Collections.IDictionary? data = null,
            CancellationToken? cancellationToken = null)
        {
            await LogAsync(
                LogType.Information,
                message,
                stackTrace,
                source,
                referenceIds,
                data,
                cancellationToken);
        }

        /// <summary>
        /// Log a message as 'warning'.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="stackTrace">Possible stack-trace.</param>
        /// <param name="source">Possible source.</param>
        /// <param name="referenceIds">Possible reference IDs to external objects.</param>
        /// <param name="data">Extra data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task LogWarningAsync(
            string? message = null,
            string? stackTrace = null,
            string? source = null,
            long[]? referenceIds = null,
            System.Collections.IDictionary? data = null,
            CancellationToken? cancellationToken = null)
        {
            await LogAsync(
                LogType.Warning,
                message,
                stackTrace,
                source,
                referenceIds,
                data,
                cancellationToken);
        }

        /// <summary>
        /// Log a message as 'error'.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="stackTrace">Possible stack-trace.</param>
        /// <param name="source">Possible source.</param>
        /// <param name="referenceIds">Possible reference IDs to external objects.</param>
        /// <param name="data">Extra data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task LogErrorAsync(
            string? message = null,
            string? stackTrace = null,
            string? source = null,
            long[]? referenceIds = null,
            System.Collections.IDictionary? data = null,
            CancellationToken? cancellationToken = null)
        {
            await LogAsync(
                LogType.Error,
                message,
                stackTrace,
                source,
                referenceIds,
                data,
                cancellationToken);
        }

        /// <summary>
        /// Shorthand to log an Exception.
        /// </summary>
        /// <param name="exception">Exception to log.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task LogErrorAsync(
            Exception exception,
            CancellationToken? cancellationToken = null)
        {
            await LogErrorAsync(
                exception.Message,
                exception.StackTrace,
                exception.Source,
                null,
                exception.Data,
                cancellationToken);
        }

        /// <summary>
        /// Log a message as 'critical'.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="stackTrace">Possible stack-trace.</param>
        /// <param name="source">Possible source.</param>
        /// <param name="referenceIds">Possible reference IDs to external objects.</param>
        /// <param name="data">Extra data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task LogCriticalAsync(
            string? message = null,
            string? stackTrace = null,
            string? source = null,
            long[]? referenceIds = null,
            System.Collections.IDictionary? data = null,
            CancellationToken? cancellationToken = null)
        {
            await LogAsync(
                LogType.Critical,
                message,
                stackTrace,
                source,
                referenceIds,
                data,
                cancellationToken);
        }

        /// <summary>
        /// Shorthand to log an Exception.
        /// </summary>
        /// <param name="exception">Exception to log.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task LogCriticalAsync(
            Exception exception,
            CancellationToken? cancellationToken = null)
        {
            await LogCriticalAsync(
                exception.Message,
                exception.StackTrace,
                exception.Source,
                null,
                exception.Data,
                cancellationToken);
        }

        #endregion

        #region Save

        /// <summary>
        /// Save run-data to database.
        /// </summary>
        /// <param name="ctoken">Cancellation token.</param>
        /// <exception cref="Exception">Throw if TaskRun is not found in database.</exception>
        public async Task SaveAsync(CancellationToken? ctoken = null)
        {
            using var db = new EfContext();

            var run = await db.TaskRuns
                .FirstOrDefaultAsync(n => n.Id == Run.Id,
                                     ctoken ?? CancellationToken.None);

            if (run == null)
            {
                throw new Exception(
                    $"Unable to find run in database with identifier {Run.Id}");
            }

            // Save storage.
            if (DataStorage != null)
            {
                using var stream = new MemoryStream();

                await JsonSerializer.SerializeAsync(
                    stream,
                DataStorage,
                    cancellationToken: ctoken ?? CancellationToken.None);

                run.Data = Encoding.UTF8.GetString(
                    stream.ToArray());
            }

            // Update end-times.
            run.Finished = Run.Finished;
            run.RunTimeSeconds = Run.RunTimeSeconds;

            // Save.
            await db.SaveChangesAsync(
                ctoken ?? CancellationToken.None);
        }

        #endregion

        #region Info

        /// <summary>
        /// Get event-id for this run.
        /// </summary>
        /// <returns>Event id.</returns>
        public EventId GetEventId()
        {
            EventId ??= new EventId((int)Run.Id);

            return EventId.Value;
        }

        /// <summary>
        /// Calculate average time remaining based on items processed so far.
        /// </summary>
        /// <param name="totalItems">Total items.</param>
        /// <param name="processedItems">Current processed items.</param>
        /// <returns>Calculated time remaining.</returns>
        public TimeSpan CalcAverageTimeRemaining(
            long totalItems,
            long processedItems)
        {
            var current = DateTimeOffset.Now - Run.Started;
            var avg = current.TotalMilliseconds / processedItems;
            var rem = avg * (totalItems - processedItems);

            return TimeSpan.FromMilliseconds(rem);
        }

        #endregion

        #region Increment a counter

        /// <summary>
        /// Increment a set value in storage.
        /// </summary>
        /// <param name="dataKey">Key.</param>
        /// <param name="keyPrefix">Key prefix.</param>
        public void IncrementCounter(
            DataKey dataKey,
            string? keyPrefix = null)
        {
            var key = Enum.GetName(
                typeof(DataKey),
                dataKey);

            if (keyPrefix != null)
            {
                key = keyPrefix + "." + key;
            }

            if (key == null)
            {
                return;
            }

            IncrementCounter(key);
        }

        /// <summary>
        /// Increment a set value in storage.
        /// </summary>
        /// <param name="key">Key.</param>
        public void IncrementCounter(string key)
        {
            var value = GetData<long>(key);

            value++;

            SetData(key, value);
        }

        #endregion

        #region Decrease a counter

        /// <summary>
        /// Decrease a set value in storage.
        /// </summary>
        /// <param name="dataKey">Key.</param>
        /// <param name="keyPrefix">Key prefix.</param>
        public void DecreaseCounter(
            DataKey dataKey,
            string? keyPrefix = null)
        {
            var key = Enum.GetName(
                typeof(DataKey),
                dataKey);

            if (keyPrefix != null)
            {
                key = keyPrefix + "." + key;
            }

            if (key == null)
            {
                return;
            }

            DecreaseCounter(key);
        }

        /// <summary>
        /// Decrease a set value in storage.
        /// </summary>
        /// <param name="key">Key.</param>
        public void DecreaseCounter(string key)
        {
            var value = GetData<long>(key);

            value--;

            SetData(key, value);
        }

        #endregion

        #region Helper functions

        /// <summary>
        /// Wrapper to run a list of functions with interval and delays.
        /// </summary>
        /// <param name="name">Name of the run.</param>
        /// <param name="ctoken">Cancellation token.</param>
        /// <param name="delay">Delay before start, if any.</param>
        /// <param name="interval">Interval between runs, if any.</param>
        /// <param name="logger">Logging interface, if any.</param>
        /// <param name="functions">List of functions to run, in order.</param>
        public static async Task Setup(
            string name,
            CancellationToken? ctoken,
            TimeSpan? delay,
            TimeSpan? interval,
            ILogger? logger,
            params Func<TaskHelper, CancellationToken, Task>[] functions)
        {
            if (delay.HasValue)
            {
                logger?.LogWarning(
                    $"Waiting {delay.Value.ToHumanReadable(true)} before starting the first run.");

                await Task.Delay(
                    delay.Value,
                    ctoken ?? CancellationToken.None);
            }

            while (ctoken?.IsCancellationRequested != true)
            {
                var run = await TaskRun.CreateNewAsync(
                    name,
                    ctoken);

                var eventId = new EventId((int)run.Id);
                var helper = new TaskHelper(run);

                logger?.LogInformation(
                    eventId,
                    "Starting a new run.");

                try
                {
                    foreach (var function in functions)
                    {
                        logger?.LogInformation(
                            eventId,
                            $"Running function \"{function.Method.Name}\".");

                        var start = DateTimeOffset.Now;

                        await function.Invoke(
                            helper,
                            ctoken ?? CancellationToken.None);

                        var end = DateTimeOffset.Now;
                        var duration = end - start;

                        logger?.LogInformation(
                            eventId,
                            $"Finished running function \"{function.Method.Name}\" which took {duration.ToHumanReadable(true)}.");

                        await helper.LogInformationAsync(
                            $"Finished running function \"{function.Method.Name}\" which took {duration.ToHumanReadable(true)}.",
                            cancellationToken: ctoken);
                    }
                }
                catch (Exception ex)
                {
                    logger?.LogError(
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

                if (interval.HasValue)
                {
                    logger?.LogInformation(
                        eventId,
                        $"Waiting {interval.Value.ToHumanReadable(true)} before re-running.");

                    await Task.Delay(
                        interval.Value,
                        ctoken ?? CancellationToken.None);
                }
            }

            logger?.LogInformation(
                "Cancellation requested. Shutting down.");
        }

        #endregion
    }
}