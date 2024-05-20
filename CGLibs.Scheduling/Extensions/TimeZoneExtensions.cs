using System;

namespace CGLibs.Scheduling
{
    internal static class TimeZoneExtension
    {

        public static string GetTzAbbreviation(this TimeZoneInfo tz, DateTime timetocheck)
        {

            string timeZoneName = tz.Id;

            if (tz.SupportsDaylightSavingTime && tz.IsDaylightSavingTime(timetocheck))
            {
                timeZoneName = tz.DaylightName;
            }

            string output = string.Empty;

            string[] timeZoneWords = timeZoneName.Split(' ');
            foreach (string timeZoneWord in timeZoneWords)
            {
                if (timeZoneWord[0] != '(')
                {
                    output += timeZoneWord[0];
                }
                else
                {
                    output += timeZoneWord;
                }
            }
            return output;
        }

        public static string GetTzCorrected(this TimeZoneInfo tz, DateTime timetocheck)
        {

            string timeZoneName = tz.StandardName;

            if (tz.SupportsDaylightSavingTime && tz.IsDaylightSavingTime(timetocheck))
            {
                timeZoneName = tz.DaylightName;
            }

            return timeZoneName;
        }

    }
}
