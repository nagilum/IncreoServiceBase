namespace Increo.ServiceBase.Extenders
{
    internal static class TimeSpanExtender
    {
        /// <summary>
        /// Format a TimeSpan into a more human readable output.
        /// </summary>
        /// <param name="ts">TimeSpan.</param>
        /// <param name="includeNormalToString">Whether to include the normal ToString output.</param>
        /// <returns>Human readable format.</returns>
        public static string ToHumanReadable(
            this TimeSpan ts,
            bool includeNormalToString = false)
        {
            var parts = new List<string>();
            var seconds = ts.TotalSeconds;

            int factor;

            const double secondsInMinute = 60;
            const double secondsInHour = 3600;
            const double secondsInDay = 86400;

            if (seconds > secondsInDay)
            {
                factor = (int)Math.Floor(seconds / secondsInDay);

                parts.Add($"{factor} days");
                seconds -= secondsInDay * factor;
            }

            if (seconds > secondsInHour)
            {
                factor = (int)Math.Floor(seconds / secondsInHour);

                parts.Add($"{factor} hours");
                seconds -= secondsInHour * factor;
            }

            if (seconds > secondsInMinute)
            {
                factor = (int)Math.Floor(seconds / secondsInMinute);

                parts.Add($"{factor} minutes");
                seconds -= secondsInMinute * factor;
            }

            seconds = (int)seconds;

            if (seconds > 0 ||
                parts.Count == 0)
            {
                parts.Add($"{seconds} seconds");
            }

            string output;

            if (parts.Count > 1)
            {
                output = string.Join(", ", parts.Take(parts.Count - 1));
                output += " and " + parts.Last();
            }
            else
            {
                output = parts[0];
            }

            if (includeNormalToString)
            {
                output += $" ({ts})";
            }

            return output;
        }
    }
}