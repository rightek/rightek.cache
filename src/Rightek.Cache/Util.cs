using System;

namespace Rightek.Cache
{
    public static class Util
    {
        internal static void ThrowIfNull<T>(this T o, string paramName)
        {
            if (o is null) throw new ArgumentNullException(paramName);
        }

        internal static void ThrowIfNullOrEmpty(this string s, string paramName)
        {
            if (string.IsNullOrWhiteSpace(s)) throw new ArgumentNullException(paramName);
        }
    }
}