namespace Increo.ServiceBase.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    internal class LogTypeDatabaseValueAttribute : Attribute
    {
        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Add shortened name, for database value.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <exception cref="Exception">Throw if name is longer than 5 characters.0</exception>
        public LogTypeDatabaseValueAttribute(string name)
        {
            if (name?.Length > 5 ||
                string.IsNullOrWhiteSpace(name))
            {
                throw new Exception(
                    "Parameter 'name' can only be up to 5 characters long and cannot be blank.");
            }

            this.Name = name!;
        }
    }
}