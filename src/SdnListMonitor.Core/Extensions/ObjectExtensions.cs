using System;

namespace SdnListMonitor.Core.Extensions
{
    internal static class ObjectExtensions
    {
        public static T ThrowIfNull<T> (this T instance, string paramName)
            where T : class
        {
            return instance ?? throw new ArgumentNullException (paramName);
        }
    }
}
