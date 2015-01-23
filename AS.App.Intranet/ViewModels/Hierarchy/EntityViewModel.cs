namespace AS.App.Intranet.ViewModels.Hierarchy
{
    public class EntityViewModel
    {
        public decimal ID { get; set; }
        public decimal? LocationID { get; set; }
        public decimal? StatusID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string CostCenter { get; set; }
        public string Status { get; set; }
    }
}