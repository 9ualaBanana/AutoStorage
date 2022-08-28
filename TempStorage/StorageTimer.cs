using System.Diagnostics.CodeAnalysis;
using System.Timers;

namespace System.Collections.Specialized;

internal static class StorageTimer
{
    internal const GTimer? Unlimited = null;

    internal static bool IsUnlimited([NotNullWhen(false)] GTimer? storageTimer) => storageTimer is Unlimited;

    internal static GTimer Initialize(this GTimer timer, params ElapsedEventHandler[] with)
    {
        if (!with.Any()) throw new MissingMethodException(
            $"Storage timer must be initialized with {nameof(ElapsedEventHandler)} responsible for removing items with elapsed storage time.");

        foreach (var elapsedEventHandler in with)
            timer.Elapsed += elapsedEventHandler;
        timer.AutoReset = false;
        timer.Start();
        return timer;
    }
}
