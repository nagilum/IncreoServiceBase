namespace Increo.ServiceBase.Extenders
{
    internal static class EnumExtender
    {
        /// <summary>
        /// Get attribute value.
        /// </summary>
        /// <typeparam name="T">Attribute type.</typeparam>
        /// <param name="value">Value var.</param>
        /// <returns>Attribute, if found.</returns>
        public static T? GetAttribute<T>(this Enum value) where T : Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);

            if (name == null)
            {
                return default;
            }

            return type
                .GetField(name)?
                .GetCustomAttributes(false)
                .OfType<T>()
                .FirstOrDefault();
        }
    }
}