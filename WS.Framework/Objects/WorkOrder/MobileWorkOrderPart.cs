namespace WS.Framework.Objects.MobileWorkOrder
{
    public class MobileWorkOrderPart
    {
        public string ID { get; set; }
        public string Description { get; set; }
        public string SearchText { get; set; }
        public string Region { get; set; }
        public string SalesCatalogSection { get; set; }
        public int PartID { get; set; }
        public string Part { get; set; }
        public int Qty { get; set; }
        public int DamageBillingPercentage { get; set; }
        public string UnitofMeasurePrimary { get; set; }
    }
}
