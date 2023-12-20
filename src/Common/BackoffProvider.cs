using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipsideCrypto.NET.Common;
public class BackoffProvider
{
    public static IEnumerable<TimeSpan> JitteredCappedExponentialBackoff(TimeSpan baseRetryDelay, double scaleFactor = 1.1, 
        TimeSpan? backoffCap = null, TimeSpan? maxJitter = null, int? seed = null)
    {
        var random = seed.HasValue ? new Random(seed.Value) : new Random();

        backoffCap ??= TimeSpan.FromSeconds(10);
        maxJitter ??= TimeSpan.FromSeconds(2);

        double factor = 1;

        while (true)
        {
            long durationTicks = Math.Min((long) (baseRetryDelay.Ticks * factor), backoffCap.Value.Ticks);
            long jitter = (long) (((random.NextDouble() * 2) - 1) * Math.Min(maxJitter.Value.Ticks, durationTicks / 8));

            yield return TimeSpan.FromTicks(durationTicks + jitter);

            factor *= scaleFactor;
        }
    }
}
