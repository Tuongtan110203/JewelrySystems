namespace WebBanVang.Models.Domain
{
    public class TimeHelper
    {
        public static DateTime GetCurrentTimeInTimeZone(string timeZoneId)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
        }
    }
}
