using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TransportProgressRecord = PsGet.Helper.ProgressRecord;
using TransportProgressRecordType = PsGet.Helper.ProgressRecordType;
using PshProgressRecord = System.Management.Automation.ProgressRecord;
using PshProgressRecordType = System.Management.Automation.ProgressRecordType;

namespace PsGet.Converters {
    internal static class ProgressRecordConverter {
        internal static PshProgressRecord ToActual(this TransportProgressRecord record) {
            return new PshProgressRecord(record.ActivityId, record.Activity, record.StatusDescription) {
                CurrentOperation = record.CurrentOperation,
                ParentActivityId = record.ParentActivityId,
                PercentComplete = record.PercentComplete,
                RecordType = record.RecordType.ToActual(),
                SecondsRemaining = record.SecondsRemaining
            };
        }

        internal static PshProgressRecordType ToActual(this TransportProgressRecordType type) {
            return (PshProgressRecordType)type;
        }
    }
}
