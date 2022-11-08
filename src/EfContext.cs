using Increo.ServiceBase.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace Increo.ServiceBase
{
    partial class EfContext : DbContext
    {
        /// <summary>
        /// MSSQL database config.
        /// </summary>
        private MsSqlDatabaseConfig? DatabaseConfig { get; set; }

        #region Constructors

        /// <summary>
        /// Init a new Entity Framework context.
        /// </summary>
        /// <param name="connectionName">Load connection based on name.</param>
        public EfContext(string connectionName = "IncreoDb")
        {
            DatabaseConfig = Config.GetSection<MsSqlDatabaseConfig>(
                "IncreoServiceBase",
                "DatabaseConnections",
                connectionName);
        }

        /// <summary>
        /// Init a new Entity Framework context.
        /// </summary>
        /// <param name="databaseConfig">Connection config.</param>
        public EfContext(MsSqlDatabaseConfig databaseConfig)
        {
            DatabaseConfig = databaseConfig;
        }

        #endregion

        #region Configuration

        /// <summary>
        /// Configure the connection.
        /// </summary>
        /// <param name="builder">Database builder context.</param>
        /// <exception cref="Exception">Thrown if the selected database config can't produce a connection string.</exception>
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            var cstr = this.DatabaseConfig?.ToString();

            if (cstr == null)
            {
                throw new Exception(
                    "Connectionstring for SQL server cannot be empty.");
            }

            builder.UseSqlServer(cstr);
        }

        #endregion

        #region Datasets

        public DbSet<TaskRun> TaskRuns { get; set; } = null!;

        public DbSet<TaskRunLog> TaskRunLogs { get; set; } = null!;

        #endregion
    }
}