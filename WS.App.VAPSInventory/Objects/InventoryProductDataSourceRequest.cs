using Kendo.Mvc.UI;

namespace WS.App.VAPSInventory.Objects
{
    public class InventoryProductDataSourceRequest : DataSourceRequest
    {
        public int LocationNumber { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }

    }
}