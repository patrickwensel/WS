namespace WS.Framework.Objects.LDAP
{
    public class SafetyIncidentAccess
    {
        public bool AccessToSafetyIncidentApplication { get; set; }
        public bool ReadOnly { get; set; }
        public string AccessHierarchyLevel { get; set; }
        public decimal HierarchyID { get; set; }
    }
}
