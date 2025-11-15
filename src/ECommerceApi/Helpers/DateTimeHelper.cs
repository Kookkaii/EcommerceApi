namespace ECommerceApi.Helpers
{
    public static class DateTimeHelper
    {
        private static readonly TimeZoneInfo BangkokTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Bangkok");
        public static DateTime ToBangkokTime(this DateTime utcDateTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, BangkokTimeZone);
        }
    }
}