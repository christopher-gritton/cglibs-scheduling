using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace CGLibs.Scheduling
{
    [DataContract, Serializable()]
    public class ScheduleTimeZone 
    {

        public ScheduleTimeZone() 
        {
            var t = TimeZoneInfo.Local;
            TimeZoneId = t.Id;
            TimeZoneDisplay = t.StandardName;
        }

        protected void D(string debugmessage)
        {
            System.Diagnostics.Debug.WriteLine(debugmessage);
        }

        protected void E(Exception ex)
        {
            System.Diagnostics.Trace.TraceError(ex.Message + "\n" + ex.StackTrace);
        }


        [DataMember]
        public string TimeZoneId { get; set; }
        [DataMember]
        public string TimeZoneDisplay { get; set; }

        public string GetAbbrevTzName(DateTime runtime)
        {
            var tzinfo = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
            return tzinfo.GetTzAbbreviation(runtime);
        }

        public string GetSchedTzName(DateTime runtime)
        {
            var tzinfo = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
            return tzinfo.GetTzCorrected(runtime);
        }

        public List<ScheduleTimeZone> GetAvailableTimeZones()
        {
            //list of timezones
            List<ScheduleTimeZone> tzs = new List<ScheduleTimeZone>();

            try
            {
                //get system timezones
                var systzs = TimeZoneInfo.GetSystemTimeZones().ToList();
                //iterate system timezones
                foreach (TimeZoneInfo tzinfo in systzs)
                {
                    //add new ScheduleTimeZone class to list
                    tzs.Add(new ScheduleTimeZone() { TimeZoneId = tzinfo.Id, TimeZoneDisplay = tzinfo.StandardName });
                }

            }
            catch (Exception ex)
            {
                E(ex);
            }

            return tzs;
        }

        public override string ToString()
        {
            return TimeZoneDisplay;
        }
    }
}
