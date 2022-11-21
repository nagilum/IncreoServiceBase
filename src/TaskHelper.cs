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
        #region Properties

        /// <summary>
        /// When the last function was started.
        /// </summary>
        public DateTimeOffset? FunctionStart { get; set; }

        /// <summary>
        /// Internal storage while running.
        /// </summary>
        public Dictionary<string, object>? DataStorage { get; set; }

        /// <summary>
        /// Local storage of the event ID.
        /// </summary>
        private EventId? EventId { get; set; }

        /// <summary>
        /// Run identificator.
        /// </summary>
        public TaskRun Run { get; set; }

        #endregion

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
            var current = DateTimeOffset.Now - (this.FunctionStart ?? Run.Started);
            var avg = current.TotalMilliseconds / processedItems;
            var rem = avg * (totalItems - processedItems);

            return TimeSpan.FromMilliseconds(rem);
        }

        /// <summary>
        /// Set function start.
        /// </summary>
        /// <param name="start">Start.</param>
        public void SetFunctionStart(DateTimeOffset? start = null)
        {
            this.FunctionStart = start ?? DateTimeOffset.Now;
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

        #region Decrement a counter

        /// <summary>
        /// Decrease a set value in storage.
        /// </summary>
        /// <param name="dataKey">Key.</param>
        /// <param name="keyPrefix">Key prefix.</param>
        public void DecrementCounter(
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

            DecrementCounter(key);
        }

        /// <summary>
        /// Decrease a set value in storage.
        /// </summary>
        /// <param name="key">Key.</param>
        public void DecrementCounter(string key)
        {
            var value = GetData<long>(key);

            value--;

            SetData(key, value);
        }

        #endregion
    }
}