using System;
namespace Core.Helpers
{
    public static class DateHelper
    {
        public static DateTime GetParsedDateOrDefault(string dateString, DateTime defaultDate)
        {
            var date = defaultDate.Date;
            if (!string.IsNullOrEmpty(dateString))
            {
                if (DateTime.TryParseExact(dateString,
                           "yyyy-MM-dd",
                           System.Globalization.CultureInfo.InvariantCulture,
                           System.Globalization.DateTimeStyles.None,
                           out var parsedDate))
                {
                    date = parsedDate;
                }
            }
            return date;
        }
    }
}
