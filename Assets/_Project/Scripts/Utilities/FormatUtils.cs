using System.Globalization;
using Utilities.Constants;

namespace Utilities
{
    public static class FormatUtils
    {
        #if TEST
        public static string FormatLatency(long milliseconds)
        {
            if (milliseconds < 0) return UIMessages.Formatting.TimingNotAvailable;

            return string.Format(UIMessages.Formatting.LatencyFormat, milliseconds, CultureInfo.InvariantCulture);
        }
        #endif

        // You can add anither formating methods here (dates, numbers etc).
        // public static string FormatDate(DateTime date) { ... }
    }
}