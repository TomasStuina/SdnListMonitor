using System;

namespace SdnListMonitor.Core.Abstractions.Extensions
{
    public static class ObjectExtensions
    {
        public static T ThrowIfNull<T> (this T instance, string paramName)
            where T : class
        {
            return instance ?? throw new ArgumentNullException (paramName);
        }
    }
}
