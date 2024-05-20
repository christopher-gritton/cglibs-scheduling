using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CGLibs.Scheduling
{
    [DataContract]
    public enum ScheduleDay
    {
        [EnumMember]
        Sunday = 0,
        [EnumMember]
        Monday = 1,
        [EnumMember]
        Tuesday = 2,
        [EnumMember]
        Wednesday = 3,
        [EnumMember]
        Thursday = 4,
        [EnumMember]
        Friday = 5,
        [EnumMember]
        Saturday = 6
    }

    [DataContract, Serializable()]
    public class WeeklySchedule : Schedule
    {

        public WeeklySchedule() : base()
        {
            SchedulesRecurrentType = Recurrent.Weekly;
            IsScheduledDay = ScheduleDay.Sunday;
        }

        //recurrent type property
        [XmlIgnore(), IgnoreDataMember()]
        public override Recurrent SchedulesRecurrentType { get { return Recurrent.Weekly; } internal set { } }
        //schedule event
        public override event ScheduleHandler OnScheduleTriggered;
        public override event ScheduleStatusHandler OnScheduleStatusUpdate;

        [NonSerialized]
        private System.Threading.CancellationTokenSource ts;

        public override DateTime GetNextRunTime()
        {
            return NextScheduledRun(0);
        }

        private DateTime NextScheduledRun(int padminutes = 0)
        {
            try
            {

                //current date time
                DateTime utc = DateTime.UtcNow;
                utc = new DateTime(utc.Year, utc.Month, utc.Day, utc.Hour, utc.Minute, 0);
                //get current day of week for allocated timezone
                TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(Timezone.TimeZoneId);
                //convert to datetime in selected timezone
                DateTime tzTime = TimeZoneInfo.ConvertTimeFromUtc(utc, tz);

                //check if start time is in future
                if (tzTime > StartTime && !IsRecurring || !Enabled)
                {
                    //return minval since we will not run again
                    return DateTime.MinValue;
                }

                if (!IsRecurring || !Enabled)
                {
                    //returning the scheduled run time (single datetime in future)
                    return StartTime;
                }

                //cycle while StartTime not in future
                DateTime nextrun = new DateTime(StartTime.Year, StartTime.Month, StartTime.Day, StartTime.Hour, StartTime.Minute, 0);

                //calculate next run
                do
                {
                    bool isvalidruntime = false;
                    if (tzTime <= nextrun)
                    {
                        //check if day of week is scheduled
                        //get day of week
                        DayOfWeek dayofweek = nextrun.DayOfWeek;

                        if (nextrun.Day == StartTime.Day && nextrun.Month == StartTime.Month && nextrun.Year == StartTime.Year) isvalidruntime = true;

                        switch (dayofweek)
                        {
                            case DayOfWeek.Sunday:
                                if (IsScheduledDay == ScheduleDay.Sunday)
                                {
                                    isvalidruntime = true;
                                }
                                break;
                            case DayOfWeek.Monday:
                                if (IsScheduledDay == ScheduleDay.Monday)
                                {
                                    isvalidruntime = true;
                                }
                                break;
                            case DayOfWeek.Tuesday:
                                if (IsScheduledDay == ScheduleDay.Tuesday)
                                {
                                    isvalidruntime = true;
                                }
                                break;
                            case DayOfWeek.Wednesday:
                                if (IsScheduledDay == ScheduleDay.Wednesday)
                                {
                                    isvalidruntime = true;
                                }
                                break;
                            case DayOfWeek.Thursday:
                                if (IsScheduledDay == ScheduleDay.Thursday)
                                {
                                    isvalidruntime = true;
                                }
                                break;
                            case DayOfWeek.Friday:
                                if (IsScheduledDay == ScheduleDay.Friday)
                                {
                                    isvalidruntime = true;
                                }
                                break;
                            case DayOfWeek.Saturday:
                                if (IsScheduledDay == ScheduleDay.Saturday)
                                {
                                    isvalidruntime = true;
                                }
                                break;
                            default:

                                break;
                        }
                    }

                    if (isvalidruntime == true) break;

                    //move up a day
                    nextrun = nextrun.AddDays(1);

                } while (true);

                //return nextrun datetime
                return nextrun;
            }
            catch (Exception ex)
            {
                E(ex);
            }

            return DateTime.MinValue; //default value if failure occurs (Indicates no scheduled run in future)
        }

        public override void StartScheduler()
        {
            try
            {
                D("Start schedule called");
                StopSchedule(); //make sure current schedule is stopped

                ts = new System.Threading.CancellationTokenSource(); //reset cancellation token
                var schedTask = new Task(() => Scheduling(), ts.Token, TaskCreationOptions.LongRunning); //create long running task
                schedTask.Start(); //start task
            }
            catch (Exception ex)
            {
                E(ex);
            }
        }

        public override void StopSchedule()
        {
            try
            {
                D("Stop schedule called");
                if (ts != null)
                {
                    ts.Cancel();
                }
            }
            catch (Exception ex)
            {
                E(ex);
            }
        }

        private void Scheduling()
        {
            D("Scheduler running");
            DateTime lastnotify = DateTime.MinValue;
            do
            {
                try
                {
                    //check if task is canceled
                    ts.Token.ThrowIfCancellationRequested();
                    //get next runtime
                    DateTime runtime = GetNextRunTime();
                    DateTime now = DateTime.UtcNow;
                    //translate current time to assigned timezone
                    now = TimeZoneInfo.ConvertTimeFromUtc(now, TimeZoneInfo.FindSystemTimeZoneById(Timezone.TimeZoneId));

                    //check if we should trigger
                    double totalminutes = (double)now.Subtract(runtime).TotalMinutes;
                    if (((int)Math.Floor(now.Subtract(runtime).TotalMinutes)) == 0)
                    {
                        I("Schedule triggered at " + now.ToString());
                        if (OnScheduleTriggered != null)
                        {
                            OnScheduleTriggered(new ScheduleTriggeredEventArgs(this, TaskId, now));
                            if (IsRecurring)
                            {
                                OnScheduleStatusUpdate(new ScheduleStatusEventArgs(this, TaskId, NextScheduledRun(1)));
                            }
                            lastnotify = (lastnotify == DateTime.MinValue ? DateTime.Now.AddMinutes(-1) : lastnotify.AddMinutes(-1));
                        }

                        //wait for minute to elapse so we don't trigger multiple times
                        do
                        {
                            //update now
                            now = DateTime.UtcNow;
                            //translate current time to assigned timezone
                            now = TimeZoneInfo.ConvertTimeFromUtc(now, TimeZoneInfo.FindSystemTimeZoneById(Timezone.TimeZoneId));
                            //sleep
                            System.Threading.Thread.Sleep(500);
                            //check if task is canceled
                            ts.Token.ThrowIfCancellationRequested();

                        } while (((int)Math.Floor(now.Subtract(runtime).TotalMinutes)) == 0);
                    }

                    //update schedule status
                    if (OnScheduleStatusUpdate != null)
                    {
                        if (DateTime.Now.Subtract(lastnotify).TotalSeconds > 30)
                        {
                            //get next runtime
                            runtime = GetNextRunTime();
                            OnScheduleStatusUpdate(new ScheduleStatusEventArgs(this, TaskId, runtime));
                            lastnotify = DateTime.Now;
                        }
                    }

                    //sleep
                    System.Threading.Thread.Sleep(500);
                }
                catch (OperationCanceledException oex)
                {
                    D(string.Format("Scheduling canceled : {0}", oex.Message));
                    break;
                }
                catch (Exception ex)
                {
                    E(ex);
                }
            }
            while (true);
        }

        protected void Dispose(bool disposing)
        {
            D("Disposing of Schedule");
            if (disposing)
            {
                StopSchedule();

            }
            IsDisposed = true;
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [DataMember]
        public ScheduleDay IsScheduledDay { get; set; }

        [DataMember]
        public override bool IsRecurring { get; set; }

    }
}
