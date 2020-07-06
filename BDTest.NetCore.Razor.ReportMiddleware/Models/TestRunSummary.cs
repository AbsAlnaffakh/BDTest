using System;
using BDTest.Test;

namespace BDTest.NetCore.Razor.ReportMiddleware.Models
{
    public class TestRunSummary
    {
        public TestRunSummary(string recordId, DateTime dateTime, Status status, int failedCount, string tag, string environment)
        {
            RecordId = recordId;
            DateTime = dateTime;
            Status = status;
            FailedCount = failedCount;
            Tag = tag;
            Environment = environment;
        }

        public string RecordId { get; set; }
        public DateTime DateTime { get; set; }
        public Status Status { get; set; }
        public int FailedCount { get; set; }
        public string Tag { get; set; }
        public string Environment { get; set; }
    }
}