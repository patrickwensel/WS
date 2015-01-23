namespace AS.App.Intranet.ViewModels.SafetyIncident
{
    public class IncidentViewModel
    {
        public string UserName { get; set; }
        public bool AccessToSafetyIncidentApplication { get; set; }
        public string DeniedAccessMessage { get; set; }
    }
}