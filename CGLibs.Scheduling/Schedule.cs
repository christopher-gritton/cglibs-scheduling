using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace CGLibs.Scheduling
{
    //eventhandler for schedule triggered
    public delegate void ScheduleHandler(ScheduleTriggeredEventArgs e);
    //schedule status update handler
    public delegate void ScheduleStatusHandler(ScheduleStatusEventArgs e);

    [DataContract, Serializable(),
        KnownType(typeof(MonthlySchedule)),
        KnownType(typeof(WeeklySchedule)),
        KnownType(typeof(DailySchedule)),
        KnownType(typeof(HourlySchedule)),
        KnownType(typeof(MinutelySchedule)),
        XmlInclude(typeof(MonthlySchedule)),
        XmlInclude(typeof(WeeklySchedule)),
        XmlInclude(typeof(DailySchedule)),
        XmlInclude(typeof(HourlySchedule)),
        XmlInclude(typeof(MinutelySchedule))]
    public abstract class Schedule : IDisposable
    {

        public Schedule()
        {
            Timezone = new ScheduleTimeZone();
            StartTime = DateTime.UtcNow;
        }

        protected void D(string debugmessage)
        {
            System.Diagnostics.Debug.WriteLine(debugmessage);
        }

        protected void I(string infomessage)
        {
            System.Diagnostics.Trace.TraceInformation(infomessage);
        }

        protected void W(string warnmessage)
        {
            System.Diagnostics.Trace.TraceWarning(warnmessage);
        }

        protected void E(Exception ex)
        {
            System.Diagnostics.Trace.TraceError(ex.Message + "\n" + ex.StackTrace);
        }

        //start schedule
        public abstract void StartScheduler();
        //stop schedule
        public abstract void StopSchedule();
        //schedule type
        [XmlIgnore(), IgnoreDataMember()]
        public abstract Recurrent SchedulesRecurrentType { get; internal set; }
        //events
        public abstract event ScheduleHandler OnScheduleTriggered;
        public abstract event ScheduleStatusHandler OnScheduleStatusUpdate;
        //get next runtime
        public abstract DateTime GetNextRunTime();
        //disposal
        public abstract void Dispose();
        //enabled
        [DataMember]
        public bool Enabled { get; set; }

        //recurring
        [DataMember]
        public abstract bool IsRecurring { get; set; }

        //start time
        [DataMember]
        public virtual DateTime StartTime
        {
            get
            {
                //get corrected value with 0 seconds if datetime was not set use current datetime using assigned timezone
                if (startTime == DateTime.MinValue)
                {
                    DateTime ntime = DateTime.UtcNow;
                    TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(Timezone.TimeZoneId);
                    ntime = TimeZoneInfo.ConvertTimeFromUtc(ntime, tz);
                    startTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, startTime.Hour, startTime.Minute, 0);
                }
                return startTime;
            }
            set
            {
                //set value with corrected to 0 seconds - assuming time zone is correct
                startTime = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, 0);
            }
        }
        private DateTime startTime = DateTime.MinValue;

        //time zone
        [DataMember]
        public ScheduleTimeZone Timezone { get; set; }

        [IgnoreDataMember, XmlIgnore()]
        public bool IsDisposed { get; protected set; }

        [IgnoreDataMember, XmlIgnore()]
        public string TaskId { get; set; }

    }
}
