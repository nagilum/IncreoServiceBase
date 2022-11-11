namespace Increo.ServiceBase
{
    internal static class Config
    {
        /// <summary>
        /// Host context.
        /// </summary>
        private static HostBuilderContext? ConfigContext { get; set; }

        /// <summary>
        /// Set host context for later use.
        /// </summary>
        /// <param name="hostContext">Host context.</param>
        public static void SetContext(HostBuilderContext hostContext)
        {
            ConfigContext = hostContext;
        }

        /// <summary>
        /// Get a section from app settings.
        /// </summary>
        /// <typeparam name="T">Type to cast as.</typeparam>
        /// <param name="keys">Keys to find it under.</param>
        /// <returns>Section.</returns>
        /// <exception cref="Exception">Thrown if host context hasn't been set.</exception>
        public static T GetSection<T>(params string[] keys)
        {
            if (ConfigContext == null)
            {
                throw new Exception(
                    "HostBuilderContext has not been set. " +
                    "Use the SetContext() function in ConfigureServices() to set it.");
            }

            if (keys.Length == 0)
            {
                return ConfigContext.Configuration
                    .Get<T>();
            }
            else if (keys.Length == 1)
            {
                return ConfigContext.Configuration
                    .GetSection(keys[0])
                    .Get<T>();
            }
            else
            {
                IConfigurationSection section = null!;

                for (var i = 0; i < keys.Length; i++)
                {
                    if (i == 0)
                    {
                        section = ConfigContext.Configuration
                            .GetSection(keys[0]);
                    }
                    else if (i == keys.Length - 1)
                    {
                        return section
                            .GetSection(keys[i])
                            .Get<T>();
                    }
                    else
                    {
                        section = section
                            .GetSection(keys[i]);
                    }
                }
            }

            return default!;
        }

        /// <summary>
        /// Get a value from app settings.
        /// </summary>
        /// <typeparam name="T">Type to cast as.</typeparam>
        /// <param name="keys">Keys to find it under.</param>
        /// <returns>Value.</returns>
        /// <exception cref="Exception">Thrown if host context hasn't been set or no keys specified.</exception>
        public static T GetValue<T>(params string[] keys)
        {
            if (ConfigContext == null)
            {
                throw new Exception(
                    "HostBuilderContext has not been set. " +
                    "Use the SetContext() function in ConfigureServices() to set it.");
            }

            if (keys.Length == 0)
            {
                throw new Exception(
                    "You have to specify at least one key.");
            }

            if (keys.Length == 1)
            {
                return ConfigContext.Configuration
                    .GetValue<T>(keys[0]);
            }

            IConfigurationSection section = null!;

            for (var i = 0; i < keys.Length; i++)
            {
                if (i == keys.Length - 1)
                {
                    return section.GetValue<T>(keys[i]);
                }
                else
                {
                    section = section == null
                        ? ConfigContext.Configuration.GetSection(keys[i])
                        : section.GetSection(keys[i]);

                    if (section == null)
                    {
                        throw new Exception(
                            $"Unable to find config section {keys[i]}");
                    }
                }
            }

            return default!;
        }
    }
}