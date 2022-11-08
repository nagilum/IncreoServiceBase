using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Increo.ServiceBase.Models.Database
{
    [Table("_ISB_TaskRunLogs")]
    internal class TaskRunLog
    {
        #region SQL table mapping

        [Key]
        [Column]
        public long Id { get; set; }

        [Column]
        public long RunId { get; set; }

        [Column]
        public DateTimeOffset Created { get; set; }

        [Column]
        [MaxLength(5)]
        public string LogType { get; set; } = null!;

        [Column]
        public string? Message { get; set; }

        [Column]
        public string? StackTrace { get; set; }

        [Column]
        public string? Source { get; set; }

        [Column]
        public string? ReferenceIds { get; set; }

        [Column]
        public string? Data { get; set; }

        #endregion
    }
}