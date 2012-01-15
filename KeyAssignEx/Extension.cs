using System.Collections.Generic;

namespace KeyAssignEx
{
    static class Extension
    {
        public static IEnumerable<int> EnumerateIndex(this string source, string value)
        {
            var index = -1;
            while ((index = source.IndexOf(value, index + 1)) != -1)
            {
                yield return index;
            }
        }

        public static string Join(this IEnumerable<string> source, string separator)
        {
            return string.Join(separator, source);
        }

        public static T Case<T>(this T source, T @case, T value)
        {
            return source.Equals(@case) ? value : source;
        }
    }
}
