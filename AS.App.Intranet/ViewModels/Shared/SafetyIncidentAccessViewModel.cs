namespace AS.App.Intranet.ViewModels.Shared
{
    public class SafetyIncidentAccessViewModel
    {
        public bool AccessToSafetyIncidentApplication { get; set; }
        public bool ReadOnly { get; set; }
        public string AccessHierarchyLevel { get; set; }
        public decimal HierarchyID { get; set; }
    }
}