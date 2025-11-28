using System;
using System.Linq;

namespace YouTubeCLI.Commands
{
    public enum BroadcastFilter
    {
        All,
        Upcoming,
        Active,
        Completed
    }

    public static class BroadcastFilterExtensions
    {
        public static string ToApiString(this BroadcastFilter filter)
        {
            return filter switch
            {
                BroadcastFilter.All => "all",
                BroadcastFilter.Upcoming => "upcoming",
                BroadcastFilter.Active => "active",
                BroadcastFilter.Completed => "completed",
                _ => "all"
            };
        }

        public static BroadcastFilter FromString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return BroadcastFilter.All;

            var normalized = value.Trim().ToLowerInvariant();
            return normalized switch
            {
                "all" => BroadcastFilter.All,
                "upcoming" => BroadcastFilter.Upcoming,
                "active" => BroadcastFilter.Active,
                "completed" => BroadcastFilter.Completed,
                _ => BroadcastFilter.All
            };
        }
    }
}

