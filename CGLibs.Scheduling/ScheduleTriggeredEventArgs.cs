using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace CGLibs.Scheduling
{
    [DataContract, Serializable()]
    public class ScheduleTriggeredEventArgs : EventArgs
    {

        public ScheduleTriggeredEventArgs(Schedule sender, string triggeredid) : this(sender, triggeredid, DateTime.UtcNow)
        {

        }

        public ScheduleTriggeredEventArgs(Schedule sender, string triggeredid, DateTime triggeredat)
        {
            Sender = sender;
            TriggeredId = triggeredid;
            TriggeredAt = triggeredat;
        }

        public ScheduleTriggeredEventArgs(Schedule sender, ISchedulable schedulable, string triggeredid, DateTime triggeredat) : this(sender, triggeredid, triggeredat)
        {
            Schedulable = schedulable;
        }

        [DataMember]
        public Schedule Sender { get; private set; }

        [DataMember]
        public string TriggeredId { get; private set; }

        [DataMember]
        public DateTime TriggeredAt { get; private set; }

        [IgnoreDataMember, XmlIgnore()]
        public ISchedulable Schedulable { get; set; }

        public bool Cancel = false;

    }
}
