using Increo.ServiceBase.Attributes;

namespace Increo.ServiceBase
{
    /// <summary>
    /// Types of logs.
    /// </summary>
    public enum LogType
    {
        [LogTypeDatabaseValue("trace")]
        Trace,

        [LogTypeDatabaseValue("debug")]
        Debug,

        [LogTypeDatabaseValue("info")]
        Information,

        [LogTypeDatabaseValue("warn")]
        Warning,

        [LogTypeDatabaseValue("error")]
        Error,

        [LogTypeDatabaseValue("crit")]
        Critical
    }

    /// <summary>
    /// Types of data.
    /// </summary>
    public enum DataKey
    {
        Total,
        Processed,
        Created,
        Updated,
        Deleted
    }
}