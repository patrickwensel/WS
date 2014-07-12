namespace WS.Framework.Objects.Emails
{
    public class UpdateAttributesEmail
    {
        public string To { get; set; }
        public string BranchName { get; set; }
        public string UnitNumber { get; set; }
        public string Name { get; set; }
        public string AttributeChanges { get; set; }
        public string Note { get; set; }
    }
}
