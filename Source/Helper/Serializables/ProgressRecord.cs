using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PsGet.Helper.Serializables {
    [DataContract]
    public enum ProgressRecordType {
        [EnumMember]
        Processing = 0,
        [EnumMember]
        Completed = 1,
    }

    [DataContract]
    public class ProgressRecord {
        public ProgressRecord() { }
        public ProgressRecord(string activity, string statusDescription) {
            Activity = activity;
            StatusDescription = statusDescription;
        }

        [DataMember]
        public string Activity { get; set; }
        [DataMember]
        public int ActivityId { get; set; }
        [DataMember]
        public string CurrentOperation { get; set; }
        [DataMember]
        public int ParentActivityId { get; set; }
        [DataMember]
        public int PercentComplete { get; set; }
        [DataMember]
        public ProgressRecordType RecordType { get; set; }
        [DataMember]
        public int SecondsRemaining { get; set; }
        [DataMember]
        public string StatusDescription { get; set; }
    }
}
