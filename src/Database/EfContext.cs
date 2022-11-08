using Increo.ServiceBase.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Increo.ServiceBase.Database
{
    partial class EfContext : DbContext
    {
        public ConfigEntry.ConfigDatabaseEntry? DatabaseConfig { get; set; }

        #region Constructors

        /// <summary>
        /// Init a new Entity Framework context and load IncreoDb conntection, if found, or first, if possible.
        /// </summary>
        /// <exception cref="Exception">Thrown if an SQL connection string cannot be built.</exception>
        public EfContext()
        {
            if (Config.Loaded?.Databases?.Count > 0 &&
                Config.Loaded.Databases.ContainsKey("IncreoDb"))
            {
                this.DatabaseConfig = Config.Loaded.Databases["IncreoDb"];
                return;
            }

            var item = Config.Loaded?.Databases?.FirstOrDefault();

            if (item?.Value == null)
            {
                return;
            }

            if (item.Value.Value.ToString() == null)
            {
                throw new Exception(
                    $"Unable to build SQL connection string from appsettings database entry '{item}'.");
            }

            this.DatabaseConfig = item.Value.Value;
        }

        /// <summary>
        /// Init a new Entity Framework context.
        /// </summary>
        /// <param name="connectionName">Load connection based on name.</param>
        /// <exception cref="Exception">Thrown if connection wasn't found based on name.</exception>
        public EfContext(string connectionName)
        {
            if (Config.Loaded?.Databases?.Count > 0 &&
                Config.Loaded.Databases.ContainsKey(connectionName))
            {
                this.DatabaseConfig = Config.Loaded.Databases[connectionName];
            }
            else
            {
                throw new Exception(
                    $"Database connection '{connectionName}' not found in appsettings.");
            }
        }

        /// <summary>
        /// Init a new Entity Framework context.
        /// </summary>
        /// <param name="databaseConfig">Connection config.</param>
        public EfContext(ConfigEntry.ConfigDatabaseEntry databaseConfig)
        {
            this.DatabaseConfig = databaseConfig;
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