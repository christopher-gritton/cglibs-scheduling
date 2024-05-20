

using System.Runtime.Serialization;

namespace CGLibs.Scheduling
{

    [DataContract]
    public enum Recurrent
    {
        [EnumMember]
        Minutely = 0,
        [EnumMember]
        Hourly = 4,
        [EnumMember]
        Daily = 8,
        [EnumMember]
        Weekly = 16,
        [EnumMember]
        Monthly = 32
    }

}