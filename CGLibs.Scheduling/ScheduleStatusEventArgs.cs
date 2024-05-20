using System;
using System.Runtime.Serialization;

namespace CGLibs.Scheduling
{
    [DataContract, Serializable()]
    public class ScheduleStatusEventArgs : EventArgs
    {

        public ScheduleStatusEventArgs(Schedule sender, string taskid, DateTime nextrun)
        {
            Schedule = sender;
            NextRunAt = nextrun;
            TaskId = taskid;
        }

        [DataMember]
        public string TaskId { get; private set; }
        [DataMember]
        public DateTime NextRunAt { get; private set; }
        [DataMember]
        public Schedule Schedule { get; private set; }

    }
}
