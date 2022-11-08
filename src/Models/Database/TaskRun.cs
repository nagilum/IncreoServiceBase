using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Increo.ServiceBase.Models.Database
{
    [Table("_ISB_TaskRuns")]
    internal class TaskRun
    {
        #region SQL table mapping

        [Key]
        [Column]
        public long Id { get; set; }

        [Column]
        [MaxLength(64)]
        public string MachineName { get; set; } = null!;

        [Column]
        [MaxLength(64)]
        public string Name { get; set; } = null!;

        [Column]
        public DateTimeOffset Started { get; set; }

        [Column]
        public DateTimeOffset? Finished { get; set; }

        [Column]
        public int? RunTimeSeconds { get; set; }

        [Column]
        public string? Data { get; set; }

        #endregion

        #region Helper functions

        /// <summary>
        /// Create a new run.
        /// </summary>
        /// <param name="name">Name of the run.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>New run.</returns>
        public static async Task<TaskRun> CreateNewAsync(
            string name,
            CancellationToken? cancellationToken)
        {
            using var db = new EfContext();

            var run = new TaskRun
            {
                Name = name,
                MachineName = Environment.MachineName,
                Started = DateTimeOffset.Now
            };

            await db.TaskRuns.AddAsync(run, cancellationToken ?? CancellationToken.None);
            await db.SaveChangesAsync(cancellationToken ?? CancellationToken.None);

            return run;
        }

        #endregion
    }
}