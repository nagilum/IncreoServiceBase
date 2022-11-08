namespace Increo.ServiceBase
{
    internal static class Config
    {
        /// <summary>
        /// Loaded config.
        /// </summary>
        public static ConfigEntry Loaded { get; set; } = new();

        /// <summary>
        /// Load config from app-settings.
        /// </summary>
        /// <param name="hostContext">Host builder context.</param>
        public static void Load(HostBuilderContext hostContext)
        {
            Loaded = hostContext.Configuration
                .GetSection("IncreoServiceBase")
                .Get<ConfigEntry>();
        }
    }

    internal class ConfigEntry
    {
        public Dictionary<string, object>? Custom { get; set; }

        public Dictionary<string, ConfigDatabaseEntry>? Databases { get; set; }

        internal class ConfigDatabaseEntry
        {
            public string? DataSource { get; set; }

            public string? InitialCatalog { get; set; }

            public string? Server { get; set; }

            public string? Database { get; set; }

            public string? UserId { get; set; }

            public string? Password { get; set; }

            public string? NetworkLibrary { get; set; }

            public string? AttachDbFilename { get; set; }

            public bool? MultipleActiveResultSets { get; set; }

            public bool? TrustedConnection { get; set; }

            public override string ToString()
            {
                var dict = new Dictionary<string, string>();

                if (this.DataSource != null)
                {
                    dict.Add("Data Source", this.DataSource);
                }

                if (this.InitialCatalog != null)
                {
                    dict.Add("Initial Catalog", this.InitialCatalog);
                }

                if (this.Server != null)
                {
                    dict.Add("Server", this.Server);
                }

                if (this.Database != null)
                {
                    dict.Add("Database", this.Database);
                }

                if (this.UserId != null)
                {
                    dict.Add("User Id", this.UserId);
                }

                if (this.Password != null)
                {
                    dict.Add("Password", this.Password);
                }

                if (this.NetworkLibrary != null)
                {
                    dict.Add("Network Library", this.NetworkLibrary);
                }

                if (this.AttachDbFilename != null)
                {
                    dict.Add("AttachDbFilename", this.AttachDbFilename);
                }

                if (this.MultipleActiveResultSets is bool mars)
                {
                    dict.Add("MultipleActiveResultSets", mars.ToString());
                }

                if (this.TrustedConnection is bool tc)
                {
                    dict.Add("Trusted Connection", tc.ToString());
                }

                return string.Join(
                    ";",
                    dict.Select(n => $"{n.Key}={n.Value}"));
            }
        }
    }
}