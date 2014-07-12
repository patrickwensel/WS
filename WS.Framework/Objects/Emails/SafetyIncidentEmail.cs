using System;

namespace WS.Framework.Objects.Emails
{
    public class SafetyIncidentEmail
    {
        public string ToAddress { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateOfIncident { get; set; }
        public string Country { get; set; }
        public string Location { get; set; }
        public string EmployeeType { get; set; }
        public string WorkRelated { get; set; }
        public string Outcome { get; set; }
        public string DescriptionOfIncident { get; set; }
        public string UnsafeActCondition { get; set; }
        public string RootCase { get; set; }
        public string Recordable { get; set; }
        public string ContactMessage { get; set; }
        public string Status { get; set; }
        public Decimal ID { get; set; }
        public string EmailMessage { get; set; }
    }
}
